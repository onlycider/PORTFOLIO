using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class DinoObject : MonoBehaviour
{
    private SkeletonAnimation m_skeletonAnimation;
    public SkeletonAnimation skeletonAnimation{get{return m_skeletonAnimation;}}

    //중요 스킬 시작 이펙트 key - 애니메이션 이름, value - 발동되어야 할 시작 이펙트
    private Dictionary<string, DinoSkillAnimationEffects> m_normalSkillEffects;

    private List<SpecialSkillStartEffectController> m_specialSkillStartEffects;

    private Dictionary<int, List<StartSkillEffectController>> m_skillStartEffects;

    private List<AlwaysDinoEffectController> m_alwaysEffects;

    private Dictionary<string, MultiEventSkillEffect> m_multiEventSkillEffects;

    //기본 피격 이펙트들 - 공격 애니메이션 시작후 9프레임에 터지는 이펙트가 기본 이펙트
    // private Dictionary<string, TargetDinoEffects> m_normalTargetDinoEffects;
    private Dictionary<string, DinoSkillTargetEffects> m_normalTargetDinoEffects;

    //multi와 같이 한 애니메이션에서 여러 개의 이벤트가 발동될때 그에 맞춘 이펙트를 여러개 생성하여 띄워주는 경우
    private Dictionary<string, MultiEventTargetDinoEffects> m_multiEventTargetDinoEffects;

    private Dictionary<string, List<BulletTargetDinoEffectController>> m_bulletEffects;



    private Transform m_targetTransform;
    private Transform m_bodyTransform;

    private string m_playedEffectAnimation = string.Empty;

    private int m_dinoRenderOrder = 0;

    private bool m_flip = false;
    private bool m_initialized = false;
    private int m_selectedSkillIndex;
    public float effectScale = 0f;

    private Action<bool> INGAME_MULTIHIT_DISPLAY;

    private List<GameObject> m_reinforceEffects;
    
    void Awake()
    {
        m_skeletonAnimation = GetComponent<SkeletonAnimation>();
        m_bodyTransform = transform.Find("EffBorn_Center").transform; //TODO : Center로 변경해야함
        
        if (m_bodyTransform == null)
            m_bodyTransform = transform.Find("EffBorn_Body").transform;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(m_initialized)
            return;

        m_dinoRenderOrder = m_skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder;
        EnrollEvents();
        EnrollDinoEffects();
        PlayAlwaysEffect();
        m_initialized = true;

    }

    public void SetDinoObject(bool _isEnemy = false)
    {
        if(m_initialized)
            return;

        m_flip = _isEnemy;
        m_dinoRenderOrder = m_skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder;
        EnrollEvents();
        EnrollDinoEffects();
        PlayAlwaysEffect();
        m_initialized = true;
    }
    
    public void SetDinoObject(DinoInfo userDinoInfo, bool _isEnemy = false)
    {
        if(m_initialized)
            return;

        m_flip = _isEnemy;
        m_dinoRenderOrder = m_skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder;
        EnrollEvents();
        EnrollDinoEffects();
        PlayAlwaysEffect();
        CreateDinoReinforceEffects(userDinoInfo);
        
        m_initialized = true;
    }

    private void CreateDinoReinforceEffects(DinoInfo userDinoInfo)
    {
        int reinforceIndex = userDinoInfo.nIntention;
        int effectCount = reinforceIndex / 3;
        Vector3 temp = effectScale * new Vector3(102, 102, 102);
        m_reinforceEffects = new List<GameObject>();

        for (int i = 1; i <= effectCount; i++)
        {
            EffectController effect = Utils.GetCreatedObjectComponent<EffectController>($"CommonObjects/force{i}", m_bodyTransform);
            int renderOrder = i == 1 || i == 3 ? m_dinoRenderOrder + 1 : m_dinoRenderOrder - 1;
            effect.SetEffectDepth(renderOrder);
            effect.transform.localScale =temp;//clone.transform.localScale;
            m_reinforceEffects.Add(effect.gameObject);
            
            if (i == 3)
            {
                EffectController effect2 = Utils.GetCreatedObjectComponent<EffectController>($"CommonObjects/force{effectCount + 1}", m_bodyTransform);
                effect2.SetEffectDepth(m_dinoRenderOrder - 1);
                effect2.transform.localScale = temp;//effectScale * clone2.transform.localScale;
                m_reinforceEffects.Add(effect2.gameObject);
            }
        }
    }
    
    public void RefreshDinoReinforceEffect(DinoInfo userDinoInfo)
    {
        int reinforceIndex = userDinoInfo.nIntention;
        if (!(reinforceIndex == 3 || reinforceIndex == 6 || reinforceIndex == 9))
            return;
        
        int effectCount = reinforceIndex / 3;
        Vector3 temp = effectScale * new Vector3(102, 102, 102);
        EffectController effect = Utils.GetCreatedObjectComponent<EffectController>($"CommonObjects/force{effectCount}", m_bodyTransform);
        effect.SetEffectDepth(m_dinoRenderOrder + effectCount + 1);
        effect.transform.localScale = temp;
        
        if (effectCount == 3)
        {
            EffectController effect2 = Utils.GetCreatedObjectComponent<EffectController>($"CommonObjects/force{effectCount + 1}", m_bodyTransform);
            effect2.SetEffectDepth(m_dinoRenderOrder + effectCount + 1);
            effect2.transform.localScale = temp;//effectScale * clone2.transform.localScale;
        }
    }

    void OnDestroy () {
        if (m_skeletonAnimation == null)
            return;
        
        m_skeletonAnimation.AnimationState.Start -= HandleAnimationStateStartEvent;
        m_skeletonAnimation.AnimationState.Event -= HandleAnimationStateEvents;
        m_skeletonAnimation.AnimationState.End -= HandleAnimationStateEndEvent;
    }

    private void EnrollEvents()
    {
        m_skeletonAnimation.AnimationState.Start += HandleAnimationStateStartEvent;
        m_skeletonAnimation.AnimationState.Event += HandleAnimationStateEvents;
        m_skeletonAnimation.AnimationState.End += HandleAnimationStateEndEvent;
    }

    // private Dictionary<int, string> m_skillAnimations;

    // private void EnrollSkillAnimations()
    // {
    //     m_skillAnimations = new Dictionary<int, string>();
    //     m_skillAnimations.Add(1, soul);
    // }

    private void EnrollDinoEffects()
    {
        m_specialSkillStartEffects = new List<SpecialSkillStartEffectController>();
        m_skillStartEffects = new Dictionary<int, List<StartSkillEffectController>>();
        m_alwaysEffects = new List<AlwaysDinoEffectController>();
        m_normalSkillEffects = new Dictionary<string, DinoSkillAnimationEffects>();
        m_multiEventSkillEffects = new Dictionary<string, MultiEventSkillEffect>();
        // m_normalTargetDinoEffects = new Dictionary<string, TargetDinoEffects>();
        m_normalTargetDinoEffects = new Dictionary<string, DinoSkillTargetEffects>();
        m_multiEventTargetDinoEffects = new Dictionary<string, MultiEventTargetDinoEffects>();
        DinoEffectController[] effects = GetComponentsInChildren<DinoEffectController>(true);
        foreach(DinoEffectController effect in effects)
        {
            effect.SetDepth(m_dinoRenderOrder);
            effect.SetSpineAnimation(m_skeletonAnimation);
            effect.Initialize();

            if(effect is SpecialSkillStartEffectController)
            {
                m_specialSkillStartEffects.Add(effect as SpecialSkillStartEffectController);
                continue;
            }

            if(effect is StartSkillEffectController)
            {
                EnrollSkillStartEffect(effect as StartSkillEffectController);
                continue;
            }
            //MultiEventTargetDinoEffectController는 TargetDinoEffectController를 상속 받았기 때문에 순서상 제일 먼저 체크해줘야 한다.
            if(effect is MultiEventTargetDinoEffectController)
            {
                EnrollMultiEventTargetEffect(effect);
                continue;
            }
            if(effect is TargetDinoEffectController)
            {
                EnrollNormalTargetEffect(effect);
                continue;
            }

            if(effect is MultiEventDinoEffectController)
            {
                EnrollMultiEventSkillEffect(effect);
                continue;
            }

            if(effect is AlwaysDinoEffectController)
            {
                m_alwaysEffects.Add(effect as AlwaysDinoEffectController);
                continue;
            }

            string animationName = effect.animationName;

            if(m_normalSkillEffects.ContainsKey(animationName))
                m_normalSkillEffects[animationName].AddDinoSkillEffect(effect);
            else
            {
                DinoSkillAnimationEffects skillEffects = new DinoSkillAnimationEffects();
                skillEffects.AddDinoSkillEffect(effect);
                m_normalSkillEffects.Add(animationName, skillEffects);
            }
        }
    }

    public void PlaySpecialSkillStartEffect(int skillType = -1)
    {
        if(m_specialSkillStartEffects.Count > 0)
        {
            foreach(SpecialSkillStartEffectController effect in m_specialSkillStartEffects)
                effect.PlayDinoEffect();

            return;
        }

        if(m_skillStartEffects.ContainsKey(skillType))
        {
            List<StartSkillEffectController> effects = m_skillStartEffects[skillType];
            foreach(StartSkillEffectController effect in effects)
                effect.PlayDinoEffect();
        }
    }

    private void PlayAlwaysEffect()
    {
        foreach(AlwaysDinoEffectController effect in m_alwaysEffects)
            effect.PlayDinoEffect();
    }

    private void DeactivateAlwaysEffect()
    {
        foreach(AlwaysDinoEffectController effect in m_alwaysEffects)
            effect.gameObject.SetActive(false);
    }

    private void EnrollSkillStartEffect(StartSkillEffectController effect)
    {
        int typeIndex = effect.skillTypeIndex;

        if(m_skillStartEffects.ContainsKey(typeIndex))
        {
            List<StartSkillEffectController> effects = m_skillStartEffects[typeIndex];
            effects.Add(effect);
        }
        else
        {
            List<StartSkillEffectController> effects = new List<StartSkillEffectController>();
            effects.Add(effect);
            m_skillStartEffects.Add(typeIndex, effects);
        }
    }

    private void EnrollMultiEventSkillEffect(DinoEffectController effect)
    {
        string animationName = effect.animationName;
        MultiEventDinoEffectController effectController = effect as MultiEventDinoEffectController;
        if(m_multiEventSkillEffects.ContainsKey(animationName))
            m_multiEventSkillEffects[animationName].AddMultiEventSkillEffect(effectController);
        else
        {
            MultiEventSkillEffect multiEventSkillEffect = new MultiEventSkillEffect();
            multiEventSkillEffect.AddMultiEventSkillEffect(effectController);
            m_multiEventSkillEffects.Add(animationName, multiEventSkillEffect);
        }        
    }

    private void EnrollNormalTargetEffect(DinoEffectController effect)
    {
        string animationName = effect.animationName;
        TargetDinoEffectController targetDinoEffect = effect as TargetDinoEffectController;
        targetDinoEffect.SetOriginParent();

        if(m_flip)
            targetDinoEffect.SetFlip();

        if(m_normalTargetDinoEffects.ContainsKey(animationName))
            m_normalTargetDinoEffects[animationName].AddDinoSkillTargetEffect(targetDinoEffect);
        else
        {
            DinoSkillTargetEffects targetEffects = new DinoSkillTargetEffects();
            targetEffects.AddDinoSkillTargetEffect(targetDinoEffect);
            m_normalTargetDinoEffects.Add(animationName, targetEffects);
        }
    }

    private void EnrollMultiEventTargetEffect(DinoEffectController effect)
    {
        string animationName = effect.animationName;
        MultiEventTargetDinoEffectController multiEventTargetEffect = effect as MultiEventTargetDinoEffectController;
        multiEventTargetEffect.SetOriginParent();

        if(m_flip)
            multiEventTargetEffect.SetFlip();

        if(m_multiEventTargetDinoEffects.ContainsKey(animationName))
            m_multiEventTargetDinoEffects[animationName].AddMultiEventSkillEffect(multiEventTargetEffect);
        else
        {
            MultiEventTargetDinoEffects multiEventTargetDinoEffects = new MultiEventTargetDinoEffects();
            multiEventTargetDinoEffects.AddMultiEventSkillEffect(multiEventTargetEffect);
            m_multiEventTargetDinoEffects.Add(animationName, multiEventTargetDinoEffects);
        }
    }



    /// <summary>
    /// 애니메이션이 시작할때의 이펙트가 나온다.
    /// 1. 상위 공룡의 스킬 스타트 이펙트 (엘그라시아 얼음)
    /// 2. 기본 피격 이펙트 - ex> attack_1의 9프레임에 나오는 피격 이펙트
    /// </summary>
    /// <param name="entry"></param>
    private void  HandleAnimationStateStartEvent(TrackEntry entry)
    {
        string animationName = entry.Animation.Name;
        // if(m_normalSkillEffects.ContainsKey(animationName))
        // {
        //     string eventName = "start";
        //     List<DinoEffectController> effects = m_normalSkillEffects[animationName].GetDinoSkillEffects(eventName);

        //     if(effects != null)
        //     {
        //         foreach(DinoEffectController effect in effects)
        //             effect.PlayDinoEffect();
        //     }
        // }

        string eventName = "none";
        if(m_normalSkillEffects.ContainsKey(animationName))
        {
            List<DinoEffectController> effects = m_normalSkillEffects[animationName].GetDinoSkillEffects(eventName, m_selectedSkillIndex);
            if(effects != null)
            {
                foreach(DinoEffectController effect in effects)
                {
                    if(effect is BulletTargetDinoEffectController)
                    {
                        effect.PlayDinoEffect();
                        BulletTargetDinoEffectController bulletEffect = effect as BulletTargetDinoEffectController;
                        bulletEffect.PlayBulletEffect(this, m_targetTransform);
                        continue;
                    }

                    effect.PlayDinoEffect();
                }
            }
        }

        if(animationName == "die")
            RemoveReinforceEffects();

        if(m_targetTransform == null)
            return;

        if(m_normalTargetDinoEffects.ContainsKey(animationName))
        {
            List<TargetDinoEffectController> effects = m_normalTargetDinoEffects[animationName].GetDinoSkillTargetEffects(eventName, m_selectedSkillIndex);
            if(effects != null)
            {
                foreach(TargetDinoEffectController effect in effects)
                    effect.PlayTargetEffect(this, m_targetTransform);
            }
        }
    }

    /// <summary>
    /// 스킬에 관한 이벤트 이펙트
    /// 1. 애니메이션에서 어떠한 이벤트가 발동되면 그에 맞춘 스킬 이펙트를 보여준다.
    /// 2. 애니메이션에서 발동한 이벤트에 대하여 그에 맞춘 피격 이펙트가 있다면 보여준다.
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="e"></param>
    private void  HandleAnimationStateEvents(TrackEntry entry, Spine.Event e)
    {
        string animationName = entry.Animation.Name;
        string eventName = e.Data.Name;
        if(m_normalSkillEffects.ContainsKey(animationName))
        {
            
            List<DinoEffectController> effects = m_normalSkillEffects[animationName].GetDinoSkillEffects(eventName, m_selectedSkillIndex);
            if(effects != null)
            {
                foreach(DinoEffectController effect in effects)
                {
                    if(effect is BulletTargetDinoEffectController)
                    {
                        effect.PlayDinoEffect();
                        BulletTargetDinoEffectController bulletEffect = effect as BulletTargetDinoEffectController;
                        bulletEffect.PlayBulletEffect(this, m_targetTransform);
                        continue;
                    }

                    effect.PlayDinoEffect();
                }
            }
        }

        if(m_multiEventSkillEffects.ContainsKey(animationName))
        {
            MultiEventSkillEffect multiEventEffects = m_multiEventSkillEffects[animationName]; 
            multiEventEffects.PlayMultiEventSkillEffect(eventName);
            
        }

        if(m_targetTransform == null)
            return;

        if(m_normalTargetDinoEffects.ContainsKey(animationName))
        {
            List<TargetDinoEffectController> effects = m_normalTargetDinoEffects[animationName].GetDinoSkillTargetEffects(eventName, m_selectedSkillIndex);
            if(effects != null)
            {
                foreach(TargetDinoEffectController effect in effects)
                    effect.PlayTargetEffect(m_targetTransform);
            }
        }

        if(m_multiEventTargetDinoEffects.ContainsKey(animationName))
        {
            MultiEventTargetDinoEffects multiEventTargetEffects = m_multiEventTargetDinoEffects[animationName];
            multiEventTargetEffects.PlayMultiEventTargetSkillEffect(eventName, m_targetTransform);
        }

        if(eventName == "multi" || eventName == "multi2")
        {
            // Debug.Log(eventName);
            Utils.InvokeAction(INGAME_MULTIHIT_DISPLAY, m_flip);
        }
    }

    private void HandleAnimationStateEndEvent(TrackEntry entry)
    {
        string animationName = entry.Animation.Name;
        if(m_playedEffectAnimation.Equals(animationName))
        {
            m_multiEventSkillEffects[animationName].InitializeMultiEventSkillEffect();
            m_playedEffectAnimation = string.Empty;
            // m_targetTransform = null;
        }
    }

    public void SetTarget(Transform target)
    {
        m_targetTransform = target;
    }

    public void SetSelectedSkillIndex(int index)
    {
        m_selectedSkillIndex = index;
    }
    

    public void SetAnimation(string Name)
    {
        m_skeletonAnimation.AnimationName = Name;
    }

    public void SetMultiHitIngameDisplayAction(Action<bool> ingame_multihit_display)
    {
        INGAME_MULTIHIT_DISPLAY = ingame_multihit_display;
    }

    public void InitializeMultiHitIngameDisplayAction()
    {
        INGAME_MULTIHIT_DISPLAY = null;
    }

    private void RemoveReinforceEffects()
    {
        if(m_reinforceEffects == null)
            return;

        foreach(GameObject effect in m_reinforceEffects)
        {
            Destroy(effect);
        }

        m_reinforceEffects = null;
    }
}
