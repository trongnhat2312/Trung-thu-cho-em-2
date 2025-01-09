using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Koi.Render2D;


namespace Koi.UI
{
    public class SpriteAnimationUI : BaseSpriteAnimation
    {
        [Header("UI")]
        [SerializeField] Image image;
        [SerializeField] bool setNativeSize = false;

        protected override void UpdateRenderer(Sprite pSprite)
        {
            image.sprite = pSprite;
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        }
    }
}
