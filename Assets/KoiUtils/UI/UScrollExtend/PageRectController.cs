using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Koi.UI
{

    public class PageRectController : MonoBehaviour
    {
        [Serializable]
        public class PageInfo
        {
            public int pageId;
            public List<ScrollRect> listScrollRect = new List<ScrollRect>();
        }

        [SerializeField] PageRect m_Page;
        [SerializeField] ScrollRectController m_ScrollController;
        [SerializeField] ScrollRectEditorSupport m_EditorSupport;
        [SerializeField] bool isHorizontalPage = true;
        [SerializeField] List<PageInfo> listPage = new List<PageInfo>();

        void Awake()
        {
            if (m_Page == null)
            {
                m_Page = GetComponent<PageRect>();
            }
            if (m_ScrollController == null)
            {
                m_ScrollController = GetComponent<ScrollRectController>();
            }
            if (m_EditorSupport == null)
            {
                m_EditorSupport = GetComponent<ScrollRectEditorSupport>();
            }
            m_Page.isHorizontalPage = isHorizontalPage;
        }

        void Start()
        {
            InitListener();
        }

        public void SetupSize(Vector2 pViewSize, bool resetContentPos = false)
        {
            int curId = m_ScrollController.CurrentId;
            GetComponent<RectTransform>().sizeDelta = pViewSize;
            m_ScrollController.SetupCellSize(pViewSize);
            m_EditorSupport.SetupFitchChildSize(pViewSize);
            m_EditorSupport.SetupAll(resetContentPos);
        }

        void InitListener()
        {
            m_Page.AddBeginDragListener(OnBeginDrag);
            m_Page.AddDraggingListener(OnDragging);
            m_Page.AddEndDragListener(OnEndDrag);
            m_Page.AddInitializePotentialDragListener(OnInitializePotentialDrag);
            m_Page.AddScrollListener(OnScroll);
        }

        PointerEventData RebuildEventData(PointerEventData pEventData)
        {
            var copyData = pEventData;
            var delta = copyData.delta;
            if (isHorizontalPage)
            {
                delta.x = 0;
            }
            else
            {
                delta.y = 0;
            }
            copyData.delta = delta;
            return copyData;
        }

        void OnBeginDrag(PointerEventData pEventData)
        {
            if (m_Page.isScrollInsidePage)
            {
                bool haveScroll = false;

                var savedDelta = pEventData.delta;
                var onPageEventData = RebuildEventData(pEventData);
                foreach (ScrollRect scroll in CurListScrollChild())
                {
                    haveScroll = true;
                    scroll.OnBeginDrag(onPageEventData);
                }
                pEventData.delta = savedDelta;

                if (!haveScroll)
                {
                    m_Page.OnDragOnNoneScrollPage(pEventData);
                }
            }
        }

        void OnDragging(PointerEventData pEventData)
        {
            if (m_Page.isScrollInsidePage)
            {
                var savedDelta = pEventData.delta;

                pEventData = RebuildEventData(pEventData);
                foreach (ScrollRect scroll in CurListScrollChild())
                {
                    scroll.OnDrag(pEventData);
                }

                pEventData.delta = savedDelta;
            }
        }

        void OnEndDrag(PointerEventData pEventData)
        {

            if (m_Page.isScrollInsidePage)
            {
                var savedDelta = pEventData.delta;

                pEventData = RebuildEventData(pEventData);
                foreach (ScrollRect scroll in CurListScrollChild())
                {
                    scroll.OnEndDrag(pEventData);
                }
                pEventData.delta = savedDelta;
            }
        }

        void OnInitializePotentialDrag(PointerEventData pEventData)
        {
            if (m_Page.isScrollInsidePage)
            {
                var savedDelta = pEventData.delta;

                pEventData = RebuildEventData(pEventData);
                foreach (ScrollRect scroll in CurListScrollChild())
                {
                    scroll.OnInitializePotentialDrag(pEventData);
                }
                pEventData.delta = savedDelta;
            }
        }

        void OnScroll(PointerEventData pEventData)
        {
            if (m_Page.isScrollInsidePage)
            {
                var savedDelta = pEventData.delta;

                pEventData = RebuildEventData(pEventData);
                foreach (ScrollRect scroll in CurListScrollChild())
                {
                    scroll.OnScroll(pEventData);
                }
                pEventData.delta = savedDelta;
            }
        }

        List<ScrollRect> CurListScrollChild()
        {
            int curId = m_ScrollController.CurrentId;
            foreach (PageInfo page in listPage)
            {
                if (curId == page.pageId)
                {
                    return page.listScrollRect;
                }
            }

            return new List<ScrollRect>();
        }
    }
}
