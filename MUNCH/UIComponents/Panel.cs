using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MunchProject
{
    public enum PanelTag
    {
        NONE,
        VisionApiResult,
        SearchResultView,
        ProductInformationView,
        ProductRegisterView,
        SelectPopup,
        MainCameraView,
        RegisterPreviewView,
        RegisterCompleteView,
    }

    public class Panel : MonoBehaviour
    {
        private PanelTag m_panelTag;
        public PanelTag panelTag { get { return m_panelTag; } set { m_panelTag = value; } }

        private RectTransform m_rectTransform;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            // m_rectTransform.sizeDelta = Vector2.one * 20;
            m_rectTransform.sizeDelta = Vector2.zero;
            OnAwake();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            m_rectTransform.sizeDelta = Vector2.zero;
            OnStart();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            OnEnabled();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            OnDisabled();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }
        protected virtual void OnDestroied() { }

        public virtual void SetPanelInfo(object info) { }

        public virtual void InvokeBackKey() { }
    }
}