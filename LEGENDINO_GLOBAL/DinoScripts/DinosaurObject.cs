using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

/// <summary>
/// 이벤트에 물려 있는 최종 이펙트 
/// </summary>
public class DinoActionEventEffect{
    private GameObject m_activeEffect;
    private GameObject m_targetTakeEffect;

    public GameObject activeEffect{get{return m_activeEffect;}}
    public GameObject targetTakeEffect{get{return m_targetTakeEffect;}}

    public DinoActionEventEffect(DinoActionMap map)
    {
        GameObject activeEffectAsset = null;
        GameObject targetEffectAsset = null;
        #if UNITY_ANDROID && UNITY_EDITOR_OSX
        if (!string.IsNullOrEmpty(map.activeEffIndex) && !map.activeEffIndex.Equals("None"))
        {
            string assetPath = "Assets/DinoPro/AssetResources/Effect_Opt/BattleEff_Use/eff_{0}.prefab";
            string assetFullName = string.Format(assetPath, map.activeEffIndex.ToString());
            activeEffectAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetFullName);
        }
        else if(map.activeEffectObject != null)
        {
            m_activeEffect = map.activeEffectObject;
        }

        if (!string.IsNullOrEmpty(map.targetTakeEffIndex) && !map.activeEffIndex.Equals("None"))
        {
            string assetPath = "Assets/DinoPro/AssetResources/Effect_Opt/BattleEff_Use/eff_{0}.prefab";
            string assetFullName = string.Format(assetPath, map.targetTakeEffIndex.ToString());
            targetEffectAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetFullName);
        }
        else if(map.targetTakeEffectObject != null)
        {
            m_targetTakeEffect = map.targetTakeEffectObject;
        }
        
        #elif UNITY_ANDROID && UNITY_EDITOR_WIN
        if (!string.IsNullOrEmpty(map.activeEffIndex) && !map.activeEffIndex.Equals("None"))
        {
            string assetPath = "Assets/DinoPro/AssetResources/Effect_Opt/BattleEff_Use/eff_{0}.prefab";
            string assetFullName = string.Format(assetPath, map.activeEffIndex.ToString());
            activeEffectAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetFullName);
        }
        else if(map.activeEffectObject != null)
        {
            m_activeEffect = map.activeEffectObject;
        }

        if (!string.IsNullOrEmpty(map.targetTakeEffIndex) && !map.activeEffIndex.Equals("None"))
        {
            string assetPath = "Assets/DinoPro/AssetResources/Effect_Opt/BattleEff_Use/eff_{0}.prefab";
            string assetFullName = string.Format(assetPath, map.targetTakeEffIndex.ToString());
            targetEffectAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetFullName);
        }
        else if(map.targetTakeEffectObject != null)
        {
            m_targetTakeEffect = map.targetTakeEffectObject;
        }
        #elif UNITY_ANDROID
        #endif

        if(activeEffectAsset != null)
        {
            m_activeEffect = MonoBehaviour.Instantiate<GameObject>(activeEffectAsset);
            m_activeEffect.transform.SetParent(map.bone);
            m_activeEffect.transform.localPosition = Vector3.zero;
            m_activeEffect.transform.localScale = activeEffectAsset.transform.localScale;
            m_activeEffect.SetActive(false);
        }

        if(targetEffectAsset != null)
        {
            m_targetTakeEffect = MonoBehaviour.Instantiate<GameObject>(activeEffectAsset);
            m_targetTakeEffect.SetActive(false);
        }
        
    }

    public void PlayActiveEffect()
    {
        // Debug.Log("Play Dino Action Event Effect");

        if(m_activeEffect != null)
        {
            m_activeEffect.SetActive(false);
            m_activeEffect.SetActive(true);
        }

    }
}

/// <summary>
/// 애니메이션에 들어있는 이벤트 리스트
/// </summary>
public class DinoActionEvents
{
    private Dictionary<string, List<DinoActionEventEffect>> m_eventEffects;
    

