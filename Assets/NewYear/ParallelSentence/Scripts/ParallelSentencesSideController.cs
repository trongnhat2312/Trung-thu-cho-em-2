using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace iSTEAM.STEAM.ParallelSentences
{

    public class ParallelSentencesSideController : MonoBehaviour
    {
        [SerializeField]
        Image scrollBg;

        [SerializeField]
        Image scrollBgSub;

        [SerializeField]
        Vector2 heightRange = new Vector2(360, 1080);

        [SerializeField]
        List<ParallelSentencesWordController> words = new List<ParallelSentencesWordController>();


        public async void AppearScroll(bool showWord = false)
        {
            foreach (var word in words)
            {
                word.gameObject.SetActive(showWord);
            }

            SetupScrollHeight(heightRange.x);

            float duration = 1.2f;
            DOTween.To(() => 0.0f, (f) =>
            {
                float height = Mathf.Lerp(heightRange.x, heightRange.y, f);
                SetupScrollHeight(height);
            }, 1.0f, duration).SetEase(Ease.OutBounce);

            await UniTask.Delay((int)(duration * 1000));
        }

        public async void AppearWords()
        {
            float delay = 0.35f;
            for (int i = 0; i < words.Count; i++)
            {
                AppearWord(words[i]);

                await UniTask.Delay((int)(delay * 1000));
            }
        }

        void AppearWord(ParallelSentencesWordController word)
        {
            word.gameObject.SetActive(true);
            word.transform.localScale = Vector3.one * 0.01f;
            word.transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack);

            // effect???
        }

        void SetupScrollHeight(float pHeight)
        {
            var size = scrollBg.rectTransform.sizeDelta;
            size.y = pHeight;
            scrollBg.rectTransform.sizeDelta = size;

            scrollBgSub.fillAmount = Mathf.Clamp01((pHeight - heightRange.x * 0.35f) / heightRange.y);
        }
    }
}