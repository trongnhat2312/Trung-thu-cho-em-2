using System;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.MenuGame
{

    public class MenuRoadMapController : MonoBehaviour
    {

        private Action<int> OnPlaceClickedListener;

        [SerializeField] AudioSource audioSource;
        [SerializeField] Button btnPlace_1;
        [SerializeField] Button btnPlace_2;
        [SerializeField] Button btnPlace_3;
        [SerializeField] Button btnPlace_4;
        [SerializeField] Button btnPlace_5;
        [SerializeField] Button btnPlace_6;


        void Start()
        {
            InitListener();
        }

        #region Listener
        private void InitListener()
        {
            btnPlace_1.onClick.AddListener(OnBtnPlace1Clicked);
            btnPlace_2.onClick.AddListener(OnBtnPlace2Clicked);
            btnPlace_3.onClick.AddListener(OnBtnPlace3Clicked);
            btnPlace_4.onClick.AddListener(OnBtnPlace4Clicked);
            btnPlace_5.onClick.AddListener(OnBtnPlace5Clicked);
            btnPlace_6.onClick.AddListener(OnBtnPlace6Clicked);
        }

        public void AddOnPlaceClickedListener(Action<int> pListener)
        {
            OnPlaceClickedListener -= pListener;
            OnPlaceClickedListener += pListener;
        }

        private void OnBtnPlace1Clicked()
        {
            OnBtnClickedInteract();
            OnPlaceClicked(1);
        }

        private void OnBtnPlace2Clicked()
        {
            OnBtnClickedInteract();
            OnPlaceClicked(2);
        }

        private void OnBtnPlace3Clicked()
        {
            OnBtnClickedInteract();
            OnPlaceClicked(3);
        }

        private void OnBtnPlace4Clicked()
        {
            OnBtnClickedInteract();
            OnPlaceClicked(4);
        }

        private void OnBtnPlace5Clicked()
        {
            OnBtnClickedInteract();
            OnPlaceClicked(5);
        }

        private void OnBtnPlace6Clicked()
        {
            OnBtnClickedInteract();
            OnPlaceClicked(6);
        }
        #endregion


        private void OnPlaceClicked(int placeId)
        {
            OnPlaceClickedListener?.Invoke(placeId);
        }

        private void OnBtnClickedInteract()
        { 
            // play SE 
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
            // play Vibrate
        }
    }
}
