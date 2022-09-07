using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;



public class JsonWriter : EditorWindow {
    [MenuItem("Dev/Json Form Writer")]
    public static void UseFormWriter()
    {
        EditorWindow.GetWindow(typeof(JsonWriter));
    }


    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        if (GUILayout.Button("pass user list form create"))
        {
            CreatePassUserListForm();
        }
        EditorGUILayout.EndVertical();

    }

    private void CreatePassUserListForm()
    {
        string outputPath = Path.Combine("AssetBundles", "MaintenancePassUser");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        MaintenancePassUserList maintenancePassUserList = new MaintenancePassUserList();

        maintenancePassUserList.pass_users = new string[]{"0000000001", "0000000002", "0000000003"};
        string listJson = JsonUtility.ToJson(maintenancePassUserList);
        string filePath = outputPath + "/pass_users.json";
        File.WriteAllText(filePath, listJson);

        Debug.Log(listJson);

    }
}
