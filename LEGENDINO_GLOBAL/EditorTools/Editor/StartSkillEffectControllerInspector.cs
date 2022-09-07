using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StartSkillEffectController))]
[CanEditMultipleObjects]
public class StartSkillEffectControllerInspector : Editor
{
    SerializedProperty effectDepthType, skillTypeIndex;

    private string[] skillTypes = {"default", "fire", "water", "earth", "light", "dark"};
    void OnEnable () {
        effectDepthType = serializedObject.FindProperty("effectDepthType");
        skillTypeIndex = serializedObject.FindProperty("skillTypeIndex");
    }

    override public void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(effectDepthType);
        bool changedDepth = EditorGUI.EndChangeCheck();
        if(changedDepth)
            serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginChangeCheck();
        int typeIndex = skillTypeIndex.intValue;
        skillTypeIndex.intValue = EditorGUILayout.Popup("Skill Type ", typeIndex, skillTypes);
        bool changedType = EditorGUI.EndChangeCheck();
        if(changedType)
            serializedObject.ApplyModifiedProperties();
    }
}
