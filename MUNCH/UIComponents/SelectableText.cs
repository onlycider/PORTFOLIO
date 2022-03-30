using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MunchProject
{
    public class SelectableText : MonoBehaviour
    {
        public TextMeshProUGUI selectableText;

        private string m_labelText;

        private Panel m_parentPanel;
        public void SetText(string text)
        {
            m_labelText = text;
            selectableText.text = text;
        }

        public void SetParentPanel(Panel panel)
        {
            m_parentPanel = panel;
        }

        public void OnClickText()
        {
            SelectPopup popup = m_parentPanel as SelectPopup;
            if(popup != null)
            {
                popup.SetSelectedText(m_labelText);
            } 
            // Debug.Log($"OnClickText {m_labelText}");
        }
    }
}

