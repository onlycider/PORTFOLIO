using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MunchProject
{
    public class DragArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform mRectTransform;

        public Action<PointerEventData> ACTION_BEGIN_DRAG;
        public Action<PointerEventData> ACTION_DRAG;
        public Action<PointerEventData> ACTION_END_DRAG;

        public void SetDragAreaSize(float width, float height)
        {
            if(mRectTransform == null)
            {
                mRectTransform = GetComponent<RectTransform>();
            }

            mRectTransform.sizeDelta = new Vector2(width, height);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Utils.InvokeAction(ACTION_BEGIN_DRAG, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Utils.InvokeAction(ACTION_DRAG, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Utils.InvokeAction(ACTION_END_DRAG, eventData);
        }
    }
}