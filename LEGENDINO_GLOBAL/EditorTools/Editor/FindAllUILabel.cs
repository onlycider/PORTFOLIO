using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/* 
    목표 : UI Root 밑에 있는 모든 오브젝트들을 탐색하여, UILabel 가지고 있는 오브젝트 전체경로 Text로 추출하기
    이유 : 전체경로를 추출하여, 편하게 Label 정보를 변경하기 위해서

    고정형 : Library Index 작성
    반응형 : 해당 라벨이 변동되고 있는 스크립트에 접근해서 Key값 넣고 수정
*/
public class FindAllUILabel : EditorWindow
{

    [MenuItem("Dev/Find Scene All UILabel")]
    public static void OnAllUILabel ()
    {
        UIRoot uIRoot = GameObject.Find("UI Root").GetComponent<UIRoot>();
        UILabel[] temp = uIRoot.GetComponentsInChildren<UILabel>(true); // UI Root 아래 UILabel 가지고있는 모든 Obj 담기
        List<GameObject> tempList = new List<GameObject>();
        List<string> pathList = new List<string>();

        for (int i = 0; i < temp.Length; i++)
        {
            tempList.Add(temp[i].gameObject);
        }


        for (int i = 0; i < tempList.Count; i++) 
        {
            string path = "/" + tempList[i].name;

            while (tempList[i].transform.parent != uIRoot.transform) // uiRoot 전까지 탐색
            {
                tempList[i] = tempList[i].transform.parent.gameObject;
                path = "/" + tempList[i].name + path;
            }
            pathList.Add(path);
        }

        string strPath = "Assets/DinoPro/Resources/LabelText";
        StreamWriter streamWriter = new StreamWriter(strPath);

        for (int i = 0; i < pathList.Count; i++)
        {
            char sp = '/';
            string prefabName = pathList[i].Split(sp)[1]; // 프리펩 명 담기

            streamWriter.WriteLine(pathList[i]);

            if (i < pathList.Count - 1)
            {
                string nextPrefabName = pathList[i + 1].Split(sp)[1]; // Next 프리펩 명 담기
                if (prefabName != nextPrefabName) // 현재 프리펩 명과 다음 프리펩 명이 다르면, "\n" 하기
                {
                    streamWriter.WriteLine("");
                }
            }
        }
        streamWriter.Flush();
        streamWriter.Close();
    }

    [MenuItem("Dev/Find Scene All UILabel Text")]
    public static void OnAllUILabelText()
    {
        UIRoot uIRoot = GameObject.Find("UI Root").GetComponent<UIRoot>();
        UILabel[] temp = uIRoot.GetComponentsInChildren<UILabel>(true); // UI Root 아래 UILabel 가지고있는 모든 Obj 담기
        List<GameObject> tempList = new List<GameObject>();
        List<string> pathList = new List<string>();

        for (int i = 0; i < temp.Length; i++)
        {
            tempList.Add(temp[i].gameObject);
        }


        for (int i = 0; i < tempList.Count; i++)
        {

            UILabel tempLabel = tempList[i].GetComponent<UILabel>();
            pathList.Add(tempLabel.text);
        }

        string strPath = "Assets/DinoPro/Resources/LabelText2";
        StreamWriter streamWriter = new StreamWriter(strPath);

        for (int i = 0; i < pathList.Count; i++)
        {
            streamWriter.WriteLine(pathList[i]);
        }
        streamWriter.Flush();
        streamWriter.Close();
    }
}
