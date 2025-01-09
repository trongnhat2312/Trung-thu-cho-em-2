using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Koi.Render2D
{
    public class SpriteAnimation : BaseSpriteAnimation
    {
        [Header("SpriteRenderer2D setup")]
        [SerializeField] SpriteRenderer _mRenderer;

        public SpriteRenderer mRenderer
        {
            get
            {
                if (_mRenderer == null)
                {
                    _mRenderer = GetComponent<SpriteRenderer>();
                }
                return _mRenderer;
            }
        }

        protected override void UpdateRenderer(Sprite pSprite)
        {
            mRenderer.sprite = pSprite;
        }

        public void SetFlipX(bool isFlipX)
        {
            mRenderer.flipX = isFlipX;
        }

        public void SetFlipY(bool isFlipY)
        {
            mRenderer.flipY = isFlipY;
        }

        public bool flipX
        {
            get { return mRenderer.flipX; }
            set { mRenderer.flipX = value; }
        }

        public bool flipY
        {
            get { return mRenderer.flipY; }
            set { mRenderer.flipY = value; }
        }
    }
}
