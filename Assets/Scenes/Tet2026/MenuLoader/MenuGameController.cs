
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TreasureHunt.Data;
using UnityEngine;

namespace TreasureHunt.MenuGame
{
    public class MenuGameController : MonoBehaviour
    {
        public enum MenuState
        {
            S0_InActive = 0,
            S1_Active = 1,
            S2_Pause = 2
        }

        Action<string> OnNeedOpenQRScannerListener;

        public MenuState menuState;
        [SerializeField] private MenuCenterController menuCenter;
        [SerializeField] private MenuPopupController menuPopup;


        public bool debugStartFromUrlQR = false;
        public string debugAbsoluteURL = "https://koikinggaming.com/vme/Tet-2025/?CheckinPlace=1";

        //public StarLightTransformer starLight;
        //public ParallelSentencesController parallelSentence;
        public GameObject ThachSanhVictory;
        public List<GameObject> m_MenuBaseComponents;

        public float StarLightInterval = 10f;
        private float _starLightCount;
        private bool _alreadyResetStarLight;
        private bool _isStarEffEnabled = true;
        private bool _isPopupOpen = true;
        private bool _isPlayedOnce = false;

        #region  Start
        private void Start()
        {
            InitListener();
        }

        void CheckCompletedChallenge()
        {
            // if (StaticParamClass.GoFromInside)
            // {
            //     return;
            // }
            if (IsAllMapUnlocked())
            {
                Debug.Log($"MenuGameController: update: all map unlocked => update");
                if (_isStarEffEnabled)
                {
                    //Debug.Log("_starLightCount:" + _starLightCount);
                    if (_starLightCount <= 0)
                    {
                        Debug.Log($"MenuGameController: update: show Victory");
                        //starLight.DoTransformToStarLight();
                        //parallelSentence.OnCompleted();
                        ThachSanhVictory.gameObject.SetActive(true);
                        SetBaseMenuComponentVisible(false);
                        _alreadyResetStarLight = false;
                        _starLightCount = StarLightInterval;
                    }
                    else
                    {
                        if (_starLightCount > 1)
                        {
                            _starLightCount -= Time.deltaTime;
                        }
                        if (_starLightCount <= 1)
                        {
                            _isPlayedOnce = true;
                        }
                    }
                }

                if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && _isPlayedOnce)
                {
                    //parallelSentence.ResetBeforeTransform();
                    //ThachSanhVictory.gameObject.SetActive(false);
                    _alreadyResetStarLight = true;
                    _isStarEffEnabled = false;

                }
            }
        }