    public DinoActionEvents(DinoActionMap map)
    {
        m_eventEffects = new Dictionary<string, List<DinoActionEventEffect>>();
        List<DinoActionEventEffect> eventEffects = new List<DinoActionEventEffect>();
        eventEffects.Add(new DinoActionEventEffect(map));

        m_eventEffects.Add(map.eventName, eventEffects);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    public void AddDinoActionEvent(DinoActionMap map)
    {
        string eventName = map.eventName;
        if(m_eventEffects.ContainsKey(eventName))
        {
            m_eventEffects[eventName].Add(new DinoActionEventEffect(map));
        }
        else
        {
            List<DinoActionEventEffect> eventEffects = new List<DinoActionEventEffect>();
            eventEffects.Add(new DinoActionEventEffect(map));
            m_eventEffects.Add(eventName, eventEffects);   
        }
    }


    public void PlayEventEffect(string _name)
    {
        if(m_eventEffects.ContainsKey(_name))
        {
            List<DinoActionEventEffect> effectList = m_eventEffects[_name];
            for(int i = 0; i < effectList.Count; i++)
            {
                effectList[i].PlayActiveEffect();
            }
        }
    }
}

/// <summary>
/// 저장되어있는 액션 맵에서 공룡이 활성화 될때 따로 저장해둔다
/// </summary>
public class DinoAction
{
    // private DinoActionType m_actionType;
    private Dictionary<string, DinoActionEvents> m_animationEventEffects;


    private List<DinoAnimation> m_dinoAnimations;
    public List<DinoAnimation> dinoAnimations{get{return m_dinoAnimations;}}

    private List<DinoActionAnimationLoopSetting> m_loopSettings; 

    // public DinoActionType actionType{get{return m_actionType;}}
    // public List<string> animations{get{return m_animations;}}

    public DinoAction(DinoActionMap map, List<DinoActionAnimationLoopSetting> _loopSettings = null)
    {
        m_animationEventEffects = new Dictionary<string, DinoActionEvents>();
        m_animationEventEffects.Add(map.animationName, new DinoActionEvents(map));

        m_dinoAnimations = new List<DinoAnimation>();
        m_loopSettings = _loopSettings;

        DinoActionAnimationLoopSetting loopSetting = GetLoopSetting(map.animationName);
        m_dinoAnimations.Add(new DinoAnimation(map.animationName, loopSetting));
    }

    public void AddAnimation(DinoActionMap map)
    {
        string animName = map.animationName;
        if(m_animationEventEffects.ContainsKey(animName))
        {
            m_animationEventEffects[animName].AddDinoActionEvent(map);
        }
        else
        {
            m_animationEventEffects.Add(animName, new DinoActionEvents(map));
            DinoActionAnimationLoopSetting loopSetting = GetLoopSetting(map.animationName);
            m_dinoAnimations.Add(new DinoAnimation(map.animationName, loopSetting));
        }
    }

    public DinoActionEvents GetAnimationEventList(string animName)
    {
        DinoActionEvents eventList = null;

        if(m_animationEventEffects.ContainsKey(animName))
        {
            eventList = m_animationEventEffects[animName];
        }
        

        return eventList;
    }

    private DinoActionAnimationLoopSetting GetLoopSetting(string animationName)
    {
        if(m_loopSettings == null || m_loopSettings.Count <= 0)
            return null;

        DinoActionAnimationLoopSetting loopSetting = null;
        for(int i = 0; i < m_loopSettings.Count; i++)
        {
            if(m_loopSettings[i].animationName.Equals(animationName))
            {
                loopSetting = m_loopSettings[i];
            }
        }
        return loopSetting;
    }
}

public class DinoAnimation
{
    private string m_animationName;
    private bool m_loop = false;
    private float m_playTime = 0f;

    public string animationName{get{return m_animationName;}}
    public bool loop{get{return m_loop;}}
    public float playTime
    {
        get{
            float time = 0f;
            if(m_loop)
                time = m_playTime;

            return time;
        }
    }

    public DinoAnimation(string _name, DinoActionAnimationLoopSetting setting = null)
    {
        m_animationName = _name;
        if(setting != null)
        {
            m_loop = true;
            m_playTime = setting.playtime;
        }
    }  
}



[DisallowMultipleComponent]
// [ExecuteInEditMode]
public class DinosaurObject : MonoBehaviour
{
    protected SkeletonAnimation m_spineAnimation;

    [SerializeField]private int m_evolveCount;

    public int evolveCount{get{return m_evolveCount;}}

    //DinoActionMap.cs 직렬화 가능 선언
    [SerializeField]private List<DinoActionMap> m_dinoActionMaps = new List<DinoActionMap>();
    
    [SerializeField]
    private List<DinoActionAnimationLoopSetting> m_dinoActionAnimationLoopSettings = new List<DinoActionAnimationLoopSetting>();

    private bool m_attachEffect = false;
    public bool attachEffect{set{m_attachEffect = value;}}

    private Dictionary<DinoActionType, DinoAction> m_dinoActions;

