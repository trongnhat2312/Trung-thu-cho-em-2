using System;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.Places
{

    public class PlaceInfoBase : MonoBehaviour
    {

        private Action OnClosePlaceInfoListener;
        private Action OnScanClickedListener;

        [SerializeField] private GameObject Question;
        [SerializeField] private PlaceInfoNew2026 placeInfoUnlock;
        [SerializeField] private PlaceInfoNew2026 placeInfoLock;

        private int numPlace;
        [SerializeField] private Button QuestionButton;
        [Header("PlaceInfo debug")]
        public PlaceInfoNew2026 placeInfo;

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

        public void OpenPlaceInfo(int pIdPlace, bool isUnlocked, Action pCloseCallback = null, Action openQRCallback = null)
        {
            OnClosePlaceInfoListener = pCloseCallback;
            OnScanClickedListener = openQRCallback;
            Debug.LogError($"PlaceInfoBase: open place == {pIdPlace}");
            numPlace = pIdPlace;
            QuestionButton.gameObject.SetActive(isUnlocked && pIdPlace != 0 && pIdPlace != 5);

            if (placeInfoLock != null)
            {
	            placeInfoLock.gameObject.SetActive(!isUnlocked);
            }

            if (placeInfoUnlock != null)
            {
	            placeInfoUnlock.gameObject.SetActive(isUnlocked);
            }


            placeInfo = isUnlocked ? placeInfoUnlock : placeInfoLock;
            gameObject.SetActive(true);
            placeInfo.Open(OnBtnCloseClicked, OnBtnScanClicked);
        }

        private async void HidePopup()
        {
            OnClosePlaceInfoListener?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnBtnCloseClicked()
        {
            HidePopup();
        }

        private void OnBtnScanClicked()
        {
            OnScanClickedListener?.Invoke();
            gameObject.SetActive(false);
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
