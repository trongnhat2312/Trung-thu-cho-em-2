
using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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

        public GameObject ScanButton;
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

            if (pm != -1 && !StaticParamClass.DaCheckRoi)
            {
                StaticParamClass.GoFromOutside = true;
                StaticParamClass.CheckinPlace = Int32.Parse(absoluteURL.Split("=")[1]);
                Debug.LogError($"MenuGameController: SetupStart First time from Url QR {pm}, checkInPlace {StaticParamClass.CheckinPlace}");

                StaticParamClass.IsMapUnlocked[StaticParamClass.CheckinPlace] = true;
                Console.WriteLine("Set out: " + StaticParamClass.GoFromOutside);
                Console.WriteLine("Set check: " + StaticParamClass.CheckinPlace);
                Debug.LogError($"MenuGameController: SetupStart setout: {StaticParamClass.GoFromOutside}, setcheck: {StaticParamClass.CheckinPlace}");
                OnShowQRScanner();
                return;
            }

            _isStarEffEnabled = true;
            //_isPopupOpen = true;
                Debug.Log($"MenuGameController: SetupStart Go From InSide?? = {StaticParamClass.GoFromInside}");
            if (StaticParamClass.GoFromInside)
            {
                Debug.Log($"MenuGameController: SetupStart Go From InSide, show map piece, place info...");
                menuCenter.showMapPieces();
                StartCoroutine(menuCenter.OpenPlaceInfoWithEffect(StaticParamClass.CheckinPlace));
            }
            else
            {
                Debug.Log($"MenuGameController: SetupStart Go From OutSide... Check Account and user");
                //Check tai khoan
                StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
            }
            menuCenter.SetUsername();

            _starLightCount = 0;
        }


        public IEnumerator GetData(string name)
        {
            Debug.Log($"MenuGameController: do get data by name {name}");
            SetGetUserData.GetCheckedinPlace_(name, getCheckIn);
            yield return null;
        }

        public void getCheckIn(string a, string name)
        {
            StaticParamClass.CheckedIn = a;
            Debug.Log($"MenuGameController: get Data result: {StaticParamClass.IsMapUnlocked.Length}");
            for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
            {
                if (a.Contains(i.ToString()))
                {
                    Debug.Log("MenuGameController: Come here moi dung: " + i);
                    StaticParamClass.IsMapUnlocked[i] = true;
                }
            }
            menuCenter.showMapPieces();
        }

        public bool IsAllMapUnlocked()
        {
            bool b = true;
            for (int i = 0; i < StaticParamClass.IsMapUnlocked.Length; i++)
            {
                if (!StaticParamClass.IsMapUnlocked[i])
                {
                    //Debug.Log(i);
                    b = false;
                    break;
                }
            }
            return b;
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
        }

        private void OnPlayButtonClicked(int pLevelPlaying)
        {
            if (IsMenuStatePause)
            {
                return;
            }

            PlayNormalLevel(pLevelPlaying);
        }

        private void PlayNormalLevel(int pLevelPlaying)
        {
            // int maxLevelNormalActived = DataManager.nLevelActive;
            // MemeType memeTypeIntro = MemeUtils.GetRandomMemeType(DataManager.MEMETypeUnlockeds);
            // string jsonData =
            //     IngameLoaderUtils.GenJsonIngameLoaderData(playingLevel, maxLevelNormalActived, memeTypeIntro);
            // OnPlayNormalLevelListener?.Invoke(jsonData);
        }




        private void ShowPlaceInfo(int pPlaceId, bool pIsPlaceUnlocked)
        {
            Debug.LogError($"MenuGameController ShowPlaceInfo pPlaceId {pPlaceId} pIsPlaceUnlocked {pIsPlaceUnlocked}");
            //old code show place
            menuPopup.ShowPlaceInfo1(pPlaceId, pIsPlaceUnlocked, OnBtnCloseInPlaceClicked, OnScanCallbackInPlaceInfo);

            //new code 
            // switch (pPlaceId)
            // {
            //     case 1:
            //         menuPopup.ShowPlaceInfo1(pPlaceId, pIsPlaceUnlocked, OnBtnCloseInPlaceClicked, OnScanCallbackInPlaceInfo);
            //         break;
            // }
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
            StaticParamClass.GoFromOutside = false;
            QRTranferData qRTranferData = new QRTranferData();
            qRTranferData.isGoInside = false;

            string jsonData = JsonUtility.ToJson(qRTranferData);
            Debug.LogError($"MenuGameController OnShowQRScanner GoFromInside {StaticParamClass.GoFromInside }");
            OnNeedOpenQRScannerListener?.Invoke(jsonData);
        }




        bool IsConnectNetwork => Application.internetReachability != NetworkReachability.NotReachable;
        private bool IsMenuStatePause => menuState == MenuState.S2_Pause;



    }

[Serializable]
    public class QRTranferData
    {
       public bool isGoInside = false;
    }
}