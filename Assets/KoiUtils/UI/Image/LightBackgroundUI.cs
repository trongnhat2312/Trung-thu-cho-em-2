using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Koi.MathUtils;

namespace Koi.UI
{
    public class LightBackgroundUI : MonoBehaviour
    {
        [SerializeField] LightPart leftPart;
        [SerializeField] LightPart rightPart;
        [SerializeField] float vRot = 15;
        [SerializeField] float vRotRand = 30;
        [SerializeField] float scaleDuration = 3.0f;
        [SerializeField] float scaleDurationRand = 1.0f;
        [SerializeField] bool activeOnStart = true;
        [SerializeField] bool ignoreTimeScale = true;

        void Start()
        {
            if (activeOnStart)
            {
                ActiveLight();
            }
        }

        public void ActiveLight()
        {
            leftPart.Setup(vRotate: vRot + vRotRand * Rand01,
                           scaleDuration: scaleDuration + scaleDurationRand * Rand01);
            rightPart.Setup(vRotate: -(vRot + vRotRand * Rand01),
                            scaleDuration: scaleDuration + scaleDurationRand * Rand01);
        }


        public void SetColor(Color color)
        {
            leftPart.SetColor(color);
            rightPart.SetColor(color);
        }

        public void FadeInLight(float duration)
        {
            leftPart.FadeIn(duration);
            rightPart.FadeIn(duration);
        }

        public void FadeOutLight(float duration)
        {
            leftPart.FadeOut(duration);
            rightPart.FadeOut(duration);
        }


        void Update()
        {
            float deltaTime = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            leftPart.Update(deltaTime);
            rightPart.Update(deltaTime);
        }



        float Rand01 { get { return Random.Range(0.0f, 1.0f); } }






        [System.Serializable]
        public class LightPart
        {
            [SerializeField] public Image image;
            [SerializeField] public float vRot;
            [SerializeField] Vector2 scaleRange = new Vector2(0.9f, 1.2f);
            [SerializeField] public float scaleDuration;
            [SerializeField] public float scaleTimeRemain;

            public void Setup(float vRotate, float scaleDuration)
            {
                this.vRot = vRotate;
                this.scaleDuration = scaleDuration;
                this.scaleTimeRemain = scaleDuration * Random.Range(1.0f, 2.0f);
            }

            public void SetColor(Color color)
            {
                image.color = color;
            }

            void SetAlpha(float alpha)
            {
                var color = image.color;
                color.a = alpha;
                image.color = color;
            }

            public void FadeIn(float duration)
            {
                SetAlpha(0);
                image.DOFade(1.0f, duration);
            }

            public void FadeOut(float duration)
            {
                image.DOFade(0.0f, duration);
            }

            public void Update(float deltaTime)
            {
                UpdateRot(deltaTime);

                UpdateScale(deltaTime);
            }

            void UpdateRot(float deltaTime)
            {
                var rot = image.rectTransform.localRotation.eulerAngles;
                rot.z += vRot * deltaTime;
                image.rectTransform.localRotation = Quaternion.Euler(rot);
            }

            void UpdateScale(float deltaTime)
            {
                scaleTimeRemain -= deltaTime;
                while (scaleTimeRemain < 0)
                {
                    scaleTimeRemain += scaleDuration + scaleDuration;
                }

                float scale = 1;
                if (scaleTimeRemain < scaleDuration)
                {
                    scale = LerpUtils.Lerp(scaleRange.x, scaleRange.y, 1 - scaleTimeRemain / scaleDuration, EasyType.InSine);
                }
                else
                {
                    scale = LerpUtils.Lerp(scaleRange.y, scaleRange.x, 2 - scaleTimeRemain / scaleDuration, EasyType.OutSine);
                }
                image.rectTransform.localScale = Vector3.one * scale;
            }
        }
    }

}