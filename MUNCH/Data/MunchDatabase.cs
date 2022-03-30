using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public partial class MunchDatabase
    {
        private static MunchDatabase m_instance;
        public static MunchDatabase instance
        {
            get
            {
                if (m_instance == null)
                {
                    Initialize();
                }

                return m_instance;
            }
        }

        private bool m_loadComplete = false;

        public static void Initialize()
        {
            if (m_instance != null)
            {
                return;
            }

            m_instance = new MunchDatabase();
        }

        private Dictionary<string, object>[] LoadJsonData(string _json)
        {
            // Debug.Log(_json);
            Dictionary<string, object>[] datas = null;
#if UNITY_EDITOR
            TextAsset jsonFile = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format("Assets/JsonData/{0}.json", _json));
            datas = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFile.text);
#else
            TextAsset jsonFile = Resources.Load<TextAsset>(string.Format("SampleData/{0}" , _json.ToLower()));
            datas = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFile.text);
#endif
            // m_gameDatapack.LoadAsset<TextAsset>();
            return datas;
        }

        /// <summary>
        /// 싱글턴 메타 데이터 풀 로드
        /// </summary>
        public void LoadMunchData()
        {
            if (m_loadComplete)
            {
                return;
            }
            m_productDictionary = new ProductDictionary(LoadJsonData("sample_meta_data"));


            m_loadComplete = true;
        }
    }
}