#region header
//----------------------------------------------
// ScrollRectExtend.cs
// This stupid code is created by StupidWizard on 2017/01/10.
//----------------------------------------------
using System.Collections.Generic;
using UnityEngine.EventSystems;


#endregion

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Koi.UI
{

    [RequireComponent(typeof(ScrollRectController))]
    public class ScrollRectExtend : ScrollRect
    {

        protected Action<PointerEventData> OnBeginDragListener;
        protected Action<PointerEventData> OnDraggingListener;
        protected Action<PointerEventData> OnEndDragListener;

        protected override void Start()
        {
            base.Start();
            InitOrientation();
        }

        protected virtual void InitOrientation()
        {
            if (horizontal || !vertical)
            {
                horizontal = true;
                vertical = false;
            }
        }

        public virtual void AddBeginDragListener(Action<PointerEventData> pListener)
        {
            OnBeginDragListener -= pListener;
            OnBeginDragListener += pListener;
        }

        public virtual void RemoveBeginDragListener(Action<PointerEventData> pListener)
        {
            OnBeginDragListener -= pListener;
        }

        public virtual void AddDraggingListener(Action<PointerEventData> pListener)
        {
            OnDraggingListener -= pListener;
            OnDraggingListener += pListener;
        }

        public virtual void RemoveDraggingListener(Action<PointerEventData> pListener)
        {
            OnDraggingListener -= pListener;
        }

        public virtual void AddEndDragListener(Action<PointerEventData> pListener)
        {
            OnEndDragListener -= pListener;
            OnEndDragListener += pListener;
        }

        public virtual void RemoveEndDragListener(Action<PointerEventData> pListener)
        {
            OnEndDragListener -= pListener;
        }

        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
//            Debug.LogError("on begin Drag: " + eventData.delta);
            if (OnBeginDragListener != null)
            {
                OnBeginDragListener(eventData);
            }
        }

        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (OnEndDragListener != null)
            {
                OnEndDragListener(eventData);
            }
            //		Debug.LogError("on stop Drag, velocity = " + velocity);
        }

        public override void OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnScroll(UnityEngine.EventSystems.PointerEventData data)
        {
            base.OnScroll(data);
        }
    }

}

