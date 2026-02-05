using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.MenuGame
{
    public class MenuCenterController : MonoBehaviour
    {
        private Action<int, bool> OnNeedShowPlaceInfoListener;
        private Action OnBtnScannerClickedListener;

        public List<GameObject> mapPieces;
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
            Debug.LogError($"MenuCenterController OnPlaceClicked {placeId}");
            OpenPlaceInfo(placeId);
        }

        private void OnBtnScannerClicked()
        {
            OnBtnScannerClickedListener?.Invoke();
        }
        #endregion


        public void OpenPlaceInfo(int placeNum)
        {

            Debug.LogError($"MenuCenterController StaticParamClass.GoFromInside {StaticParamClass.GoFromInside}, place == " + placeNum);
            if (StaticParamClass.GoFromInside)
                return;
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

        public void showMapPieces()
        {
            for (int i = 0; i < StaticParamClass.IsMapUnlocked.Length; i++)
            {
                if (StaticParamClass.IsMapUnlocked[i])
                {
                    try
                    {
                        mapPieces[i].GetComponent<UITransitionEffect>().effectFactor = 0;
                    }
                    catch (Exception exception)
                    {
                    }

                    try
                    {
                        //var child = mapPieces[i].transform.GetChild(0);
                        //mapPieces[i].GetComponent<UITransitionEffect>().effectFactor = 0;
                        //var image = child.GetComponent<Image>();
                        //image.color = Color.white;
                        MapCheckpointBase checkpointBase = mapPieces[i].GetComponent<MapCheckpointBase>();
                        if (checkpointBase != null)
                        {
                            checkpointBase.SetActiveState(true);
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
            if (IsAllMapUnlocked())
            {
                btnScanner.gameObject.SetActive(false);
            }

        }

        public IEnumerator OpenPlaceInfoWithEffect(int placeNum)
        {
            Debug.Log("place == " + placeNum);
            if (placeNum == -1)
                yield break;
            if (placeNum < 0 || placeNum > StaticParamClass.MAX_PLACE - 1)
            {
                Debug.LogError("MenuGameController OnpenPlaceInfoWithEffect Place number out of range [0, MAX_PLACE - 1]");
                yield break;
            }

            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.pieceDisappear);
            if (mapPieces != null && mapPieces.Count > placeNum && mapPieces[placeNum] != null)
            {
                var effect = mapPieces[placeNum].GetComponent<UITransitionEffect>();
                if (effect != null)
                {
                    effect.Hide(false);
                    yield return new WaitForSeconds(effect.effectPlayer.duration);
                }
            }


            yield return new WaitForSeconds(0.5f);
            StaticParamClass.GoFromInside = false;
            OpenPlaceInfo(placeNum);

        }
    }
}
