using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MunchProject
{
    public class CommonConfirmPopup : Panel
    {
        public TextMeshProUGUI message;

        private Action CONFIRM_ACTION;

        public void OnClickConfirmButton()
        {
            PanelManager.Instance.RemovePanel(this);
        }
    }
}