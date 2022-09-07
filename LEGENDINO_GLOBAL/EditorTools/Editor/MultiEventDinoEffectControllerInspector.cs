using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine;
using Spine.Unity;
using Spine.Unity.Editor;

[CustomEditor(typeof(MultiEventDinoEffectController))]
[CanEditMultipleObjects]
public class MultiEventDinoEffectControllerInspector : DinoEffectControllerInspector
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
