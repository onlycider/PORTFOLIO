using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class DinoShaderConverter : Editor
{
    [MenuItem("Dev/Dino Shader Change")]
    public static void ChangeDinoShader()
    {
        string directoryPath = "Assets/DinoPro/AssetResources/SR_Bundle/Dino";
        string spineDirPath = "Assets/DinoPro/AssetResources/SR_Bundle/Spine/Dino_{0}";

        string[] files = Directory.GetFiles(directoryPath, "*.prefab");
        string startIndex = "(";
        string endIndex = ")";

        for (int i = 0; i < files.Length; i++)
        {
            int start = files[i].IndexOf(startIndex) + 1;
            int end = files[i].IndexOf(endIndex) - start;
            string index = files[i].Substring(start, end);

            string matDir = string.Format(spineDirPath, index);
            string[] matFileNames = Directory.GetFiles(matDir, "*.mat");
            for (int k = 0; k < matFileNames.Length; k++)
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matFileNames[k]);
                Debug.Log(matFileNames[k]);
                Debug.Log(mat.shader);
                mat.shader = Shader.Find("Spine/Skeleton Tint");
                mat.SetColor("_Black", Color.black);
            }
        }
    }
}
