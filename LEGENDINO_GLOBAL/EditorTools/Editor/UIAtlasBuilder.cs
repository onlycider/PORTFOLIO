using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UIAtlasBuilder : MonoBehaviour {

	[MenuItem("Bundles/Build AtlasBundle")]
	public static void BuildAtlasBudnle()
	{
		string outputPath = Path.Combine("AssetBundles", "android/atlas");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

		string atlasDir = "Assets/DinoPro/2DWorld/Atlas_bundle";

		string[] prefabs = Directory.GetFiles(atlasDir, "*.prefab");
		string[] materials = Directory.GetFiles(atlasDir, "*.mat");
		string[] textures = Directory.GetFiles(atlasDir, "*.png");

		List<string> bundlePaths = new List<string>();
		bundlePaths.AddRange(prefabs);
		bundlePaths.AddRange(materials);
		bundlePaths.AddRange(textures);

		
		AssetBundleBuild buildMap = new AssetBundleBuild();
        buildMap.assetBundleName = "rd_atlas_bundle_a.unity3d";
        buildMap.assetNames = bundlePaths.ToArray();
		AssetBundleBuild[] buildMaps = {buildMap};
		BuildPipeline.BuildAssetBundles(outputPath, buildMaps, BuildAssetBundleOptions.None, BuildTarget.Android);
	}
}
