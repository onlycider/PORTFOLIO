using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine.Unity;
using System.IO;
using Spine;


[CustomEditor(typeof(DinosaurObject))]
public class DinosaurObjectInspector : Editor {

    public class DinoActionInspectorValues
    {
        public bool foldoutValue = false;
        public int animationSelectIndex = 0;
    }

    DinosaurObject targetDinosaurObject;

    private static List<DinoActionType> m_actionList;
    private static Dictionary<DinoActionType, DinoActionInspectorValues> m_actionInspectorValues;
    private static List<string> dinoAnimations;
    private static List<string> dinoBones;

    private static int m_selectedBoneIndex = 0;


    private DinoAnimationMap m_animationMap;
    private SkeletonData animationData;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        targetDinosaurObject = (DinosaurObject)target;
        
        SkeletonRenderer spineRenderComponent = targetDinosaurObject.GetComponent<SkeletonRenderer>();

        if(spineRenderComponent != null)
        {
            Debug.Log("Setting Skeleton Render Component");
            MakeActionList();
            
            animationData = spineRenderComponent.SkeletonDataAsset.GetSkeletonData(true);
            MakeAnimationList();
            MakeBoneList();

            SetAnimationData(animationData);
        }
        else
        {
            Debug.LogError("Can't Find SkeletonRenderer");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void MakeActionList()
    {
        // 추후 공룡 등급별로 세팅칸을 가변적으로 변경할수 있도록 로직 추가
        m_actionList = new List<DinoActionType>();
        m_actionInspectorValues = new Dictionary<DinoActionType, DinoActionInspectorValues>();

        System.Array actions = Utils.GetEnumValues<DinoActionType>();

        int lastDrawNum = (int)DinoActionType.ACTIVESKILL_FIRST_NOMINEE_3;
        for(int i = 0; i < actions.Length; i++)
        {
            if(targetDinosaurObject.evolveCount == 0 && i > lastDrawNum)
                break;
            
            DinoActionType actionValue = (DinoActionType)actions.GetValue(i);
            m_actionList.Add(actionValue);
            m_actionInspectorValues.Add(actionValue, new DinoActionInspectorValues());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void MakeAnimationList()
    {
        if(animationData == null)
            return;

        dinoAnimations = new List<string>();
        dinoAnimations.Add("none");
        Spine.Animation[] animations = animationData.Animations.Items;
        for(int i = 0; i < animations.Length; i++)
        {
            dinoAnimations.Add(animations[i].Name);
        }
    }

    private void MakeBoneList()
    {
        if(animationData == null)
            return;

        dinoBones = new List<string>();
        dinoBones.Add("none");

        Spine.BoneData[] bones = animationData.Bones.Items;

        for(int i = 0; i < bones.Length; i++)
        {
            dinoBones.Add(bones[i].Name);
        }
    }


    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_animation"></param>
    private void CheckSpineAnimation(Spine.Animation _animation)
    {
        Timeline[] timelines = _animation.Timelines.Items;
        for(int i = 0; i < timelines.Length; i++)
        {
            EventTimeline eventTimeline = timelines[i] as EventTimeline;

            if(eventTimeline != null)
            {
                Spine.Event[] events = eventTimeline.Events;
                CheckSpineAnimationEvent(events);
            }
        }
    }

    override public void OnInspectorGUI ()
    {
        // DrawAnimationEventList();
        DrawActionSettingBox();
    }

    private void DrawActionSettingBox()
    {
        EditorGUILayout.BeginVertical();

        if(GUILayout.Button("변경값 저장"))
        {
            ReplacePrefab();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("evolve count : {0}", targetDinosaurObject.evolveCount));
        
        if(GUILayout.Button("진화 회수 자동 설정"))
        {
            GetDinoEvolveCount();
            MakeActionList();
        }
        EditorGUILayout.EndHorizontal();
        DrawAddChildBoneObject();


        for(int i = 0; i < m_actionList.Count; i++)
        {
            DinoActionType action = m_actionList[i];
            using (new BoxScope()) {
                DinoActionInspectorValues actionSettingValues = m_actionInspectorValues[action];
                if (actionSettingValues.foldoutValue = EditorGUILayout.Foldout(actionSettingValues.foldoutValue, action.ToString()))
                {
                    List<DinoActionMap> serializedActionMaps = targetDinosaurObject.GetDinoActionMapForAction(action);
                    
                    DrawSerializedActionMaps(action, serializedActionMaps);

                    EditorGUILayout.BeginHorizontal();
                    actionSettingValues.animationSelectIndex = EditorGUILayout.Popup(actionSettingValues.animationSelectIndex, dinoAnimations.ToArray());
                    Color org = GUI.color;
                    GUI.color = Color.green;
                    if(GUILayout.Button("add animation"))
                    {
                        string animationName = dinoAnimations[actionSettingValues.animationSelectIndex];
                        if(animationName.Equals("none"))
                        {
                            EditorUtility.DisplayDialog("Invalid Operation", "Can't add none animation.", "Ok");
                        }
                        else
                            targetDinosaurObject.AddDinoActionMap(new DinoActionMap(action, dinoAnimations[actionSettingValues.animationSelectIndex]));
                    }
                    GUI.color = org;
                    EditorGUILayout.EndHorizontal();
                    
                    GUI.color = Color.blue;
                    if(GUILayout.Button("action play"))
                    {
                        PlayDinoAction(action);
                    }
                    GUI.color = org;
                } 
            }
        }

        if(GUILayout.Button("all clear animation"))
        {
            if (EditorUtility.DisplayDialog("Clear all Action Effect", 
                "Are you sure you want to clear all Action effect ??", "Ok", "Cancel"))
            {
                targetDinosaurObject.ClearAllDinoActionMap();    
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawAddChildBoneObject()
    {
        GUILayout.Label("본 추가");
        EditorGUILayout.BeginHorizontal();

        m_selectedBoneIndex = EditorGUILayout.Popup(m_selectedBoneIndex, dinoBones.ToArray());

        if(GUILayout.Button("add bone"))
        {
            string boneName = dinoBones[m_selectedBoneIndex];

            if(boneName.Equals("none"))
            {
                EditorUtility.DisplayDialog("본 오브젝트 생성 오류", "해당 이름으로는 자식에 해당하는 본을 생성할수 없습니다.", "확인");
            }
            else
            {
                CreateChildBoneObject(boneName);
            }
            // CreateChildBoneObject();
            //dinoBones[m_selectedBoneIndex]
        }


        EditorGUILayout.EndHorizontal();
    }

    private void CreateChildBoneObject(string boneName)
    {
        GameObject childBone = new GameObject(boneName, typeof(BoneFollower));
        childBone.transform.SetParent(targetDinosaurObject.transform);
        childBone.transform.localPosition = Vector3.zero;
        BoneFollower boneFollower = childBone.GetComponent<BoneFollower>();
        if(boneFollower != null)
        {
            boneFollower.boneName = boneName;
        }
    }

    private void DrawSerializedActionMaps(DinoActionType actionType, List<DinoActionMap> maps)
    {
        if(maps == null)
            return;

        Dictionary<string, List<DinoActionMap>> serializedActionListDict = new Dictionary<string, List<DinoActionMap>>();
        for(int i = 0; i < maps.Count; i++)
        {
            string animationName = maps[i].animationName;
            if (serializedActionListDict.ContainsKey(animationName))
            {
                serializedActionListDict[animationName].Add(maps[i]);
            }
            else
            {
                List<DinoActionMap> collectedItemsForAnimation = new List<DinoActionMap>();
                collectedItemsForAnimation.Add(maps[i]);
                serializedActionListDict.Add(animationName, collectedItemsForAnimation);
            }
        }

        
        foreach(KeyValuePair<string, List<DinoActionMap>> pair in serializedActionListDict)
        {
            using (new BoxScope()) {

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0} : ", pair.Key));
                DrawAnimationLoopToggle(actionType, pair.Key);
                EditorGUILayout.EndHorizontal();
                List<DinoActionMap> collectedMaps = pair.Value;

                for(int i = 0; i < collectedMaps.Count; i++)
                {
                    DrawEditActionMapSlot(collectedMaps[i]);
                    DrawLine();
                }
            }
        }
    }
    
    private void DrawAnimationLoopToggle(DinoActionType actionType, string animationName)
    {


        DinoActionAnimationLoopSetting setting = targetDinosaurObject.ContainsDinoActionAnimationLoopSetting(actionType, animationName);
        GUILayout.Label("- loop : ");
        // bool loopEnable = EditorGUILayout.Toggle("-loop : ", setting != null);
        bool loopEnable = EditorGUILayout.Toggle(setting != null);

        //
        if(loopEnable)
        {
            if(setting == null)
            {
                setting = new DinoActionAnimationLoopSetting(actionType, animationName, m_animationMap.animationLengthDict[animationName]); 
                targetDinosaurObject.AddDinoActionAnimationLoopSetting(setting);
                setting.playtime = EditorGUILayout.FloatField("play time : ", setting.playtime);
                // EditorGUILayout.PropertyField(setting.playtime);
                // Debug.Log("setting is null");
            }
            else
            {
                setting.playtime = EditorGUILayout.FloatField("play time : ", setting.playtime);
                // Debug.Log("setting is not null");
            }
            // EditorGUILayout.PropertyField()
        }
        //
        else
        {
            //제거
            if(setting != null)
            {
                Debug.Log("remove setting");
                targetDinosaurObject.RemoveDinoActionAnimationLoopSetting(actionType, animationName);
            }
        }

        EditorGUILayout.Space();
    }

    private void DrawEditActionMapSlot(DinoActionMap map)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("event : ");
        string noneValue = "none";
        List<string> events = new List<string>();
        if(map.animationName.Equals(noneValue))
        {
            events.Add(noneValue);
        }
        else
        {
            events.Add("start");
            events.AddRange(m_animationMap.eventListDict[map.animationName].eventList);
            events.Add("end");
        }

        int eventSelectedIndex = 0;
        if(events.Contains(map.eventName))
        {
            eventSelectedIndex = events.IndexOf(map.eventName);
        }

        eventSelectedIndex = EditorGUILayout.Popup(eventSelectedIndex, events.ToArray());

        map.eventName = events[eventSelectedIndex];

        EditorGUILayout.Space();
        GUILayout.Label("bone : ");
        List<Transform> bones = new List<Transform>();
        BoneFollower[] childBones = targetDinosaurObject.transform.GetComponentsInChildren<BoneFollower>();

        bones.Add(targetDinosaurObject.transform);
        for(int i = 0; i < childBones.Length; i++)
        {
            bones.Add(childBones[i].transform);
        }

        List<string> boneNames = new List<string>();
        for(int i = 0; i < bones.Count; i++)
        {
            boneNames.Add(bones[i].name);
        }

        int boneSelectedIndex = 0;

        if(map.bone != null)
        {
            if(boneNames.Contains(map.bone.name))
            {
                boneSelectedIndex = boneNames.IndexOf(map.bone.name);
            }
        }
        else
        {
            map.bone = targetDinosaurObject.transform;
            boneSelectedIndex = boneNames.IndexOf(map.bone.name);
        }

        boneSelectedIndex = EditorGUILayout.Popup(boneSelectedIndex, boneNames.ToArray());
        map.bone = bones[boneSelectedIndex];
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        GUILayout.Label("발동 이펙트 : ");
        EffName activeSkillEffect = Utils.ConvertStringToEnumData<EffName>(map.activeEffIndex);
        
        activeSkillEffect = (EffName)EditorGUILayout.EnumPopup(activeSkillEffect);

        map.activeEffIndex = activeSkillEffect.ToString();

        EditorGUILayout.Space();
        GUILayout.Label("피격 이펙트 : ");
        EffName targetTakeEffIndex = Utils.ConvertStringToEnumData<EffName>(map.targetTakeEffIndex);
        
        targetTakeEffIndex = (EffName)EditorGUILayout.EnumPopup(targetTakeEffIndex);

        map.targetTakeEffIndex = targetTakeEffIndex.ToString();
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        map.activeEffectObject = (GameObject)EditorGUILayout.ObjectField(map.activeEffectObject, typeof(GameObject), true);
        map.targetTakeEffectObject = (GameObject)EditorGUILayout.ObjectField(map.targetTakeEffectObject, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        Color org = GUI.color;
        GUI.color = Color.red;
        if(GUILayout.Button("Remove Action Effect"))
        {
            targetDinosaurObject.DeleteActionMap(map);
        }
        GUI.color = org;
    }

    private void DrawLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color ( 0.8f,0.8f,0.8f, 1 ) );
    }

    private void PlayDinoAction(DinoActionType action)
    {
        if(!Application.isPlaying)
            return;


        targetDinosaurObject.PlayDinoAction(action);
        Debug.Log("Play Dino Action");
    }

    /// <summary>
    /// /// 텍스트 파일에 있는 공룡의 진화 정보를 가져온다.
    /// </summary>
    private void GetDinoEvolveCount()
    {
        string infoDir = "Assets/DinoPro/AssetResources/SR_Bundle/DinoEvolveCountMap.txt";
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(infoDir);

        if(textAsset == null)
            return;
        
        string prefabName = targetDinosaurObject.transform.name;
        string namePattern = "([0-9]+)";

        bool isMatch = Regex.IsMatch(prefabName, namePattern);

        string startIndex = "(";
        string endIndex = ")";


        if(isMatch == false)
        {
            EditorUtility.DisplayDialog("프리팹 이름 오류", "해당 이름으로는 공룡 진화 정보를 가져올수 없습니다. 자동으로 진화회수가 2로 맞춰집니다.", "확인");
            targetDinosaurObject.SetEvolveCount(2);
            return;
        }

        if (prefabName.Contains(startIndex) == false || prefabName.Contains(endIndex) == false)
        {
            EditorUtility.DisplayDialog("프리팹 이름 오류", "해당 이름으로는 공룡 진화 정보를 가져올수 없습니다. 자동으로 진화회수가 2로 맞춰집니다.", "확인");
            targetDinosaurObject.SetEvolveCount(2);
            return;
        }
        int start = prefabName.IndexOf(startIndex) + 1;
        int end = prefabName.IndexOf(endIndex) - start;
        string index = prefabName.Substring(start, end);

        using(StringReader sr = new StringReader(textAsset.text))
		{
			string readLine;

            bool autoSetted = false;
			while((readLine = sr.ReadLine()) != null)
			{
				if(!readLine.StartsWith("#") && !readLine.StartsWith("\t") && !readLine.StartsWith("\n"))
				{
					string[] word = readLine.Split('\t');
					string key = word[0].Trim();
					int evolveCount = int.Parse(word[1]);
                    if(index.Equals(key))
                    {
                        targetDinosaurObject.SetEvolveCount(evolveCount);
                        autoSetted = true;
                    }
				}
			}

            if(autoSetted == false)
            {
                EditorUtility.DisplayDialog("공룡 인덱스를 찾을수 없음", "해당 공룡의 진화 회수가 2로 맞춰집니다. 기획자 및 개발자에게 문의", "확인");
                targetDinosaurObject.SetEvolveCount(2);
            }

            Debug.Log("Setting Skeleton Render Component");
            
		}
    }

    /// <summary>
    /// 
    /// </summary>
    public void ReplacePrefab()
    {
        UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(targetDinosaurObject.gameObject);
        PrefabUtility.ReplacePrefab(targetDinosaurObject.gameObject, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
        targetDinosaurObject.ApplyChangeAnimationSettingValue();
    }
    

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    public class BoxScope : System.IDisposable 
    {
        readonly bool indent;

        static GUIStyle boxScopeStyle;
        public static GUIStyle BoxScopeStyle {
            get {
                if (boxScopeStyle == null) {
                    boxScopeStyle = new GUIStyle(EditorStyles.helpBox);
                    var p = boxScopeStyle.padding;
                    p.right += 6;
                    p.top += 1;
                    p.left += 3;
                }

                return boxScopeStyle;
            }
        }

        public BoxScope (bool indent = true) {
            this.indent = indent;
            EditorGUILayout.BeginVertical(BoxScopeStyle);
            if (indent) EditorGUI.indentLevel++;
        }
            
        public void Dispose () {
            if (indent) EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }



    private void SetAnimationData(SkeletonData data)
    {
        animationData = data;

        if(animationData != null)
        {
            Spine.Animation[] animations = animationData.Animations.Items;

            m_animationMap = new DinoAnimationMap(animations);
            // animationMap = new DinoAnimationMap(animations);
            // for(int i = 0; i < animations.Length; i++)
            // {
            //     Debug.Log("**************** " + animations[i].Name);
            //     // animations[i].Timelines.Items

            //     CheckSpineAnimation(animations[i]);
            // }
        }
    }

    private void DrawAnimationEventList()
    {
        EditorGUILayout.BeginVertical();
        // for(int i = 0; i < m_animationMap.dinoAnimations.Count; i++)
        // {
            // if (EditorGUILayout.Foldout(m_animationMap.dinoAnimations[i].animationName,))
            // GUILayout.Label(m_animationMap.dinoAnimations[i].animationName);
            // for(int ii = 0; ii < m_animationMap.dinoAnimations[i].eventList.Count; ii++)
            // {

                // GUILayout.Label("\t" + m_animationMap.dinoAnimations[i].eventList[ii]);
            // }
        // }
        EditorGUILayout.EndVertical();
    }


    private void CheckSpineAnimationEvent(Spine.Event[] animEvents)
    {
        if(animEvents == null)
            return;

        for(int i = 0; i < animEvents.Length; i++)
        {
            Debug.Log(animEvents[i].ToString());
        }
    }
}
