using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using TreasureHunt.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.MenuGame
{
    public class MenuCenterController : MonoBehaviour
    {
        private Action<int> OnNeedShowPlaceInfoListener;
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
            RefreshUI();
        }

        public void RefreshUI()
        {
            showMapPieces();
        }

        public void SetUsername()
        {
            string userName = DataManager.UserName;
            if (!string.IsNullOrEmpty(userName))
            {
                txtUsername.text = userName;
            }
        }

        #region Listener
        private void InitListener()
        {
            menuRoadMap.AddOnPlaceClickedListener(OnPlaceClicked);
            btnScanner.onClick.AddListener(OnBtnScannerClicked);
        }

        public void AddOnNeedShowPlaceInfoListener(Action<int> pListener)
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

            //new 
            int placeId = placeNum;
            OnNeedShowPlaceInfoListener?.Invoke(placeId);
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

        public void showMapPieces()
        {
            for (int i = 0; i < StaticParamClass.IsMapUnlocked.Length; i++)
            {
                bool isPlaceUnlocked = DataManager.IsPlaceUnlocked(i);
                if (isPlaceUnlocked)
                {
                    try
                    {
                        mapPieces[i].GetComponent<UITransitionEffect>().effectFactor = 0;
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"MenuCenterController showMapPieces {exception.Message}");
                    }

                    try
                    {
                        MapCheckpointBase checkpointBase = mapPieces[i].GetComponent<MapCheckpointBase>();
                        if (checkpointBase != null)
                        {
                            checkpointBase.SetActiveState(true);
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"MenuCenterController showMapPieces {exception.Message}");
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
            OpenPlaceInfo(placeNum);

        }
    }
}