        void SetBaseMenuComponentVisible(bool isVisible)
        {
            foreach (var pGameObject in m_MenuBaseComponents)
            {
                try
                {
                    if (pGameObject != null)
                    {
                        pGameObject.SetActive(isVisible);
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        public void ShowMenuUI(string data)
        {
            UpdateMenuState(MenuState.S1_Active);
            menuCenter.ShowUI();
            SetupStart();
        }

        protected virtual async void SetupStart()
        {
            await UniTask.DelayFrame(3);
#if !UNITY_EDITOR
		debugStartFromUrlQR = false;
#endif

            // Main load. kiem tra xem user da co tai khoan va checkin ở địa điểm nào chưa
            // Nếu đã có tài khoản: thực hiện checkin/set place num các thứ
            // Nếu chưa có tài khoản: load checkin scene để nó checkin.
            string absoluteURL = Application.absoluteURL;
#if UNITY_EDITOR
            if (debugStartFromUrlQR)
            {
                absoluteURL = debugAbsoluteURL;
            }
#endif

            int pm = absoluteURL.IndexOf("CheckinPlace");
            Debug.LogError($"MenuGameController pmId {pm}, Set DaCheckRoi: " + StaticParamClass.DaCheckRoi);
 

            _isStarEffEnabled = true;
            //_isPopupOpen = true;
            Debug.Log($"MenuGameController: SetupStart Go From InSide?? = {StaticParamClass.GoFromInside}");
            if (!IsAllMapUnlocked())
            {
                Debug.Log($"MenuGameController: SetupStart Go From InSide, show map piece, place info...");
                menuCenter.showMapPieces();
                // StartCoroutine(menuCenter.OpenPlaceInfoWithEffect(StaticParamClass.CheckinPlace));
            } 

            menuCenter.SetUsername();

            _starLightCount = 0;
            CheckCompletedChallenge();
        }  

        public bool IsAllMapUnlocked()
        {
            bool result = false;
            try
            {
                bool isPlace1Unlocked = DataManager.IsPlaceUnlocked((int)PlaceID.Place_01_Place1);
                bool isPlace2Unlocked = DataManager.IsPlaceUnlocked((int)PlaceID.Place_02_Place2);
                bool isPlace3Unlocked = DataManager.IsPlaceUnlocked((int)PlaceID.Place_03_Place3);
                bool isPlace4Unlocked = DataManager.IsPlaceUnlocked((int)PlaceID.Place_04_Place4);
                bool isPlace5Unlocked = DataManager.IsPlaceUnlocked((int)PlaceID.Place_05_Place5);
                result = isPlace1Unlocked && isPlace2Unlocked && isPlace3Unlocked && isPlace4Unlocked && isPlace5Unlocked;
            }
            catch (Exception exception)
            {
                Debug.LogError($"MenuCenterController IsAllMapUnlocked {exception.Message}");
            }

            return result;
        }

        #endregion

        private void UpdateMenuState(MenuState state)
        {
            menuState = state;
        }

        private void InitListener()
        {
            menuCenter.AddOnNeedShowPlaceInfoListener(ShowPlaceInfo);
            menuCenter.AddOnBtnScannerClickedListener(OnBtnScannerInMenuCenterClicked);
        }

        #region Listener: add, callback

        public void AddOnCloseQRScannerListener(Action<string> pListenter)
        {
            OnNeedOpenQRScannerListener -= pListenter;
            OnNeedOpenQRScannerListener += pListenter;
        }

        #endregion


        private void RefreshUI()
        {
            menuCenter.RefreshUI();
        }   

        private void ShowPlaceInfo(int pPlaceId)
        {
            bool isPlaceUnlocked = DataManager.IsPlaceUnlocked(pPlaceId);
            Debug.LogError($"MenuGameController ShowPlaceInfo pPlaceId {pPlaceId} pIsPlaceUnlocked {isPlaceUnlocked}");
            //old code show place
            menuPopup.ShowPlaceInfo(pPlaceId, isPlaceUnlocked, OnBtnCloseInPlaceClicked, OnScanCallbackInPlaceInfo);
        }

        private void OnBtnCloseInPlaceClicked()
        {
            Debug.LogError($"MenuGameController OnBtnCloseInPlaceClicked");
            RefreshUI();
        }

        public void OnScanCallbackInPlaceInfo()
        {
            Debug.LogError($"MenuGameController OnScanCallbackInPlaceInfo");
            // ClickScan();
            OnShowQRScanner();
        }

        private void OnBtnScannerInMenuCenterClicked()
        {
            Debug.LogError($"MenuGameController OnBtnScannerInMenuCenterClicked");
            OnShowQRScanner();
        }

        private void OnShowQRScanner()
        {
            Debug.LogError($"MenuGameController OnShowQRScanner");
            QRTranferData qRTranferData = new QRTranferData();
            qRTranferData.isGoInside = false;

            string jsonData = JsonUtility.ToJson(qRTranferData);
            Debug.LogError($"MenuGameController OnShowQRScanner GoFromInside {StaticParamClass.GoFromInside}");
            OnNeedOpenQRScannerListener?.Invoke(jsonData);
        }  

    }

    [Serializable]
    public class QRTranferData
    {
        public bool isGoInside = false;
    }
}