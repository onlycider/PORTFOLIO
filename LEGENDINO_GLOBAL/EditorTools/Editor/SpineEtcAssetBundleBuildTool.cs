using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class SpineEtcAssetBundleBuildTool : EditorWindow
{
    private string versionCode;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("[etc Bundle]");
        GUILayout.Label("version : ");
        versionCode = EditorGUILayout.TextField(versionCode);
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("update assetbundle info"))
        {
            
        }

        if (GUILayout.Button("make user character asset"))
        {
            CreateUserCharacterAsset();
        }

        if (GUILayout.Button("make npc character asset"))
        {
            CreateNpcAsset();
        }

        if (GUILayout.Button("make emoticon asset"))
        {
            CreateEmoticonAssets();
        }
    }

    private void CreateUserCharacterAsset()
    {
        if (string.IsNullOrEmpty(versionCode))
        {
            Debug.LogError("Please input version code");
            return;
        }

        int version = -1;
        bool parseEnable = int.TryParse(versionCode, out version);

        if (parseEnable == false)
        {
            Debug.LogError("Version Code Parse Error !!!!!!");
            return;
        }

        if (version < 0)
        {
            Debug.LogError("Version Code Can't write negative value !!!!!!");
            return;
        }

        string versionFolder = string.Format("ver{0}", version);

        string outputPath = Path.Combine("AssetBundles/android/user_character", versionFolder);

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string prefabDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/Cha";
        string spineDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/Spine/Main_0";

        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
        assetBundleBuild.assetBundleName = string.Format("user_character.ver{0}", version);
        string[] assets = {prefabDirPath, spineDirPath};
        assetBundleBuild.assetNames = assets;
        //map.assetNames
        AssetBundleBuild[] map = { assetBundleBuild };
        BuildPipeline.BuildAssetBundles(outputPath, map, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    private void CreateNpcAsset()
    {
        if (string.IsNullOrEmpty(versionCode))
        {
            Debug.LogError("Please input version code");
            return;
        }

        int version = -1;
        bool parseEnable = int.TryParse(versionCode, out version);

        if (parseEnable == false)
        {
            Debug.LogError("Version Code Parse Error !!!!!!");
            return;
        }

        if (version < 0)
        {
            Debug.LogError("Version Code Can't write negative value !!!!!!");
            return;
        }

        string versionFolder = string.Format("ver{0}", version);

        string outputPath = Path.Combine("AssetBundles/android/npc_character", versionFolder);

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string prefabDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/NPC";
        string spineDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/Spine/NPC_0";

        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
        assetBundleBuild.assetBundleName = string.Format("npc_character.ver{0}", version);
        string[] assets = { prefabDirPath, spineDirPath };
        assetBundleBuild.assetNames = assets;
        //map.assetNames
        AssetBundleBuild[] map = { assetBundleBuild };
        BuildPipeline.BuildAssetBundles(outputPath, map, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    private void CreateEmoticonAssets()
    {
        if (string.IsNullOrEmpty(versionCode))
        {
            Debug.LogError("Please input version code");
            return;
        }

        int version = -1;
        bool parseEnable = int.TryParse(versionCode, out version);

        if (parseEnable == false)
        {
            Debug.LogError("Version Code Parse Error !!!!!!");
            return;
        }

        if (version < 0)
        {
            Debug.LogError("Version Code Can't write negative value !!!!!!");
            return;
        }

        string versionFolder = string.Format("ver{0}", version);
        string outputPath = Path.Combine("AssetBundles/android/emoticon", versionFolder);


        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string prefabDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/Emoticon";
        string spineDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/Spine/Emoticon_{0}";

        string startIndex = "(";
        string endIndex = ")";

        string[] prefabFiles = Directory.GetFiles(prefabDirPath, "*.prefab");
        string[] spineDirs = new string[prefabFiles.Length];

        List<string> assets = new List<string>();

        for (int i = 0; i < prefabFiles.Length; i++)
        {
            int start = prefabFiles[i].IndexOf(startIndex) + 1;
            int end = prefabFiles[i].IndexOf(endIndex) - start;
            string index = prefabFiles[i].Substring(start, end);

            assets.Add(prefabFiles[i]);
            assets.Add(string.Format(spineDirPath, index));
        }

        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
        assetBundleBuild.assetBundleName = string.Format("character_emoticon.ver{0}", version);
        assetBundleBuild.assetNames = assets.ToArray();

        AssetBundleBuild[] map = { assetBundleBuild };
        BuildPipeline.BuildAssetBundles(outputPath, map, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
