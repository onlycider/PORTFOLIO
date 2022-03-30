using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class PanelManager : MonoBehaviour
    {
        public static PanelManager Instance = null;

        private const string DIR_PANEL = "Panels/{0}";
        private List<Panel> panelList;

        void Awake()
        {
            if (Instance != null)
            {
                if (this != Instance)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                Instance = this;
                panelList = new List<Panel>();
            }

        }

        private void OnDestroy()
        {
            Instance = null;

            if (panelList != null) panelList.Clear();
            panelList = null;
        }

        public T AddPanel<T>(PanelTag panelTag, object info = null) where T : Panel
        {
            string path = string.Format(DIR_PANEL, panelTag.ToString());

            GameObject prefab = Resources.Load(path) as GameObject;
            GameObject o = Instantiate(prefab) as GameObject;

            o.transform.SetParent(transform);
            o.transform.localScale = Vector3.one;
            o.transform.localPosition = Vector3.zero;

            Panel p = o.GetComponent<Panel>();
            p.panelTag = panelTag;

            panelList.Add(p);
            if (info != null)
                p.SetPanelInfo(info);

            o.SetActive(true);
            return p as T;

        }

        public T GetPanel<T>(PanelTag panelTag) where T : Panel
        {
            Panel panel = null;

            if (panelList.Count == 0)
                return null;

            for (int i = 0; i < panelList.Count; i++)
            {
                if (panelList[i].panelTag.Equals(panelTag))
                    panel = panelList[i];
            }

            if (panel == null)
                return null;

            return panel as T;
        }

        public void RemovePanel(Panel panelObj)
        {
            if (panelList.Count == 0)
            {
                //ERROR
                return;
            }

            Destroy(panelObj.gameObject);
            panelList.Remove(panelObj);
        }
    }


}

