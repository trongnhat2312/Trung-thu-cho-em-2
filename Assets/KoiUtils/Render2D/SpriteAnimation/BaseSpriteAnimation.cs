using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koi.Render2D
{
    public class BaseSpriteAnimation : MonoBehaviour
    {

        public enum AniType
        {
            FromTo,
            ListId
        }

        public enum AniState
        {
            Idle,
            Running
        }

        protected Action OnEndAnimationListener;
        protected Action OnEndLoopListener;
        protected Action OnEndLoopCallbackListener;

        [Header("Base Setup")]
        [SerializeField] protected List<Sprite> listSprite = new List<Sprite>();
        [SerializeField] protected bool _isLoop = false;
        [SerializeField] protected AniType aniType;

        [SerializeField] protected int startId;
        [SerializeField] protected int endId;

        [SerializeField] protected List<int> listFrameIds = new List<int>();

        [SerializeField] protected float frameDuration = 0.016f;
        [SerializeField] protected List<float> listFrameDuration = new List<float>();

        [SerializeField] protected AniState state = AniState.Idle;
        protected float timeRemain = 0;
        protected int curIdInList = 0;

        protected virtual void Start()
        {
            ConvertToList();
            timeRemain = listFrameDuration[0];
        }

        public virtual void AddEndAnimationListener(Action pListener)
        {
            OnEndAnimationListener -= pListener;
            OnEndAnimationListener += pListener;
        }

        public virtual void AddEndLoopListener(Action pListener)
        {
            OnEndLoopListener -= pListener;
            OnEndLoopListener += pListener;
        }

        public virtual void StartAnimation(int pStartId, int pEndId, float pDuration, bool isLoop, int startIdInList = 0, Action endLoopCallback = null)
        {
            OnEndLoopCallbackListener = endLoopCallback;
            aniType = AniType.FromTo;
            this.startId = pStartId;
            this.endId = pEndId;
            this.frameDuration = pDuration;
            listFrameDuration.Clear();
            this._isLoop = isLoop;
            ConvertToList();
            state = AniState.Running;
            curIdInList = startIdInList;

            UpdateRenderer(listSprite[listFrameIds[curIdInList]]);
        }

        public virtual void StartAnimation(Action endloopCallback = null)
        {
            if (aniType == AniType.FromTo)
            {
                StartAnimation(startId, endId, frameDuration, _isLoop, 0, endloopCallback);
            }
            else
            {
                StartAnimation(listFrameIds, frameDuration, _isLoop, endloopCallback);
            }
        }

        public virtual void StartAnimation(List<int> frameIds, bool isLoop = false, Action endloopCallback = null)
        {
            StartAnimation(frameIds, frameDuration, isLoop, endloopCallback);
        }

        public virtual void StartAnimation(List<int> frameIds, float pFrameDuration, bool isLoop, Action endloopCallback = null)
        {
            OnEndLoopCallbackListener = endloopCallback;
            this.listFrameIds = frameIds;
            this.frameDuration = pFrameDuration;
            listFrameDuration.Clear();
            this._isLoop = isLoop;
            ConvertToList();
            state = AniState.Running;
            curIdInList = 0;

            UpdateRenderer(listSprite[listFrameIds[curIdInList]]);
        }

        protected virtual void UpdateRenderer(Sprite pSprite)
        {
            
        }

        protected virtual void ConvertToList()
        {
            if (aniType == AniType.FromTo)
            {
                aniType = AniType.ListId;
                listFrameIds.Clear();
                for (int i = startId; i <= endId; i++)
                {
                    listFrameIds.Add(i);
                }
            }

            if (listFrameDuration.Count < listFrameIds.Count)
            {
                listFrameDuration.Clear();
                for (int i = 0; i < listFrameIds.Count; i++)
                {
                    listFrameDuration.Add(frameDuration);
                }
            }
        }


        protected virtual void Update()
        {
            switch (state)
            {
                case AniState.Idle:
                    break;

                case AniState.Running:
                    UpdateFrame();
                    break;
            }
        }

        protected virtual void UpdateFrame()
        {
            timeRemain -= Time.deltaTime;
            if (timeRemain < 0)
            {
                timeRemain += listFrameDuration[curIdInList];

                curIdInList = (curIdInList + 1) % listFrameIds.Count;

                if (curIdInList == 0)
                {
                    OnEndLoop();
                    if (_isLoop)
                    {
                        UpdateRenderer(listSprite[listFrameIds[curIdInList]]);
                    }
                    else
                    {
                        state = AniState.Idle;
                    }
                }
                else
                {
                    UpdateRenderer(listSprite[listFrameIds[curIdInList]]);
                }
            }
        }

        protected virtual void OnEndLoop()
        {
            if (OnEndLoopListener != null)
            {
                OnEndLoopListener();
            }

            if (OnEndLoopCallbackListener != null)
            {
                OnEndLoopCallbackListener();
                OnEndLoopCallbackListener = null;
            }

            if (OnEndAnimationListener != null && !_isLoop)
            {
                OnEndAnimationListener();
            }
        }


        public virtual void Stop()
        {
            state = AniState.Idle;
        }

        public virtual void SetCurrendIdex(int index)
        {
            if (index >= 0 && index < listSprite.Count)
            {
                UpdateRenderer(listSprite[index]);
            }
        }

        public virtual void ResetToFirstFrame()
        {
            curIdInList = 0;
            timeRemain = listFrameDuration[curIdInList];
            UpdateRenderer(listSprite[listFrameIds[curIdInList]]);
        }


        public int curSpriteId { get { return curIdInList; } }
    }
}

