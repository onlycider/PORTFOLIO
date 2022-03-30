using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public class SelectPopup : Panel
    {
        public GameObject selectableTextObject;

        public Transform listParent;
        private List<string> m_textKeys;

        private string m_selectedText;

        private Action<string> selected_text_action;

        public override void SetPanelInfo(object info)
        {
            base.SetPanelInfo(info);
            m_textKeys = info as List<string>;
            DrawSelectableTexts();
        }

        private void DrawSelectableTexts()
        {
            foreach(string key in m_textKeys)
            {
                GameObject obj = Instantiate<GameObject>(selectableTextObject, listParent);
                SelectableText selectableText = obj.GetComponent<SelectableText>();
                selectableText.SetText(key);
                selectableText.SetParentPanel(this);
            }
        }

        public void SetSelectedTextAction(Action<string> action)
        {
            selected_text_action = action;
        }

        public void SetSelectedText(string text)
        {
            m_selectedText = text;
            // Debug.Log(m_selectedText);
            Utils.InvokeAction(selected_text_action, m_selectedText);
            OnClickCloseButton();
        }

        public void OnClickCloseButton()
        {
            PanelManager.Instance.RemovePanel(this);
        }
    }
}