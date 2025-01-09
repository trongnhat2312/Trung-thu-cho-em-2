#region header
//----------------------------------------------
// ScrollRectController.cs
// This stupid code is created by StupidWizard on 2017/01/10.
//----------------------------------------------
using UnityEngine.UI;
using UnityEngine.EventSystems;


#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Koi.UI
{
    public class ScrollRectController : MonoBehaviour
    {
        private const float Epsinol = 2.0f;
        private const float mRatioV = 15;

        public enum FocusState
        {
            IDLE = 0,
            WAIT_V_SLOW,
            UPDATE_FOCUS
        }

        Action<int> OnReachCell;
        Action<int> OnReachRoundCell;

        FocusState mState = FocusState.IDLE;

        ScrollRectExtend _scrollRect;
        ScrollRectExtend mScrollRect
        {
            get
            {
                if (_scrollRect == null)
                {
                    _scrollRect = GetComponent<ScrollRectExtend>();
                }
                return _scrollRect;
            }
        }

        [SerializeField] Button mBackArrow;
        [SerializeField] Button mNextArrow;

        [SerializeField] Vector2 mCellSize = new Vector2(240, 320);
        [SerializeField] float roundCellSize = 0.25f;

        [SerializeField] bool isPage = false;

        int mForceTargetId = -1;
        int mIdPress;
        int lastRoundCellId = 0;

        void Awake()
        {
            mScrollRect.AddBeginDragListener(OnBeginDrag);
            mScrollRect.AddEndDragListener(OnEndDrag);
            OnReachCell += OnMyReachCell;

            lastRoundCellId = Mathf.RoundToInt(currentFloatId);

            if (mNextArrow != null)
            {
                mNextArrow.onClick.AddListener(ScrollNext);
            }

            if (mBackArrow != null)
            {
                mBackArrow.onClick.AddListener(ScrollBack);
            }
        }

        public void SetupCellSize(Vector2 pCellSize)
        {
            this.mCellSize = pCellSize;
        }

        void OnBeginDrag(PointerEventData pEventData)
        {
            onDragEventListener(true);
        }

        void OnEndDrag(PointerEventData pEventData)
        {
            onDragEventListener(false);
        }


        public void AddReachCellListener(Action<int> pListener)
        {
            OnReachCell -= pListener;
            OnReachCell += pListener;
        }

        public void RemoveReachCellListener(Action<int> pListener)
        {
            OnReachCell -= pListener;
        }

        public void AddReachRoundCellListener(Action<int> pListener)
        {
            OnReachRoundCell -= pListener;
            OnReachRoundCell += pListener;
        }

        public void RemoveReachRoundCellListener(Action<int> pListener)
        {
            OnReachRoundCell -= pListener;
        }

        void Update()
        {
            switch (mState)
            {
                case FocusState.WAIT_V_SLOW:
                    if (mScrollRect.velocity.magnitude < mVelocityThreshold)
                    {
                        mState = FocusState.UPDATE_FOCUS;
                    }
                    break;

                case FocusState.UPDATE_FOCUS:
                    if (IsHorizontal)
                    {
                        UpdateFocusX();
                    }
                    else
                    {
                        UpdateFocusY();
                    }
                    break;

                default:	// idle - nothing to do
                    break;
            }

            UpdateRound();
        }

        void UpdateRound()
        {
            float curFloatId = currentFloatId;
            int curRound = Mathf.RoundToInt(curFloatId);
            if (curRound != lastRoundCellId && Mathf.Abs(curRound - curFloatId) < roundCellSize
                && (mForceTargetId == -1 || mForceTargetId == curRound))
            {
                lastRoundCellId = curRound;
                if (OnReachRoundCell != null)
                {
                    try
                    {
                        OnReachRoundCell(lastRoundCellId);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"ScrollRectController: OnReachRoundCell callback exception{exception.Message}");
                    }
                }
            }
        }

        void UpdateFocusX()
        {
            Vector3 contentPos = mContent.localPosition;
            float curPosX = (mContent.localPosition.x);

            int targetIdInt = CalculateNearestTargetId();
            //targetIdInt = Mathf.Min(targetIdInt, mScrollRect.content.childCount - 1);

            float targetX = -targetIdInt * mCellSize.x;
            float deltaX = targetX - curPosX;

            if (Mathf.Abs(deltaX) < mDeltaPosThreshold)
            {
                Stop(targetIdInt);
                contentPos.x = targetX;
                mContent.localPosition = contentPos;
            }
            else
            {
                mScrollRect.velocity = mRatioV * new Vector2(deltaX, 0);
                KickMove(deltaX < 0);
            }
        }

        void UpdateFocusY()
        {
            Vector3 contentPos = mContent.localPosition;
            float curPosY = (mContent.localPosition.y);

            int targetIdInt = CalculateNearestTargetId();

            float targetY = targetIdInt * mCellSize.y;
            float deltaY = targetY - curPosY;

            if (Mathf.Abs(deltaY) < mDeltaPosThreshold)
            {
                Stop(targetIdInt);
                contentPos.y = targetY;
                mContent.localPosition = contentPos;
            }
            else
            {
                mScrollRect.velocity = mRatioV * new Vector2(0, deltaY);
                KickMove(deltaY > 0);
            }
        }


        void Stop(int idStop)
        {
            mState = FocusState.IDLE;
            if (OnReachCell != null)
            {
                try
                {
                    OnReachCell(idStop);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"ScrollRectController: OnReachCell callback exception: {exception.Message}");
                }
            }
            mScrollRect.velocity = Vector2.zero;
        }

        int CalculateNearestTargetId()
        {
            int curId = CurrentId;
            if (mForceTargetId >= 0)
            {
                if (curId != mForceTargetId)
                {
                    return mForceTargetId;
                }
                else
                {
                    mForceTargetId = -1;
                }
            }
            return curId;
        }



        /// <summary>
        /// the drag event change callback.
        /// </summary>
        /// <param name="isDrag">If set to <c>true</c> is start drag else is finish drag.</param>
        public void onDragEventListener(bool isDrag)
        {
            if (isDrag)
            {
                mState = FocusState.IDLE;
                mIdPress = CalculateNearestTargetId();
            }
            else
            {
                if (isPage)
                {
                    OnFinishDragPageProcess();
                }
                else
                {
                    OnFinishDragCellProcess();
                }
            }
        }

        void OnFinishDragCellProcess()
        {
            if (mScrollRect.velocity.magnitude > mVelocityThreshold)
            {
                mState = FocusState.WAIT_V_SLOW;
            }
            else
            {
                mState = FocusState.UPDATE_FOCUS;
            }
        }

        void OnFinishDragPageProcess()
        {
            if (mScrollRect.velocity.magnitude > mVelocityThreshold * 0.5f)
            {
                mState = FocusState.IDLE;
                //				Debug.LogError("Velocity = " + mScrollRect.velocity);
                int curId = CalculateNearestTargetId();
                if (mIdPress == curId)
                {
                    if (mScrollRect.velocity.x > 0)
                    {
                        mScrollRect.velocity = Vector2.zero;
                        ScrollBack();
                    }
                    else if (mScrollRect.velocity.x < 0)
                    {
                        mScrollRect.velocity = Vector2.zero;
                        ScrollNext();
                    }
                }
                else
                {
                    mForceTargetId = curId;
                    mState = FocusState.UPDATE_FOCUS;
                    KickMove(true);
                }


            }
            else
            {
                //				Debug.LogError("Tooweek " + mScrollRect.velocity);
                mState = FocusState.UPDATE_FOCUS;
            }
        }


        /// <summary>
        /// Call back when scrollView reach any cell.
        /// </summary>
        /// <param name="id">Id of cell reached.</param>
        void OnMyReachCell(int id)
        {
            CheckOnOffArrow(id);
        }

        protected void CheckOnOffArrow(int currentId)
        {
            if (mBackArrow != null)
            {
                mBackArrow.interactable = currentId > 0;
            }
            if (mNextArrow != null)
            {
                mNextArrow.interactable = currentId < MaxId;
            }
        }

        public void SetupAdjustSizeX(float pX)
        {
            var size = mContent.sizeDelta;
            size.x = pX;
            mContent.sizeDelta = size;

            CheckOnOffArrow(CalculateNearestTargetId());
        }

        public void SetupAdjustSizeY(float pY)
        {
            var size = mContent.sizeDelta;
            size.y = pY;
            mContent.sizeDelta = size;

            CheckOnOffArrow(CalculateNearestTargetId());
        }


        /// <summary>
        /// Scroll to back Cell.
        /// </summary>
        public void ScrollBack()
        {
            int curId = CurrentId;
            if (curId > 0)
            {
                mForceTargetId = curId - 1;

                if (mState == FocusState.IDLE)
                {
                    mState = FocusState.UPDATE_FOCUS;
                    KickMove(false);
                }
            }
        }

        /// <summary>
        /// Scroll to next Cell.
        /// </summary>
        public void ScrollNext()
        {
            int curId = CurrentId;
            if (curId < MaxId)
            {
                mForceTargetId = curId + 1;

                if (mState == FocusState.IDLE)
                {
                    mState = FocusState.UPDATE_FOCUS;
                    KickMove(true);
                }
            }
        }

        public void ForceScrollToPos(float targetX, float targetY)
        {
            var contentPos = mContent.localPosition;
            if (IsHorizontal)
            {
                contentPos.x = targetX;
            }
            else
            {
                contentPos.y = targetY;
            }

            mContent.localPosition = contentPos;
        }


        /// <summary>
        /// Scrolls to any cell.
        /// </summary>
        /// <param name="targetId">Id of Cell which want to scroll to.</param>
        public void ScrollToCell(int targetId, bool force = false)
        {
            int oldId = CurrentId;
            if (targetId == oldId)
            {
                OnMyReachCell(targetId);
                return;
            }

            if (targetId >= 0 && targetId <= MaxId)
            {
                mForceTargetId = targetId;
                if (force)
                {
                    ForceScrollToTarget();
                    OnMyReachCell(targetId);

                    mState = FocusState.UPDATE_FOCUS;
                    KickMove(true);
                }
                else if (mState == FocusState.IDLE)
                {
                    mState = FocusState.UPDATE_FOCUS;
                    KickMove(mForceTargetId > oldId);
                }
            }
        }

        void ForceScrollToTarget()
        {
            var contentPos = mContent.localPosition;
            if (IsHorizontal)
            {
                float targetX = -mForceTargetId * mCellSize.x;
                contentPos.x = targetX;
            }
            else
            {
                float targetY = mForceTargetId * mCellSize.y;
                contentPos.y = targetY;
            }

            mContent.localPosition = contentPos;
        }

        /// <summary>
        /// Kick the move horizontal start.
        /// </summary>
        /// <param name="isKickToRight">If set to <c>true</c> scroll to show right side.</param>
        void KickMove(bool isKickToNext)
        {
            var contentPos = mContent.anchoredPosition;

            if (IsHorizontal)
            {
                contentPos.x += Epsinol * (isKickToNext ? -1 : 1);
            }
            else
            {
                contentPos.y += Epsinol * (isKickToNext ? 1 : -1);
            }

            mContent.anchoredPosition = contentPos;
        }

        /// <summary>
        /// Gets the id of current Cell. if scrolling at mid of Cell[i] and Cell[i+1] -> return float of mid of [i,i+1].
        /// </summary>
        /// <value>The current float identifier.</value>
        public float currentFloatId
        {
            get
            {
                float targetIdFloat = 0;
                if (IsHorizontal)
                {
                    float curPosX = mContent.anchoredPosition.x;
                    targetIdFloat = -curPosX / mCellSize.x;
                }
                else
                {
                    float curPosY = mContent.anchoredPosition.y;
                    targetIdFloat = curPosY / mCellSize.y;
                }

                targetIdFloat = Mathf.Min(targetIdFloat, MaxId);
                targetIdFloat = Mathf.Max(targetIdFloat, 0);

                return targetIdFloat;
            }
        }

        public float CurrentIdFloat
        {
            get
            {
                float targetIdFloat = 0;
                if (IsHorizontal)
                {
                    float curPosX = mContent.anchoredPosition.x;
                    targetIdFloat = -curPosX / mCellSize.x;
                }
                else
                {
                    float curPosY = mContent.anchoredPosition.y;
                    targetIdFloat = curPosY / mCellSize.y;
                }
                return Mathf.Max(0, Mathf.Min(targetIdFloat, MaxId));
            }
        }

        /// <summary>
        /// Gets the id of current Cell.
        /// </summary>
        /// <value>The current identifier.</value>
        public int CurrentId
        {
            get
            {
                int targetIdInt = Mathf.RoundToInt(CurrentIdFloat);

                targetIdInt = Mathf.Min(targetIdInt, MaxId);
                targetIdInt = Mathf.Max(targetIdInt, 0);

                return targetIdInt;
            }
        }


        /// <summary>
        /// Gets the max id of all cell.
        /// </summary>
        /// <value>The max identifier.</value>
        public int MaxId
        {
            get
            {
                int maxId = 0;
                if (IsHorizontal)
                {
                    float sub = mViewport.rect.width - mCellSize.x;
                    float ext = (sub > 0) ? (sub % mCellSize.x) : sub;
                    maxId = Mathf.Max(0, (int)((mContent.rect.width - ext) / mCellSize.x + 0.1f) - 1);
                }
                else
                {
                    float sub = mViewport.rect.height - mCellSize.y;
                    float ext = (sub > 0) ? (sub % mCellSize.y) : sub;
                    maxId = Mathf.Max(0, (int)((mContent.rect.height - ext) / mCellSize.y + 0.1f) - 1);
                }

                return maxId;
            }
        }


        public RectTransform mContent
        {
            get
            {
                return (mScrollRect != null) ? mScrollRect.content : null;
            }
        }

        RectTransform mViewport
        {
            get
            {
                return (mScrollRect != null) ? mScrollRect.viewport : null;
            }
        }

        float mVelocityThreshold
        {
            get
            {
                return (mScrollRect != null && IsHorizontal) ? mCellSize.x : mCellSize.y;
            }
        }

        float mDeltaPosThreshold
        {
            get
            {
                return mVelocityThreshold * 0.01f;		// with fps = 100;
            }
        }

        public Vector2 CellSize
        {
            get
            {
                return mCellSize;
            }
        }

        bool IsHorizontal => mScrollRect.horizontal;
    }
}
