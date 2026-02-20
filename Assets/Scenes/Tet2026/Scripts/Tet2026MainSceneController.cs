using Koi.Scene;
using UnityEngine;
using TreasureHunt.FirstLoad;
using TreasureHunt.MenuGame;
using TreasureHunt.QRScanner;
using TreasureHunt.Data;

namespace TreasureHunt
{
    public class MainSceneController : BaseSceneController
    {
        public enum MainUIState
        {
            S0_FirstLoad = 0,
            S1_Menu = 1,
            S2_QRScanner = 2,
        }

        public MainUIState mainUIState;
        public FirstSceneLoader objFirstLoad;
        public QRScannerLoader objQRScanner;
        public MenuLoader objMenu;
        public bool debugStartFromUrlQR = false;
        public string debugAbsoluteURL = "https://koikinggaming.com/vme/Tet-2025/?CheckinPlace=1";


        private void Start()
        {
            Debug.LogError("MainSceneController Start");
            Application.targetFrameRate = 60;
            Time.timeScale = 1.0f;

            InitFirstRootObj("");
        }

        private void InitFirstRootObj(string jsonData)
        {
            Debug.LogError("MainSceneController InitFirstRootObj");
            mainUIState = MainUIState.S0_FirstLoad;
            ActiveUIWithState();
            objFirstLoad.AddOnCompletedFirstLoadListener(OnFirstLoaded);//Force addListener after call setup data
            objFirstLoad.SetupDataLoaded(jsonData);
        }

        private async void OnFirstLoaded()
        {

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
            Debug.LogError($"Tet2026MainSceneController pmId {pm}");

            bool isHasAccount = DataManager.IsCheckInDone;
            bool isPlace0IntroUnlocked = DataManager.IsPlaceUnlocked((int) PlaceID.Place_00_IntroEvent);//dung ham nay de check xem da checkin dia dien n chua, voi n la int
            bool case1Accept = pm != -1 && !isHasAccount;
            bool case2Accept = pm != -1 && isHasAccount;
            if (case1Accept)
            {
                Debug.LogError($"Tet2026MainSceneController:CASE 1 SetupStart First time from Url QR {pm}, case1Accept: {case1Accept} isHasAccount: {isHasAccount}");
                InitQRScannerRootObj("");
                return;
            }
            else if (case2Accept)
            {
                Debug.LogError($"Tet2026MainSceneController:CASE 1 SetupStart First time from Url QR {pm}, case1Accept: {case1Accept} isHasAccount: {isHasAccount}");
                InitQRScannerRootObj("");
                return;
            }

            Debug.LogError("Tet2026MainSceneController OnFirstLoaded");
            string jsonData = "";
            //process data firstLoad
            InitMenuRootObj(jsonData);
        }

        #region Menu

        private void InitMenuRootObj(string jsonData)
        {
            Debug.LogError("MainSceneController InitMenuRootObj");
            mainUIState = MainUIState.S1_Menu;
            ActiveUIWithState();
            //Start setup UI
            objMenu.SetupDataLoaded(jsonData);
            //Add Listener
            objMenu.AddOnCloseQRScannerListener(OnNeedOpenQRScanner);
        }

        async void OnNeedOpenQRScanner(string jsonData)
        {
            Debug.LogError($"MainSceneController OnNeedOpenQRScanner");
            // load ingame with data
            InitQRScannerRootObj(jsonData);
        }

        #endregion 

        #region QR Scanner
        private async void InitQRScannerRootObj(string jsonData)
        {
            Debug.LogError("MainSceneController InitMenuRootObj");
            mainUIState = MainUIState.S2_QRScanner;
            ActiveUIWithState();
            //Start setup UI
            objQRScanner.SetupDataLoaded(jsonData);
            //Add Listener
            objQRScanner.AddOnCloseQRScannerListener(OnCloseQRScanner);
        }

        private async void OnCloseQRScanner()
        {
            Debug.LogError($"MainSceneController OnCloseQRScanner");
            //back to menu
            InitMenuRootObj("");

        }
        #endregion

        private void ActiveUIWithState()
        {
            switch (mainUIState)
            {
                case MainUIState.S0_FirstLoad:
                    objFirstLoad.gameObject.SetActive(true);
                    objMenu.gameObject.SetActive(false);
                    objQRScanner.gameObject.SetActive(false);
                    break;
                case MainUIState.S1_Menu:
                    objFirstLoad.gameObject.SetActive(false);
                    objMenu.gameObject.SetActive(true);
                    objQRScanner.gameObject.SetActive(false);
                    break;
                case MainUIState.S2_QRScanner:
                    objFirstLoad.gameObject.SetActive(false);
                    objMenu.gameObject.SetActive(false);
                    objQRScanner.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
