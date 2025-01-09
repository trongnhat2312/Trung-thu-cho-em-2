using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Koi.UI
{

    [RequireComponent(typeof(PageRectController))]
    public class PageRect : ScrollRectExtend
    {
        public enum PageState
        {
            Idle,
            ScrollInsideCurrentPage,
            ScrollPages
        }

        //        Action<PointerEventData> OnBeginDragListener;
        //        Action<PointerEventData> OnDraggingListener;
        //        Action<PointerEventData> OnEndDragListener;
        protected Action<PointerEventData> OnInitializePotentialDragListener;
        protected Action<PointerEventData> OnScrollListener;

        public bool isHorizontalPage { get; set; }

        PageState m_state = PageState.Idle;
        public PageState pageState { get { return m_state; } }

        public bool isScrollInsidePage
        {
            get
            {
                return (m_state == PageState.ScrollInsideCurrentPage);
            }
        }

        protected override void InitOrientation()
        {

        }

        public void OnDragOnNoneScrollPage(PointerEventData eventData)
        {
            m_state = PageState.ScrollPages;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            //            Debug.LogError("on begin Drag: " + eventData.delta);
            if ((Mathf.Abs(eventData.delta.x) >= Mathf.Abs(eventData.delta.y) && isHorizontalPage)
                || (Mathf.Abs(eventData.delta.x) <= Mathf.Abs(eventData.delta.y) && !isHorizontalPage))
            {
                m_state = PageState.ScrollPages;
            }
            else
            {
                m_state = PageState.ScrollInsideCurrentPage;
            }
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!isScrollInsidePage)
            {
                var beforeUpdatePos = content.anchoredPosition;
                base.OnDrag(eventData);
                var updatedPos = content.anchoredPosition;
                if (isHorizontalPage)
                {
                    updatedPos.y = beforeUpdatePos.y;
                }
                else
                {
                    updatedPos.x = beforeUpdatePos.x;
                }
                content.anchoredPosition = updatedPos;
            }
            else
            {
                if (OnDraggingListener != null)
                {
                    OnDraggingListener(eventData);
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            m_state = PageState.Idle;
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (!isScrollInsidePage)
            {
                base.OnInitializePotentialDrag(eventData);
            }
            if (OnInitializePotentialDragListener != null)
            {
                OnInitializePotentialDragListener(eventData);
            }
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (!isScrollInsidePage)
            {
                var beforeUpdatePos = content.anchoredPosition;
                base.OnScroll(eventData);
                var updatedPos = content.anchoredPosition;
                if (isHorizontalPage)
                {
                    updatedPos.y = beforeUpdatePos.y;
                }
                else
                {
                    updatedPos.x = beforeUpdatePos.x;
                }
                content.anchoredPosition = updatedPos;
            }
            if (OnScrollListener != null)
            {
                OnScrollListener(eventData);
            }
        }


        public void AddInitializePotentialDragListener(Action<PointerEventData> pListener)
        {
            OnInitializePotentialDragListener -= pListener;
            OnInitializePotentialDragListener += pListener;
        }

        public void AddScrollListener(Action<PointerEventData> pListener)
        {
            OnScrollListener -= pListener;
            OnScrollListener += pListener;
        }
    }

}

