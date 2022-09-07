using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FavoriteSceneTool : EditorWindow {
	public string key = "SCENES";

	private Vector2 scrollPos;

	[MenuItem("Dev/FavoriteSceneTool")]
	public static void UseFavoriteSceneTool()
	{
		EditorWindow.GetWindow(typeof(FavoriteSceneTool));
	}

	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI()
	{
		Color orgColor = GUI.color;

		EditorGUILayout.Space();
		// EditorGUIUtility.LookLikeControls();

		EditorGUILayout.BeginVertical();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		DrawSceneNames();
		EditorGUILayout.EndScrollView();

		GUI.color = Color.green;
		
		EditorGUILayout.Space();
		if(GUILayout.Button("Add Current Scene"))
		{
			AddCurrentScene();
		}

		if(GUILayout.Button("Save Project"))
		{
			AssetDatabase.SaveAssets();
		}

		GUI.color = orgColor;
		EditorGUILayout.EndVertical();
		
	}

	void OnHierarchyChange()
	{
		Repaint();
	}

	private void DrawSceneNames()
	{
		string[] scenes = GetSceneNames();
		Color orgColor = GUI.color;

		foreach(string name in scenes)
		{
			EditorGUILayout.BeginHorizontal();

			if(name.Equals(EditorSceneManager.GetActiveScene().path))
			{
				GUI.color = Color.green;
				EditorGUILayout.TextField(name);
				GUI.color = orgColor;
			}
			else
			{
				EditorGUILayout.TextField(name);
			}

			if(GUILayout.Button("Open"))
			{
				EditorSceneManager.OpenScene(name);
			}

			if(GUILayout.Button("X"))
			{
				DeleteScene(name);
			}

			EditorGUILayout.EndHorizontal();
		}
	}

	public string[] GetSceneNames()
	{
		string value = "";

		if(EditorPrefs.HasKey(key))
		{
			value = EditorPrefs.GetString(key);
		}
		
		string[] scenes = value.Split(',');
		return scenes;
	}

	private void AddCurrentScene()
	{
		string currentSceneName = EditorSceneManager.GetActiveScene().path;
		string[] scenes = GetSceneNames();

		List<string> newScenes = new List<string>();

		bool isExist = false;
		
		foreach(string name in scenes)
		{
			if(name.Equals(currentSceneName))
			{
				isExist = true;
				continue;
			}

			if(name.Equals("") || name == null)
			{
				continue;
			}

			newScenes.Add(name);
		}

		if(isExist)
			return;
		
		newScenes.Add(currentSceneName);
		newScenes.Sort();

		int i = 0;

		string value = "";

		foreach(string name in newScenes)
		{
			if(i != 0)
			{
				value +=",";
			}

			i++;

			value += name;
		}

		EditorPrefs.SetString(key, value);
	}


	private void DeleteScene(string deletedName)
	{
		string[] scenes = GetSceneNames();
		
		List<string> newScenes = new List<string>();

		// bool isExist = false;
		
		foreach(string name in scenes)
		{
			if(name.Equals(deletedName))
			{
				continue;
			}

			if(name.Equals("") || name == null)
			{
				continue;
			}

			newScenes.Add(name);
		}

		int i = 0;

		string value = "";

		foreach(string name in newScenes)
		{
			if(i != 0)
			{
				value += ",";
			}

			i++;

			value += name;
		}

		EditorPrefs.SetString(key, value);
	}
}
