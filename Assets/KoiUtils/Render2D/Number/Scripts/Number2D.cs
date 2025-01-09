using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Koi.Render2D
{

    public class Number2D : BaseNumber
    {
        [Header("Renderer 2D")]
        [SerializeField] SpriteRenderer digitPrefab;


        [Header("Debug")]
        [SerializeField] float pixelToUnit = 0.01f;

        [SerializeField] protected List<DigitRender2D> debug_digitImages = new List<DigitRender2D>();

        void Start()
        {
            UpdateValue();
            initedInScene = true;
        }

        protected override void SetupDigitImage(DigitRender pDigitRender)
        {
            SpriteRenderer pDigitImage = ((DigitRender2D)pDigitRender).image;

            var size = digitRectTransform.sizeDelta;
            size.x = pDigitImage.sprite.rect.width / pDigitImage.sprite.rect.height * size.y;
            //float scale = size.y / digitImages[i].sprite.rect.height;
            //digitImages[i].transform.localScale = scale * Vector3.one;
            pDigitImage.size = size * pixelToUnit;
            pDigitImage.gameObject.SetActive(true);

            pDigitImage.transform.localRotation = Quaternion.Euler(digitRectTransform.localRot);
            pDigitImage.transform.localScale = digitRectTransform.localScale;
            
            pDigitImage.color = color;
        }

        protected virtual void RemoveDigitImagesAt(int rmId)
        {
            digitImages.RemoveAt(rmId);
            debug_digitImages.RemoveAt(rmId);
        }

        protected virtual void AddDigitImage(DigitRender2D pDigit)
        {
            digitImages.Add(pDigit);
            debug_digitImages.Add(pDigit);
        }

        protected override void CheckDigitImagePointer()
        {
            for (int i = nDigitImages - 1; i >= 0; i--)
            {
                if (digitImages[i].gameObject == null)
                {
                    RemoveDigitImagesAt(i);
                }
            }

            var listChild = digitContainer.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer img in listChild)
            {
                bool exist = false;
                foreach (DigitRender digit in digitImages)
                {
                    if (digit.gameObject == img.gameObject)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    AddDigitImage(CreateDigitRender(img));
                }
            }
        }

        protected DigitRender2D CreateDigitRender(SpriteRenderer img)
        {
            DigitRender2D result = new DigitRender2D(img);
            return result;
        }

        protected override void UpdateDigitImagePos()
        {
            int nSprite = nRenderElement;

            float totalWidth = (nSprite - 1) * digitSpace;
            if (separateWidth)
            {
                for (int i = 0; i < nSprite; i++)
                {
                    totalWidth += digitImages[i].size_x / pixelToUnit;
                }
            }
            else
            {
                totalWidth += nSprite * digitRectTransform.sizeDelta.x;
            }


            float xStart = 0;

            switch (aligment)
            {
                case NumberAlignment.LEFT:
                    break;

                case NumberAlignment.CENTER:
                    xStart = -totalWidth * 0.5f * pixelToUnit;
                    break;

                case NumberAlignment.RIGHT:
                    xStart = -totalWidth * pixelToUnit;
                    break;
            }

            float space = digitSpace * pixelToUnit;
            float cellSeparateWidth = digitRectTransform.sizeDelta.x * pixelToUnit;
            for (int i = 0; i < nDigitImages; i++)
            {
                float curDigitWidth = separateWidth? digitImages[i].size_x : cellSeparateWidth;
                digitImages[i].gameObject.transform.localPosition = new Vector3(xStart + curDigitWidth * 0.5f, 0, 0);
                xStart = xStart + curDigitWidth + space;
            }
        }

        protected override void AddDigitImage()
        {
            base.AddDigitImage();
            SpriteRenderer img;
            if (digitPrefab != null)
            {
                img = Instantiate(digitPrefab);
            }
            else
            {
                var obj = new GameObject("digit_" + nDigitImages);
                img = obj.AddComponent<SpriteRenderer>();
                img.drawMode = SpriteDrawMode.Sliced;
            }
            img.transform.SetParent(digitContainer);
            img.transform.localRotation = Quaternion.Euler(digitRectTransform.localRot);
            img.transform.localScale = digitRectTransform.localScale;
            img.size = digitRectTransform.sizeDelta;

            AddDigitImage(CreateDigitRender(img));
        }


        public override void UpdateDigitSize()
        {
            base.UpdateDigitSize();

            if (digitPrefab != null)
            {
                digitRectTransform.sizeDelta = digitPrefab.size;
            }
            else if (_sprites != null && _sprites.Count > 0 && _sprites[0] != null)
            {
                digitRectTransform.sizeDelta = _sprites[0].rect.size;
            }
        }




        [Serializable]
        public class DigitRender2D : DigitRender
        {
            public SpriteRenderer m_image;
            public SpriteRenderer image
            {
                get
                {
                    if (m_image == null)
                    {
                        m_image = m_gameObject.GetComponent<SpriteRenderer>();
                    }

                    return m_image;
                }
            }

            public DigitRender2D()
            { 
            }

            public DigitRender2D(SpriteRenderer img)
            {
                this.m_image = img;
                if (img != null)
                {
                    this.sprite = img.sprite;
                }
                this.m_gameObject = m_image.gameObject;
            }

            public override Sprite sprite
            {
                get => m_image.sprite;
                set => m_image.sprite = value;
            }

            public override Color color
            {
                get => m_image.color;
                set => m_image.color = value;
            }

            public override float size_x => m_image.size.x;

            public override GameObject gameObject => (m_image != null) ? m_image.gameObject : null;
        }
    }



    #if UNITY_EDITOR
    [CustomEditor(typeof(Number2D))]
    public class Number2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Number2D myScript = (Number2D)target;

            GUILayout.Label("");
            GUILayout.Label("Update DigitRectTransform Size from prefab data or spritedata");

            if (GUILayout.Button("Setup Digit Size"))
            {
                myScript.UpdateDigitSize();
            }


            GUILayout.Label("");

            GUILayout.Label("UpdateValue");

            if (GUILayout.Button("UpdateValue"))
            {
                myScript.UpdateValue();
            }

            GUILayout.Label("");
        }
    }
#endif
}