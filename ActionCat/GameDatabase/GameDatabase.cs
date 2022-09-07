using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 데이터 모음
/// 추후 GameData와 연결될 수 있도록 함
/// </summary>
public partial class GameDatabase
{
    public static GameDatabase Instance{
        get
        {
            if(m_instance == null)
                Initialize();
            
            return m_instance;
        }
    }
    private static GameDatabase m_instance;

    private string m_uuid;
    public string uuid{get{return m_uuid;}}

    private AssetBundle m_gameDatapack;
    public AssetBundle gameDatapack{get{return m_gameDatapack;}}
    public GameDatabase()
    {
        m_uuid = PlayerPrefsManager.GetString(PlayerPrefsKey.DEVICE_ID, SystemInfo.deviceUniqueIdentifier);
    }

    public static void Initialize()
    {
        m_instance = new GameDatabase();
    }

    public void SetGameDatabase(AssetBundle _datapack)
    {
        m_gameDatapack = _datapack;

        // foreach(string assetName in m_gameDatapack.GetAllAssetNames())
        // {
        //     Debug.Log(assetName);
        // }
        // m_gameDatapack.LoadAllAssets();
    }

    private Dictionary<string, object>[] LoadJsonData(string _json)
    {
        // Debug.Log(_json);
        Dictionary<string, object>[] datas = null;
#if UNITY_EDITOR
        TextAsset jsonFile = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format("Assets/JsonData/{0}.json", _json));
        datas = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFile.text);
#else
        TextAsset jsonFile = m_gameDatapack.LoadAsset<TextAsset>(string.Format("assets/jsondata/{0}.json" , _json.ToLower()));
        datas = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>[]>(jsonFile.text);
#endif
        // m_gameDatapack.LoadAsset<TextAsset>();
        return datas;
    }
}
