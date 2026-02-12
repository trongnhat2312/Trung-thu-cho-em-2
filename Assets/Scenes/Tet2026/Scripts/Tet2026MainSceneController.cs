using Koi.Scene; 
using UnityEngine; 
using TreasureHunt.FirstLoad;
using TreasureHunt.MenuGame;
using TreasureHunt.QRScanner;

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
            Debug.LogError("MainSceneController OnFirstLoaded");
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
