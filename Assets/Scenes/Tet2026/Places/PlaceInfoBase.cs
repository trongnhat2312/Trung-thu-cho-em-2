using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.Places
{

    public class PlaceInfoBase : MonoBehaviour
    {

        private Action OnClosePlaceInfoListener;

        public GameObject[] PlaceInfos;

        public GameObject Question;

        private int numPlace;
        public Button QuestionButton;
        public int getNumPlace() { return numPlace; }
        GameObject placeInfoGened;



        void Start()
        {
            InitListener();
        }

        #region Listener
        private void InitListener()
        {
            QuestionButton.onClick.AddListener(OnBtnShowQuestionClicked);
        }
        public void AddOnClosePlaceInfoListener(Action listener)
        {
            OnClosePlaceInfoListener -= listener;
            OnClosePlaceInfoListener += listener;
        }
        #endregion

        public void OpenPlaceInfo(int i, bool isUnlocked, Action pCloseCallback = null, Action openQRCallback = null)
        {
            OnClosePlaceInfoListener = pCloseCallback;
            Debug.LogError($"PlaceInfoBase: open place == {i}");
            numPlace = i;
            int offset = isUnlocked ? StaticParamClass.MAX_PLACE : 0;
            placeInfoGened = Instantiate(PlaceInfos[i + offset]);
            placeInfoGened.transform.SetParent(transform, false);
            placeInfoGened.name = "PlaceInfo " + i;
            // QuestionButton.gameObject.SetActive(isUnlocked && i != 0 && i != 5);
            QuestionButton.gameObject.SetActive(false);

            try
            {
                placeInfoGened.GetComponent<PlaceInfoNew2026>().Open(OnBtnCloseClicked, openQRCallback);
            }
            catch (Exception exception)
            {
                Debug.LogError($"PlaceInfoBase OpenPlaceInfo exception: {exception.Message}");
            }
        }

        private async void HidePopup()
        {
            OnClosePlaceInfoListener?.Invoke();
            gameObject.SetActive(false);
            await UniTask.DelayFrame(3);
            DestroyImmediate(placeInfoGened);
        }

        private void OnBtnCloseClicked()
        {
            HidePopup();
        }

        private void OnBtnShowQuestionClicked()
        {
            var question = Instantiate(Question);
            question.GetComponent<Question>().SetPlaceInfoHolder(numPlace);
            question.transform.SetParent(transform.parent.transform, false);
            question.name = "Question";
        }
    }
}