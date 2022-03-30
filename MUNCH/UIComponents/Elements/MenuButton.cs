using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MunchProject
{
    public class MenuButton : MonoBehaviour
    {
        public TextMeshProUGUI menuNameText;

        public Image colorMaskingImage;

        public Image buttonImage;

        private Vector3 mOriginalButtonScale;
        private Color mOriginalIconColor;

        private Color transitionColor = new Color(1f, 0.608f, 0.608f);

        public Action<int> MENU_CLICK_ACTION;

        void Awake()
        {
            SetOriginalStatusValues();
        }

        private void SetOriginalStatusValues()
        {
            mOriginalButtonScale = colorMaskingImage.rectTransform.localScale;
            mOriginalIconColor = colorMaskingImage.color;
        }

        public void SetSelectionStatus()
        {
            buttonImage.rectTransform.localScale = Vector3.one * 1.2f;
            colorMaskingImage.color = transitionColor;
            menuNameText.fontStyle = FontStyles.Bold;
        }

        public void TransitionSelectionStatus()
        {
            buttonImage.rectTransform.DOScale(Vector3.one * 1.2f, 0.4f);
            DOTween.To(() => colorMaskingImage.color, x => colorMaskingImage.color = x, transitionColor, 0.4f);
            menuNameText.fontStyle = FontStyles.Bold;
        }

        public void TransitionOriginalStatus()
        {
            DOTween.To(() => colorMaskingImage.color, x => colorMaskingImage.color = x, mOriginalIconColor, 0.4f);
            buttonImage.rectTransform.DOScale(Vector3.one, 0.4f);
            menuNameText.fontStyle = FontStyles.Normal;
        }

        public void OnClickMenuButton()
        {
            Utils.InvokeAction(MENU_CLICK_ACTION, transform.GetSiblingIndex());
        }
    }
}