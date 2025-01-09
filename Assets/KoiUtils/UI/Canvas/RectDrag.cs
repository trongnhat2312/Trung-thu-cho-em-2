using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Koi.UI
{
    public class RectDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool dragOnSurfaces = true;

        private RectTransform m_DraggingPlane;

        public bool isDragging = false;

        private Action<int, Vector2> OnBeginDragListener;
        private Action<int, Vector2> OnPointerDownListener;

        private Action<int, Vector2> OnDraggingListener;

        private Action<int> OnFinishDragListener;
        private Action<int> OnPointerUpListener;
        private Action<int> OnPointerExitListener;

        List<int> listDragingId;


        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnPointerDownListener != null)
            {
                OnPointerDownListener(eventData.pointerId, eventData.position);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (OnBeginDragListener != null)
            {
                OnBeginDragListener(eventData.pointerId, eventData.position);
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (OnDraggingListener != null)
            {
                OnDraggingListener(data.pointerId, data.position);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (OnFinishDragListener != null)
            {
                OnFinishDragListener(eventData.pointerId);
            }
        }



        public void OnPointerUp(PointerEventData eventData)
        {
            if (OnPointerUpListener != null)
            {
                OnPointerUpListener(eventData.pointerId);
            }
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExitListener != null)
            {
                OnPointerExitListener(eventData.pointerId);
            }
        }















        public void AddPointerDownListener(Action<int, Vector2> listener)
        {
            OnPointerDownListener -= listener;
            OnPointerDownListener += listener;
        }


        public void AddBeginDragListener(Action<int, Vector2> listener)
        {
            OnBeginDragListener -= listener;
            OnBeginDragListener += listener;
        }

        public void AddDraggingListener(Action<int, Vector2> listener)
        {
            OnDraggingListener -= listener;
            OnDraggingListener += listener;
        }

        public void AddFinishDragListener(Action<int> listener)
        {
            OnFinishDragListener -= listener;
            OnFinishDragListener += listener;
        }


        public void AddPointerUpListener(Action<int> listener)
        {
            OnPointerUpListener -= listener;
            OnPointerUpListener += listener;
        }

        public void AddPointerExitListener(Action<int> listener)
        {
            OnPointerExitListener -= listener;
            OnPointerExitListener += listener;
        }
    }
}

