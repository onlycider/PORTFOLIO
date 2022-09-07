using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BulletTargetDinoEffectController))]
[CanEditMultipleObjects]
public class BulletTargetDinoEffectControllerInspector : DinoEffectControllerInspector
{
    SerializedProperty selectedBone, duration;
    protected override void OnEnable()
    {
        base.OnEnable();
        selectedBone = serializedObject.FindProperty("selectedBone");
        duration = serializedObject.FindProperty("duration");
    }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(selectedBone);
        bool changedBone = EditorGUI.EndChangeCheck();
        if(changedBone)
            serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(duration);
        bool changedDuration = EditorGUI.EndChangeCheck();
        if(changedDuration)
            serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Target's Event Dictionary"))
        {
            var animation = skeletonAnimation.skeletonDataAsset.GetSkeletonData(true).FindAnimation(curAnimationName);
            List<float> durationList = new List<float>();
            if (animation != null)
            {
                foreach (var timeLine in animation.Timelines)
                {
                    var eventTimeLine = timeLine as Spine.EventTimeline;
                    if (eventTimeLine != null)
                    {
                        foreach (var spineEvent in eventTimeLine.Events)
                        {
                            durationList.Add(spineEvent.Time);
                            Debug.LogFormat("Event : {0}, Duration : {1}", spineEvent.Data.Name, spineEvent.Time);
                        }
                    }
                }
                if (durationList.Count == 0)
                    Debug.LogFormat("List is Empty");
                else
                    Debug.LogFormat("Bullet Duration : {0}", Mathf.Abs(durationList[durationList.Count - 1] - durationList[0]));
            }
        }

        // EditorGUI.BeginChangeCheck();
        // EditorGUILayout.PropertyField(followTargetBone);
        // bool changedFollowBone = EditorGUI.EndChangeCheck();
        // if(changedFollowBone)
        //     serializedObject.ApplyModifiedProperties();
        
    }
}
