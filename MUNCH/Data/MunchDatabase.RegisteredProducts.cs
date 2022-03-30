using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        public const string PRODUCT_DATA_DIR = "RegisteredProducts";
        public string UserDataDir = $"{Application.persistentDataPath}/{PRODUCT_DATA_DIR}/";

        public void RegisterProductData(Dictionary<string, object> data)
        {
            string directoryName = (string)data["id"];
            string registerProductData = JsonFx.Json.JsonWriter.Serialize(data);

            CheckRegisteredProductsDirectory();
            string dir = $"{Application.persistentDataPath}/{PRODUCT_DATA_DIR}/{directoryName}/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            SaveProductRegisteredJsonData(dir, registerProductData);
            SaveProductImage(dir);
            SaveVisionApiResult(dir);
        }

        public void SaveProductRegisteredJsonData(string directoryName, string json)
        {
            string filePath = $"{directoryName}product.txt";
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            writer.Write(json);
            writer.Close();
        }

        public void CheckRegisteredProductsDirectory()
        {
            string baseDir = $"{Application.persistentDataPath}/{PRODUCT_DATA_DIR}/";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
        }

        public string[] GetRegisteredProductInfos()
        {
            CheckRegisteredProductsDirectory();

            string baseDir = $"{Application.persistentDataPath}/{PRODUCT_DATA_DIR}/";
            return Directory.GetDirectories(baseDir);
        }

        public string[] GetResourcesProductInfos()
        {
            string baseDir = $"{Application.dataPath}/Resources/";
            return Directory.GetDirectories(baseDir);
        }

        public int GetDirectoryIndex(string directoryName)
        {
            string[] dirs = GetRegisteredProductInfos();

            if (dirs.Length <= 0)
            {
                return -1;
            }

            string dir = $"{Application.persistentDataPath}/{PRODUCT_DATA_DIR}/{directoryName}";
            for (int i = 0; i < dirs.Length; ++i)
            {
                if (dir == dirs[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public Dictionary<string, object> LoadRegisteredJsonData(string directoryName)
        {
            string textAsset = File.ReadAllText($"{directoryName}/product.txt");
            return JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(textAsset);
        }
    }
}