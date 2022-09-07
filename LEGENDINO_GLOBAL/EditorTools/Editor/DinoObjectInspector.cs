using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine;
using Spine.Unity;
using Spine.Unity.Editor;

[CustomEditor(typeof(DinoObject)), CanEditMultipleObjects]
public class DinoObjectInspector : Editor
{
    SerializedProperty effectScale;

    private void OnEnable() 
    {
        effectScale = serializedObject.FindProperty("effectScale");
    }


    override public void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(effectScale);

        bool changeValue = EditorGUI.EndChangeCheck();
        if(changeValue)
            serializedObject.ApplyModifiedProperties();
    }
}
