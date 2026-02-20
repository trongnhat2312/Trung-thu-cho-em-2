using System;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.Popup
{
    public class EventCompletedPopup : MonoBehaviour
    {
        private Action OnClosePopupListener;
        [SerializeField] Button btnOk;


        void Start()
        {
            btnOk.onClick.AddListener(OnBtnOkClicked);
        }

        private void OnBtnOkClicked()
        {
            OnClosePopupListener?.Invoke();
        }

        public void ShowPopup(Action pCallback)
        {
            OnClosePopupListener = pCallback;
            gameObject.SetActive(true);
        }
    }
}
