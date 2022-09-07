using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class DinoTexturesOptimizer : EditorWindow
{
    [MenuItem("Assets/Dino Textures Optimizer")]
    public static void ReimportOptimizedTexture()
    {
        UnityEngine.Object[] selectedObjects = Selection.objects;
        for(int i = 0; i < selectedObjects.Length; i++)
        {
            OptimzeTextureForAndroid(selectedObjects[i]);
        }
    }

    private static void OptimzeTextureForAndroid(UnityEngine.Object selectedObject)
    {
        // string prefabPath = AssetDatabase.GetAssetPath(selectedObject);
        GameObject prefab = selectedObject as GameObject;
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
        for(int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if(renderer.sharedMaterial == null)
                continue;

            Shader shader = renderer.sharedMaterial.shader;

            for(int k = 0; k < ShaderUtil.GetPropertyCount(shader); k++)
            {
                ShaderUtil.ShaderPropertyType textureType = ShaderUtil.GetPropertyType(shader, k);
                if(textureType == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    Texture texture = renderer.sharedMaterial.GetTexture(ShaderUtil.GetPropertyName(shader, k));
                    // if(texture == null)
                    //     continue;

                    string texturePath = AssetDatabase.GetAssetPath(texture);
                    if(string.IsNullOrEmpty(texturePath))
                        continue;

                    // Texture2D texture2d = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
                    // if(texture2d != null)
                    //     Debug.Log(texturePath + " : " + texture2d.format);
                    string  platformString = "Android";
                    int     platformMaxTextureSize = 2048;
                    TextureImporterFormat platformTextureFmt = TextureImporterFormat.ETC2_RGBA8;
                    int     platformCompressionQuality = 100;
                    bool    platformAllowsAlphaSplit = false;
                    TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(texturePath);

                    bool formating = ti.GetPlatformTextureSettings(platformString, out platformMaxTextureSize, out platformTextureFmt, out platformCompressionQuality, out platformAllowsAlphaSplit);
                    if(formating == true)
                    {
                        Debug.Log(texturePath);
                    }

                    // if(formating 
                    //     || platformMaxTextureSize > 2048 
                    //     || platformTextureFmt != TextureImporterFormat.ETC2_RGBA8 
                    //     || platformCompressionQuality < 100)
                        

                    // TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                    // settings.name = "Android";
                    // if(platformMaxTextureSize > 2048)
                    //     platformMaxTextureSize = 2048;

                    // settings.maxTextureSize = platformMaxTextureSize;
                    // settings.format = TextureImporterFormat.ETC2_RGBA8;
                    // settings.compressionQuality = 100;
                    // Debug.Log(settings.compressionQuality);
                    // settings.allowsAlphaSplitting = platformAllowsAlphaSplit;
                    // settings.overridden = true;
                    // ti.SetPlatformTextureSettings(settings);
                    // ti.SaveAndReimport();
                    // AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                    // }
                }
            }
        }
    }
}
