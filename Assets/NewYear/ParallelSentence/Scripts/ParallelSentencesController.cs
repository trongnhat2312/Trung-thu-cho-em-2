using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace iSTEAM.STEAM.ParallelSentences
{

    public class ParallelSentencesController : MonoBehaviour
    {
        [SerializeField]
        ParallelSentencesSideController leftSide;

        [SerializeField]
        ParallelSentencesSideController rightSide;

        [SerializeField]
        GameObject bonusGuideRoot;

		private void Update()
		{
            if (Input.GetKeyDown(KeyCode.P))
            {
                OnCompleted();
            }
		}

        public void ResetBeforeTransform()
        {
            leftSide.gameObject.SetActive(false);
            rightSide.gameObject.SetActive(false);
        }


        public async void OnCompleted()
        {
            leftSide.gameObject.SetActive(true);
            rightSide.gameObject.SetActive(true);

            // todo - effect

            //await UniTask.Delay(500);

            leftSide.AppearScroll();
            rightSide.AppearScroll();

            await UniTask.Delay(1000);

            leftSide.AppearWords();

            await UniTask.Delay(3000);

            rightSide.AppearWords();

            await UniTask.Delay(3600);
            bonusGuideRoot.gameObject.SetActive(true);
        }
    }
}