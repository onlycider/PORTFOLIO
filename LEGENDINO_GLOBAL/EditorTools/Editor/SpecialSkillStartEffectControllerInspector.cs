using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpecialSkillStartEffectController))]
[CanEditMultipleObjects]
public class SpecialSkillStartEffectControllerInspector : Editor
{
    SerializedProperty effectDepthType;
    void OnEnable () {
        effectDepthType = serializedObject.FindProperty("effectDepthType");
    }

    override public void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(effectDepthType);
        bool changedDepth = EditorGUI.EndChangeCheck();
        if(changedDepth)
            serializedObject.ApplyModifiedProperties();
    }
}
