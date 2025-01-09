using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koi.Render2D
{
    public class BaseNumber : MonoBehaviour
    {
        private const float EPSILON = 0.001f;

        [Serializable]
        public class DigitRender
        {
            [SerializeField] protected Sprite m_sprite = null;
            [SerializeField] protected GameObject m_gameObject = null;
            public virtual Sprite sprite
            {
                get => m_sprite;
                set => m_sprite = value;
            }

            protected Color m_color = Color.white;
            public virtual Color color
            {
                get => m_color;
                set => m_color = value;
            }

            public virtual float size_x => 0;

            public virtual GameObject gameObject => m_gameObject;
        }

        [Serializable]
        public class NumberRectTransofrm
        {
            public Vector3 localRot = Vector3.zero;
            public Vector3 localScale = Vector3.one;

            public Vector2 sizeDelta = new Vector2(45, 80);
        }

        [Serializable]
        public class MetricPrefixSetup
        {
            public bool useMetricPrefix = false;
            public int nCharAfterDot = 1;
            public Sprite dot;
            public Sprite kilo;
            public Sprite million;
            public Sprite billion;
        }

        public enum NumberAlignment
        {
            CENTER = 0,
            LEFT,
            RIGHT
        }

        [Header("Resource Setup")]
        /// <summary>
        /// The sprites of digits (from 0 to 9).
        /// </summary>
        [SerializeField] protected List<Sprite> _sprites = new List<Sprite>();

        [SerializeField] protected MetricPrefixSetup metricPrefixSetup;
        [SerializeField] protected bool _useDot = false;

        [Header("Font config")]
        public NumberAlignment aligment = NumberAlignment.LEFT;

        /// <summary>
        /// The padding of digits.
        /// </summary>
        [SerializeField] protected float _digitSpace = 0;
        [SerializeField] protected Color color = Color.white;
        [SerializeField] protected NumberRectTransofrm digitRectTransform;
        [SerializeField] protected bool _separateWidth = false;


        [SerializeField] protected Transform _digitContainer;

        [Header("Debug")]
        [SerializeField] protected int _number = 0;
        [SerializeField] protected List<int> digits = new List<int>();
        [SerializeField] protected List<DigitRender> digitImages = new List<DigitRender>();
        [SerializeField] protected int nCharacter = 0;

        protected bool initedInScene = false;

        private void Start()
        {
            UpdateValue();
            initedInScene = true;
        }

        private void OnEnable()
        {
            UpdateValue();
            initedInScene = true;
        }

        public virtual void UpdateValue()
        {
            try
            {
                UpdateDigits();

                UpdateDigitImages();

                UpdateDigitImagePos();
            }
            catch (Exception ex)
            {
            }
        }

        public virtual void SetAllNumberColor(Color pColor)
        {
            this.color = pColor;
            foreach (DigitRender num in digitImages)
            {
                num.color = pColor;
            }
        }


        protected virtual void UpdateDigits()
        {
            digits.Clear();

            int countDigit = 0;

            int tempNumber = (int)_number;
            int digit = tempNumber % 10;
            digits.Insert(0, digit);
            tempNumber = tempNumber / 10;
            countDigit++;
            while (tempNumber > 0)
            {
                if (useDot && countDigit % 3 == 0 && !metricPrefixSetup.useMetricPrefix)
                {
                    digits.Insert(0, 10);
                }

                digit = tempNumber % 10;
                digits.Insert(0, digit);
                tempNumber = tempNumber / 10;
                countDigit++;
            }
        }

        protected virtual void UpdateDigitImages()
        {
            CheckDigitImagePointer();

            if (metricPrefixSetup.useMetricPrefix)
            {
                UpdateDigitImagesMetricPrefix();
            }
            else
            {
                UpdateDigitImagesNormal();
            }
        }

        protected virtual void CheckDigitImagePointer()
        {
        }

        protected virtual void SetupDigitImageSprite(int digitImageId, Sprite pSprite)
        {
            digitImages[digitImageId].sprite = pSprite;
        }

        protected virtual void UpdateDigitImagesNormal()
        {
            nCharacter = digits.Count;
            for (int i = 0; i < digits.Count; i++)
            {
                if (nDigitImages < i + 1)
                {
                    AddDigitImage();
                }
                if (digits[i] < sprites.Count)
                {
                    SetupDigitImageSprite(i, sprites[digits[i]]);
                    SetupDigitImage(digitImages[i]);
                }
            }

            for (int i = digits.Count; i < nDigitImages; i++)
            {
                digitImages[i].gameObject.SetActive(false);
            }
        }

        protected virtual void UpdateDigitImagesMetricPrefix()
        {
            if (!ProcessMetricPrefixData(out int length, out int dotPos, out Sprite unitSprite))
            {
                UpdateDigitImagesNormal();
                return;
            }

            nCharacter = length;
            for (int i = 0; i < length; i++)
            {
                if (nDigitImages < i + 1)
                {
                    AddDigitImage();
                }

                if (i == dotPos)
                {

                    SetupDigitImageSprite(i, metricPrefixSetup.dot);
                }
                else if (i == length - 1)
                {
                    SetupDigitImageSprite(i, unitSprite);
                }
                else
                {
                    int characterValue = (i < dotPos) ? digits[i] : digits[i - 1];
                    if (characterValue < sprites.Count)
                    {
                        SetupDigitImageSprite(i, sprites[characterValue]);
                    }
                }

                SetupDigitImage(digitImages[i]);
            }

            for (int i = length; i < nDigitImages; i++)
            {
                digitImages[i].gameObject.SetActive(false);
            }
        }

        protected virtual bool ProcessMetricPrefixData(out int length, out int dotPos, out Sprite unitSprite)
        {
            length = 0;
            if (digits.Count > 9 && metricPrefixSetup.billion != null)
            {
                dotPos = digits.Count - 9;
                unitSprite = metricPrefixSetup.billion;
            }
            else if (digits.Count > 6 && metricPrefixSetup.million != null)
            {
                dotPos = digits.Count - 6;
                unitSprite = metricPrefixSetup.million;
            }
            else if (digits.Count > 3 && metricPrefixSetup.kilo != null)
            {
                dotPos = digits.Count - 3;
                unitSprite = metricPrefixSetup.kilo;
            }
            else
            {
                dotPos = -1;
                unitSprite = null;
                return false;
            }

            // because render "dot" and "unit"
            // => n_element MAX <= digits.Count + 2
            // and litMit of nCharAfterDot => n_element Max <= dotPos + 1 + nCharAfterDot + 1;
            length = Mathf.Min(digits.Count + 2, dotPos + 1 + metricPrefixSetup.nCharAfterDot + 1);
            return true;
        }

        protected virtual void AddDigitImage()
        {
        }

        protected virtual void SetupDigitImage(DigitRender pDigitRender)
        { 
        }


        protected virtual void UpdateDigitImagePos()
        {
        }

        public virtual void UpdateDigitSize()
        {

        }

        public virtual int nDigitImages => digitImages.Count;

        public int nRenderElement
        {
            get
            {
                return nCharacter;
                //for (int i = 0; i < digitImages.Count; i++)
                //{
                //    if (digitImages[i] == null || !digitImages[i].gameObject.activeInHierarchy)
                //    {
                //        return i;
                //    }
                //}
                //return digitImages.Count;
            }
        }

        public List<Sprite> sprites
        {
            get { return _sprites; }
            set
            {
                if (_sprites != value)
                {
                    _sprites = value;
                    UpdateDigitImages();
                }
            }
        }

        public float digitSpace
        {
            get { return _digitSpace; }
            set
            {
                if (Math.Abs(_digitSpace - value) > EPSILON)
                {
                    _digitSpace = value;
                    UpdateDigitImagePos();
                }
            }
        }

        public bool useDot
        {
            get { return _useDot; }
            set
            {
                if (_useDot != value)
                {
                    _useDot = value;
                    UpdateValue();
                }
            }
        }


        public bool separateWidth
        {
            get { return _separateWidth; }
            set
            {
                if (_separateWidth != value)
                {
                    _separateWidth = value;
                    UpdateDigitImages();
                    UpdateDigitImagePos();
                }
            }
        }


        public int numberValue
        {
            get { return _number; }

            set
            {
                if (_number != value || !initedInScene)
                {
                    _number = Mathf.Max(value, 0);
                    UpdateValue();
                }
            }
        }

        protected virtual Transform digitContainer
        {
            get
            {
                if (_digitContainer == null)
                {
                    var obj = new GameObject("DigitContainer");
                    obj.transform.parent = transform;
                    _digitContainer = obj.AddComponent<Transform>();
                    _digitContainer.localPosition = Vector3.zero;
                    _digitContainer.localRotation = Quaternion.identity;
                    _digitContainer.localScale = Vector3.one;
                }
                return _digitContainer;
            }
        }
    }
}