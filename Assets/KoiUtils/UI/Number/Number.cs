using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Koi.Render2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Koi.UI
{

    public class Number : BaseNumber
    {
        [Header("UI setup")]
        [SerializeField] bool _raycastTarget = true;
        public bool raycastTarget
        {
            get { return _raycastTarget; }
            set
            {
                if (_raycastTarget != value)
                {
                    _raycastTarget = value;
                    UpdateRaycastTarget();
                }
            }
        }

        /// <summary>
        /// The digit prefab. prefab defines transform of digit ImageUI.
        /// prefab should be child of this object (number object) and set disable (now show, only use to define transform setting).
        /// </summary>
        [SerializeField] Image digitPrefab;

        void Start()
        {
            UpdateValue();
            initedInScene = true;
        }

        public void UpdateRaycastTarget()
        {
            for (int i = 0; i < nDigitImages; i++)
            {
                ((DigitRenderUI)digitImages[i]).raycastTarget = raycastTarget;
            }
        }

        protected override void SetupDigitImage(DigitRender pDigitRender)
        {
            Image pDigitImage = ((DigitRenderUI)pDigitRender).image;

            if (separateWidth)
            {
                var size = digitRectTransform.sizeDelta;
                size.x = pDigitImage.sprite.rect.width / pDigitImage.sprite.rect.height * size.y;
                pDigitImage.rectTransform.sizeDelta = size;
                pDigitImage.preserveAspect = false;
            }
            else
            {
                pDigitImage.rectTransform.sizeDelta = digitRectTransform.sizeDelta;
                pDigitImage.preserveAspect = true;
            }

            pDigitImage.gameObject.SetActive(true);
            pDigitImage.transform.localRotation = Quaternion.Euler(digitRectTransform.localRot);
            pDigitImage.transform.localScale = digitRectTransform.localScale;

            pDigitImage.color = color;
        }


        protected override void CheckDigitImagePointer()
        {
            for (int i = nDigitImages - 1; i >= 0; i--)
            {
                if (digitImages[i].gameObject == null)
                {
                    digitImages.RemoveAt(i);
                }
                else
                {
                    try
                    {
                        DigitRenderUI digitUI = (DigitRenderUI)digitImages[i];
                        if (digitUI.image == null)
                        {
                            digitImages.RemoveAt(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        digitImages.RemoveAt(i);
                    }
                }
            }

            var listChild = digitContainer.GetComponentsInChildren<Image>();
            foreach (Image img in listChild)
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
                    digitImages.Add(CreateDigitRender(img));
                }
            }
        }

        protected DigitRenderUI CreateDigitRender(Image img)
        {
            DigitRenderUI result = new DigitRenderUI(img);
            return result;
        }

        protected override void UpdateDigitImagePos()
        {
            int nSprite = nRenderElement;

            //float totalWidth = (digits.Count - 1) * digitSpace;
            float totalWidth = (nSprite - 1) * digitSpace;
            for (int i = 0; i < nSprite; i++)
            {
                totalWidth += digitImages[i].size_x;
            }

            float xStart = 0;

            switch (aligment)
            {
                case NumberAlignment.LEFT:
                    break;

                case NumberAlignment.CENTER:
                    xStart = -totalWidth * 0.5f;
                    break;

                case NumberAlignment.RIGHT:
                    xStart = -totalWidth;
                    break;
            }

            for (int i = 0; i < nSprite; i++)
            {
                float curDigitWidth = digitImages[i].size_x;
                digitImages[i].gameObject.transform.localPosition = new Vector3(xStart + curDigitWidth * 0.5f, 0, 0);
                xStart = xStart + curDigitWidth + digitSpace;
            }
        }


        protected override void AddDigitImage()
        {
            Image img;
            if (digitPrefab != null)
            {
                img = Instantiate(digitPrefab);
            }
            else
            {
                var obj = new GameObject("digit_" + nDigitImages);
                obj.AddComponent<RectTransform>();
                img = obj.AddComponent<Image>();
            }
            img.transform.SetParent(digitContainer);
            img.transform.localRotation = Quaternion.Euler(digitRectTransform.localRot);
            img.transform.localScale = digitRectTransform.localScale;
            img.rectTransform.sizeDelta = digitRectTransform.sizeDelta;
            img.raycastTarget = raycastTarget;

            digitImages.Add(CreateDigitRender(img));
        }


        /// <summary>
        /// Updates the size of the digit. Only use for editor
        /// </summary>
        public override void UpdateDigitSize()
        {
            if (digitPrefab != null)
            {
                digitRectTransform.sizeDelta = digitPrefab.rectTransform.sizeDelta;
            }
            else if (_sprites != null)
            {
                digitRectTransform.sizeDelta = _sprites[0].rect.size;
            }
        }


        protected override Transform digitContainer
        {
            get
            {
                if (_digitContainer == null)
                {
                    var obj = new GameObject("DigitContainer");
                    obj.transform.parent = transform;
                    _digitContainer = obj.AddComponent<RectTransform>();
                    _digitContainer.localPosition = Vector3.zero;
                    _digitContainer.localRotation = Quaternion.identity;
                    _digitContainer.localScale = Vector3.one;
                }
                return _digitContainer;
            }
        }


        public float numberWidth
        {
            get
            {
                int nSprite = nRenderElement;

                //float totalWidth = (digits.Count - 1) * digitSpace;
                float totalWidth = (nSprite - 1) * digitSpace;
                for (int i = 0; i < nSprite; i++)
                {
                    totalWidth += digitImages[i].size_x;
                }
                return totalWidth;
            }
        }



        [Serializable]
        public class DigitRenderUI : DigitRender
        {
            public Image image;


            public DigitRenderUI()
            {

            }

            public DigitRenderUI(Image img)
            {
                this.image = img;
                if (img != null)
                {
                    this.sprite = img.sprite;
                }
                this.m_gameObject = img.gameObject;// for debug
            }

            public override Sprite sprite
            {
                get => image.sprite;
                set => image.sprite = value;
            }

            public override Color color
            {
                get => image.color;
                set => image.color = value;
            }

            public bool raycastTarget
            {
                get => image.raycastTarget;
                set => image.raycastTarget = value;
            }

            public override float size_x => image.rectTransform.sizeDelta.x;

            public override GameObject gameObject => (image != null) ? image.gameObject : null;
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(Number))]
    public class NumberEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Number myScript = (Number)target;

            GUILayout.Label("");
            GUILayout.Label("Update DigitRectTransform Size from prefab data or spritedata");

            if (GUILayout.Button("Setup Digit Size"))
            {
                myScript.UpdateDigitSize();
            }

            GUILayout.Label("");

            GUILayout.Label("Update Raycast Target");

            if (GUILayout.Button("UpdateRaycastTarget"))
            {
                myScript.UpdateRaycastTarget();
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
