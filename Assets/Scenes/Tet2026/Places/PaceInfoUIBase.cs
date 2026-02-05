using System;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.Places
{

    public class PaceInfoUIBase : MonoBehaviour
    {
        private Action OnBtnOkClickedListener;
        private Action OnBtnQRClickedListener;
        private Action OnBtnBackClickedListener;
        private Action OnBtnNextClickedListener;

        [SerializeField] Button m_OkButton;
        [SerializeField] Button m_QRButton;
        [SerializeField] Button m_BackButton;
        [SerializeField] Button m_NextButton;

        void Start()
        {
            InitListener();
        }

        void InitListener()
        {
            m_OkButton.onClick.AddListener(OnBtnOkClicked);
            if (m_QRButton != null)
            {
                m_QRButton.onClick.AddListener(OnBtnQRClicked);
            }
            m_BackButton.onClick.AddListener(OnBtnBackClicked);
            m_NextButton.onClick.AddListener(OnBtnNextClicked);
        }

        public void AddOnBtnOkClickedListener(Action listener)
        {
            OnBtnOkClickedListener -= listener;
            OnBtnOkClickedListener += listener;
        }

        public void AddOnBtnQRClickedListener(Action listener)
        {
            OnBtnQRClickedListener -= listener;
            OnBtnQRClickedListener += listener;
        }

        public void AddOnBtnBackClickedListener(Action listener)
        {
            OnBtnBackClickedListener -= listener;
            OnBtnBackClickedListener += listener;
        }

        public void AddOnBtnNextClickedListener(Action listener)
        {
            OnBtnNextClickedListener -= listener;
            OnBtnNextClickedListener += listener;
        }

        void OnBtnOkClicked()
        {
            OnBtnOkClickedListener?.Invoke();
        }

        void OnBtnQRClicked()
        {
            OnBtnQRClickedListener?.Invoke();
        }

        void OnBtnBackClicked()
        {
            OnBtnBackClickedListener?.Invoke();
        }

        void OnBtnNextClicked()
        {
            OnBtnNextClickedListener?.Invoke();
        }


        public void CheckNextBack(int pageId, int N_Page)
        {
            try
            {
                m_NextButton.gameObject.SetActive(pageId < N_Page - 1);
                m_BackButton.gameObject.SetActive(pageId > 0);
            }
            catch (Exception exception)
            {
                Debug.LogError($"PaceInfoUIBase CheckNextBack exception: {exception.Message}");
            }
        }

        public void SetONOFFQRBtn(bool isOn)
        {
            if (m_QRButton != null)
            {
                m_QRButton.gameObject.SetActive(isOn);
            }
        }
    }

}