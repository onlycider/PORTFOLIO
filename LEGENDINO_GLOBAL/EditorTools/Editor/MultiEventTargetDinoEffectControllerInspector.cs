using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiEventTargetDinoEffectController))]
[CanEditMultipleObjects]
public class MultiEventTargetDinoEffectControllerInspector : TargetDinoEffectControllerInspector
{
    SerializedProperty count;
   
    protected override void OnEnable()
    {
        base.OnEnable();
        count = serializedObject.FindProperty("count");
    }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(count);
        bool changedCount = EditorGUI.EndChangeCheck();
        if(changedCount)
            serializedObject.ApplyModifiedProperties();
    }
}
