using Koi.Scene;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TreasureHunt.FirstLoad;
using TreasureHunt.MenuGame;
using TreasureHunt.QRScanner;
#if UNITY_IOS
using Unity.Notifications.iOS;
using Cysharp.Threading.Tasks; 
#endif

namespace TreasureHunt
{
    public class MainSceneController : BaseSceneController
    {
        public enum ScreenType
        {
            S0_FirstLoad = 0,
            S1_Menu = 1,
            S2_QRScanner = 2,
            S3_IngameTutorial = 3,
            S4_IngameCalendar = 4,
            S5_Ingame3DFill = 5,
        }

        public ScreenType curScreen;
        [Header("Prefab FirstLoad")]
        [SerializeField] private GameObject prefabFirstLoad;
        [Header("Prefab Ingame")]
        [SerializeField] private GameObject prefabQRScanner;
        [SerializeField] private GameObject prefabIngameTutorial;
        [SerializeField] private GameObject prefabIngameCalendar;
        [SerializeField] private GameObject prefabIngameDaily3DFill;

        [Header("Prefab Menu")]
        [SerializeField] private GameObject prefabMenu;

        public bool isLoadFirstLoad = true;

        [Header("Debug gameObj")]
        public List<GameObject> listEditorOnlys;
        public GameObject objFirstLoad;
        public GameObject objQRScanner;
        public GameObject objIngameTutorial;
        public GameObject objIngameCalendar;
        public GameObject objIngameDaily3DFill;
        public GameObject objMenu;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Time.timeScale = 1.0f;
            
            //only 1 scene -> call setup data
            SetupData("");
        }

        public override void SetupData(string jsonData)
        {
            Debug.LogError($"Tet2026MainSceneController SetupData");
            for (int i = 0; i < listEditorOnlys.Count; i++)
            {
                DestroyObjIfExist(listEditorOnlys[i]);
            }
            if (isLoadFirstLoad)
            {
                InitFirstRootObj(jsonData);
            }
            else
            {
                OnFirstLoaded();
            }
        }

        private void InitFirstRootObj(string jsonData)
        {
            // Debug.LogError("MainSceneController InitSceneRootObj");
            DestroyObjIfExist(objFirstLoad, true);
            objFirstLoad = Instantiate(prefabFirstLoad, transform);
            curScreen = ScreenType.S0_FirstLoad;
            FirstSceneLoader firstSceneLoader = objFirstLoad.GetComponent<FirstSceneLoader>();
            firstSceneLoader.AddOnCompletedFirstLoadListener(OnFirstLoaded);//Force addListener after call setup data
            firstSceneLoader.SetupDataLoaded(jsonData);
        }

        private async void OnFirstLoaded()
        {
            string jsonData = "";
            //process data firstLoad  
            InitMenuRootObj(jsonData);

            await UniTask.DelayFrame(1);
            await UniTask.Delay(200);
            Destroy(objFirstLoad);
        }

        #region Menu 

        private void InitMenuRootObj(string jsonData)
        {
            //Debug.LogError("MainSceneController InitMenuRootObj");
            //Init new Obj Menu
            //DestroyObjIfExist(objMenu, true);
            if (MenuLoader.Instance == null)
            {
                objMenu = Instantiate(prefabMenu);
            }
            else
            {
                MenuLoader.Instance.gameObject.SetActive(true);
            }

            MenuLoader menuLoader = objMenu.GetComponent<MenuLoader>();
            //Start setup UI
            menuLoader.SetupDataLoaded(jsonData);
            //Add Listener
            menuLoader.AddOnCloseQRScannerListener(OnNeedOpenQRScanner);
            //Change ScreenState
            curScreen = ScreenType.S1_Menu;
            //Saved ScreenType
            // UIDataManager.UpdateScreenType(curScreen);
        }

        async void OnNeedOpenQRScanner(string jsonData)
        {
            Debug.LogError($"MainSceneController OnNeedOpenQRScanner");
            // load ingame with data
            InitQRScannerRootObj(jsonData);

            await UniTask.DelayFrame(1);
            await UniTask.Delay(200);
            DestroyObjIfExist(objMenu);
        }

        #endregion

        private void InitIngameRootObj(string pData)
        {

        }

        #region QR Scanner
        private async void InitQRScannerRootObj(string jsonData)
        {
            //Debug.LogError("MainSceneController InitMenuRootObj"); 
            if (QRScannerLoader.Instance == null)
            {
                objQRScanner = Instantiate(prefabQRScanner);
            }
            else
            {
                QRScannerLoader.Instance.gameObject.SetActive(true);
            }

            QRScannerLoader qrScannerLoader = objQRScanner.GetComponent<QRScannerLoader>();
            //Start setup UI
            qrScannerLoader.SetupDataLoaded(jsonData);
            //Add Listener
            qrScannerLoader.AddOnCloseQRScannerListener(OnCloseQRScanner); 
            //Change ScreenState
            curScreen = ScreenType.S2_QRScanner;
        }

        private async void OnCloseQRScanner()
        {
            //back to menu
            InitMenuRootObj("");
            await UniTask.DelayFrame(1);
            await UniTask.Delay(200);
            DestroyObjIfExist(objQRScanner);

        } 
        #endregion


        private void DestroyObjIfExist(GameObject obj, bool isImmediate = false)
        {
            Debug.LogError("DestroyObjIfExist objName = " + obj.name);
            if (obj != null)
            {
                if (isImmediate)
                {
                    DestroyImmediate(obj);
                }
                else
                {
                    Destroy(obj);
                }
            }
        }
    }
}