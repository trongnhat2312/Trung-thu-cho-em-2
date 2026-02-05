using System;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.MenuGame
{
    public class MenuCenterController : MonoBehaviour
    {
        private Action<int, bool> OnNeedShowPlaceInfoListener;
        private Action OnBtnScannerClickedListener;

        [SerializeField] Text txtUsername;
        [SerializeField] Button btnScanner;
        [SerializeField] MenuRoadMapController menuRoadMap;


        private void Start()
        {
            InitListener();
        }

        public void ShowUI()
        {
        }

        public void SetUsername()
        {
            if (PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName))
            {
                txtUsername.text = PlayerPrefs.GetString(StaticParamClass.PrefCheckinName);
            }
        }

        #region Listener
        private void InitListener()
        {
            menuRoadMap.AddOnPlaceClickedListener(OnPlaceClicked);
            btnScanner.onClick.AddListener(OnBtnScannerClicked);
        }

        public void AddOnNeedShowPlaceInfoListener(Action<int, bool> pListener)
        {
            OnNeedShowPlaceInfoListener -= pListener;
            OnNeedShowPlaceInfoListener += pListener;
        }

        public void AddOnBtnScannerClickedListener(Action pListener)
        {
            OnBtnScannerClickedListener -= pListener;
            OnBtnScannerClickedListener += pListener;
        }

        private void OnPlaceClicked(int placeId)
        {
            OpenPlaceInfo(placeId);
        }

        private void OnBtnScannerClicked()
        {
            OnBtnScannerClickedListener?.Invoke();
        }
        #endregion


        public void OpenPlaceInfo(int placeNum)
        {

            if (StaticParamClass.GoFromInside)
                return;
            Debug.Log("place == " + placeNum);
            if (placeNum == -1)
                return;
            if (placeNum < 0 || (placeNum > StaticParamClass.MAX_PLACE - 1))
            {
                Debug.LogError("Place number out of range [0, MAX_PLACE - 1]");
                return;
            }
            if (IsAllMapUnlocked())
            {
                // todo - dont need to show. or must show then close then show completed anim
                // return;
            }

            // PlaceInfo = Instantiate(PlaceInfoPrefab);
            // PlaceInfo.transform.SetParent(MainScreen.transform.parent, false);
            // PlaceInfo.name = "Place Info";
            // PlaceInfo.GetComponent<PlaceInfoHolder>().OpenPlaceInfo(placeNum, StaticParamClass.IsMapUnlocked[placeNum], null, () =>
            // {
            //     Debug.Log($"MainController: place == {placeNum}, close and open QR");
            //     // process open qr here
            //     ClickScan();
            // });


            //new 
            int placeId = placeNum;
            bool isPlaceUnlocked = StaticParamClass.IsMapUnlocked[placeId];
            OnNeedShowPlaceInfoListener?.Invoke(placeId, isPlaceUnlocked);
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

    }
}