    private DinoActionType m_currentActionType;
    // public Dictionary<DinoAction, List<DinoActionMap>> actionMapsDictionary;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// 액션 데이터 초기화 - 직렬화 된 데이터들을 읽어 들여 참조 가능한 형태로 풀어둠
    /// </summary>
    void Awake()
    {
        InitializeDinoAction();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        InitializeDinosaur();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // Debug.Log("dinosaur object enable");
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// 이 컴포넌트를 붙일때 기본적으로 세팅되어야 할 값들을 미리 세팅할 수 있도록 합니디.
    /// 레전다이노 배틀에서 기본적으로 공통 애니메이션을 쓰는 부분은 미리 자동으로 설정되도록 처리합니다.
    /// </summary>
    void Reset()
    {
        Debug.Log("Dinosaur Object AddComponent ######### ");
    }

    private void InitializeDinoAction()
    {
        m_dinoActions = new Dictionary<DinoActionType, DinoAction>();
        for(int i = 0; i < m_dinoActionMaps.Count; i++)
        {
            DinoActionType action = m_dinoActionMaps[i].action;
            if (m_dinoActions.ContainsKey(action))
            {
                m_dinoActions[action].AddAnimation(m_dinoActionMaps[i]);
            }
            else
            {
                List<DinoActionAnimationLoopSetting> settings = GetLoopSettingsForAction(action);
                m_dinoActions.Add(action, new DinoAction(m_dinoActionMaps[i], settings));
            }
        }
    }

    private List<DinoActionAnimationLoopSetting> GetLoopSettingsForAction(DinoActionType _type)
    {
        List<DinoActionAnimationLoopSetting> settings = new List<DinoActionAnimationLoopSetting>();

        for(int i = 0; i < m_dinoActionAnimationLoopSettings.Count; i++)
        {
            if(m_dinoActionAnimationLoopSettings[i].action.Equals(_type))
                settings.Add(m_dinoActionAnimationLoopSettings[i]);
        } 

        return settings;
    }


    /// <summary>
    /// 액션에 할당된 애니메이션 루프 값을 가져 온다
    /// </summary>
    /// <param name="_action"></param>
    /// <returns></returns>
    public List<DinoActionMap> GetDinoActionMapForAction(DinoActionType _action)
    {
        List<DinoActionMap> actionMaps = new List<DinoActionMap>();
        if(m_dinoActionMaps == null)
            return actionMaps;

        for(int i = 0; i < m_dinoActionMaps.Count; i++)
        {
            if(m_dinoActionMaps[i].action.Equals(_action))
                actionMaps.Add(m_dinoActionMaps[i]); 
        }

        return actionMaps;
    }

    /// <summary>
    /// 이벤트 셋팅부분
    /// 이벤트는 어웨이크에서 세팅할 경우 세팅되지 않아 Start에서 세팅해야 함
    /// </summary>
    private void InitializeDinosaur()
    {
        m_spineAnimation = GetComponent<SkeletonAnimation>();
        if (m_spineAnimation == null)
            return;

        m_spineAnimation.AnimationState.Start += HandleAnimationStateStartEvent;
        m_spineAnimation.AnimationState.Event += HandleAnimationStateEvents;
        m_spineAnimation.AnimationState.End += HandleAnimationStateEndEvent;

        // m_spineAnimation.AnimationState.Start 

		// Debug.Log(m_spineAnimation.skeleton); 
        // m_spineAnimation.skeleton.Data.Animations.
        // Spine.EventData[] animations = m_spineAnimation.skeleton.Data.Events.Items;

        // for(int i = 0; i < animations.Length; i++)
        // {
        // 	Debug.Log(animations[i].Name);
        // }
        // ExposedList<TrackEntry> tracks = m_spineAnimation.state.Tracks;
        // for(int i = 0; i < tracks.Count; i++)
        // {
        // 	// Debug.Log(tracks[i]);
        // }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    private void HandleAnimationStateStartEvent(TrackEntry entry)
    {
        // Debug.Log("StartEvent animation name : " + entry.Animation.Name);

        if(m_dinoActions.ContainsKey(m_currentActionType) == false)
            return;

        DinoActionEvents events = m_dinoActions[m_currentActionType].GetAnimationEventList(entry.Animation.Name);
        if(events != null)
        {
            events.PlayEventEffect("start");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="e"></param>
    private void HandleAnimationStateEvents(TrackEntry entry, Spine.Event e)
    {
        // Debug.Log("AnimationStateEvent animation name : " + entry.Animation.Name);
        // Debug.Log("AnimationStateEvent event name : " + e.ToString());


        if(m_dinoActions.ContainsKey(m_currentActionType) == false)
            return;

        DinoActionEvents events = m_dinoActions[m_currentActionType].GetAnimationEventList(entry.Animation.Name);
        if(events != null)
        {
            events.PlayEventEffect(e.Data.Name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    private void HandleAnimationStateEndEvent(TrackEntry entry)
    {
        Debug.Log("AnimationStateEndEvent animation name : " + entry.Animation.Name);

        if(m_dinoActions.ContainsKey(m_currentActionType) == false)
            return;

        DinoActionEvents events = m_dinoActions[m_currentActionType].GetAnimationEventList(entry.Animation.Name);
        if(events != null)
        {
            events.PlayEventEffect("end");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    public void PlayDinoAction(DinoActionType type)
    {
        m_currentActionType = type;
        if(m_dinoActions.ContainsKey(m_currentActionType) == false)
            return;

        List<DinoAnimation> animationSets = m_dinoActions[m_currentActionType].dinoAnimations;
        float waitTime = 0f;
        for(int i = 0; i < animationSets.Count; i++)
        {
            DinoAnimation animationSet = animationSets[i];
            if(i <= 0)
            {
                m_spineAnimation.AnimationState.SetAnimation(0, animationSet.animationName, animationSet.loop).MixDuration = 0f;
            }
            else
            {
                m_spineAnimation.AnimationState.AddAnimation(0, animationSet.animationName, animationSet.loop, waitTime);
            }

            waitTime += animationSet.playTime;
        }

        // 평상시 애니메이션으로 복귀함
        m_spineAnimation.AnimationState.AddAnimation(0, "idle", true, waitTime).MixDuration = 0f;
    }

    /// <summary>
    /// 주의 !! 
    /// 런타임에서 절대 참조하지 말것 
    /// </summary>
    /// <param name="actionMap"></param>
    public void AddDinoActionMap(DinoActionMap actionMap)
    {
        m_dinoActionMaps.Add(actionMap);
    }

    /// <summary>
    /// 주의 !! 
    /// 런타임에서 절대 참조하지 말것
    /// </summary>
    /// <param name="actionMap"></param>
    public void DeleteActionMap(DinoActionMap actionMap)
    {
        if(m_dinoActionMaps.Contains(actionMap))
        {
            m_dinoActionMaps.Remove(actionMap);
        }
    }

    /// <summary>
    /// 주의 !! 
    /// 런타임에서 절대 참조하지 말것  
    /// </summary>
    public void ClearAllDinoActionMap()
    {
        m_dinoActionMaps.Clear();
    }

    /// <summary>
    /// 주의 !! 
    /// 무조건 인스펙터 세팅 구간에서 사용
    /// </summary>
    /// <param name="count"></param>
    public void SetEvolveCount(int count)
    {
        m_evolveCount = count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="_animName"></param>
    /// <returns></returns>
    public DinoActionAnimationLoopSetting ContainsDinoActionAnimationLoopSetting(DinoActionType actionType, string _animName)
    {
        DinoActionAnimationLoopSetting setting = null;

        // string dinoActiontype = actionType.ToString();
        for(int i = 0; i < m_dinoActionAnimationLoopSettings.Count; i++)
        {
            DinoActionType type = m_dinoActionAnimationLoopSettings[i].action;
            string animName = m_dinoActionAnimationLoopSettings[i].animationName;
            if (type.Equals(actionType) && animName.Equals(_animName))
            {
                setting = m_dinoActionAnimationLoopSettings[i];
                break;   
            }
        }
        return setting;
    }

    public void AddDinoActionAnimationLoopSetting(DinoActionAnimationLoopSetting setting)
    {
        m_dinoActionAnimationLoopSettings.Add(setting);
    }

    public void RemoveDinoActionAnimationLoopSetting(DinoActionType actionType, string _animName)
    {
        DinoActionAnimationLoopSetting setting = null;
        for(int i = 0; i < m_dinoActionAnimationLoopSettings.Count; i++)
        {
            DinoActionType type = m_dinoActionAnimationLoopSettings[i].action;
            string animName = m_dinoActionAnimationLoopSettings[i].animationName;
            if (type.Equals(actionType) && animName.Equals(_animName))
            {
                setting = m_dinoActionAnimationLoopSettings[i];
                break;   
            }
        }

        if(setting != null)
            m_dinoActionAnimationLoopSettings.Remove(setting);
    }

    public void ApplyChangeAnimationSettingValue()
    {
        InitializeDinoAction();
    }
}
