using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestTool : EditorWindow
{
    PlayerPrefsKey prefsKey = PlayerPrefsKey.INVALID_KEY;
    [MenuItem("Dev/Test Tool")]
    public static void OpenTestTool()
    {
        EditorWindow.GetWindow(typeof(TestTool));
    }

    string userId = string.Empty;


    string playerPrefsKey = string.Empty;
    string playerPrefsValue = string.Empty;
    void OnEnable()
    {
        
    }
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("[Player Prefs Value Check]");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Delete All PlayerPrefs"))
        {
            PlayerPrefsManager.DeleteAllPlayerPrefs();
        }

        if(GUILayout.Button("Initialize ID"))
        {
            PlayerPrefsManager.InitializeUserID();
        }

        
        GUILayout.Label("user id : ");
        userId = GUILayout.TextField(userId);

        if(GUILayout.Button("save user id"))
        {
            PlayerPrefsManager.SetUserID(userId);
        }



        if(GUILayout.Button("Clean Cache"))
        {
            if (Caching.ClearCache ()) 
            {
                Debug.Log("Successfully cleaned the cache.");
            }
            else 
            {
                Debug.Log("Cache is being used.");
            }
        }

        if(GUILayout.Button("Change Last Test UID"))
        {
            PlayerPrefs.SetString("TestLastUID", userId);
        }

        GUILayout.Label("prefs Key : ");
        playerPrefsKey = GUILayout.TextField(playerPrefsKey);
        GUILayout.Label("prefs Value : ");
        playerPrefsValue = GUILayout.TextField(playerPrefsValue);
        if(GUILayout.Button("Set PlayerPrefs string Value"))
        {
            PlayerPrefs.SetString(playerPrefsKey, playerPrefsValue);
        }
    }
}
