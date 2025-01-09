using System.Collections;
using System.Collections.Generic;
using Koi.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Koi.UI
{
    public class BlinkImage : MonoBehaviour
    {
        [SerializeField] private Image m_target;
        [SerializeField] private float blinkDuration = 1.0f;
        [SerializeField] private float timeCount = 0;
        [SerializeField] private Vector2 alphaRange = new Vector2(0, 1);

        void Update()
        {
            timeCount += Time.deltaTime;
            if (timeCount > blinkDuration)
            {
                timeCount -= blinkDuration;
            }

            float ratio = timeCount / blinkDuration;
            Color color = target.color;
            float sin = Mathf.Sin(ratio * Mathf.PI);
            float value = (1 + sin) * 0.5f;
            color.a = Mathf.Lerp(alphaRange.x, alphaRange.y, value);
            target.color = color;
        }

        private Image target
        {
            get
            {
                if (m_target == null)
                {
                    m_target = gameObject.GetComponent<Image>();
                }
                return m_target;
            }
        }
    }
}