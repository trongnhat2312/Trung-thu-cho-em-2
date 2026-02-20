using System;
using TreasureHunt.Places;
using TreasureHunt.Popup;
using Unity.VisualScripting;
using UnityEngine;

namespace TreasureHunt.Common
{

    public class CommonPopupManager : MonoBehaviour
    {
        private static CommonPopupManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public static bool IsInited => Instance != null;


        [SerializeField] CheckInPopup checkInPopup;
        [SerializeField] PlaceInfoNew2026 introEventPopup;
        [SerializeField] RewardPlacePopup rewardPlacePopup;
        [SerializeField] PlaceInfoBase placeInfo01;
        [SerializeField] PlaceInfoBase placeInfo02;
        [SerializeField] PlaceInfoBase placeInfo03;
        [SerializeField] PlaceInfoBase placeInfo04;
        [SerializeField] PlaceInfoBase placeInfo05;
        [SerializeField] EventCompletedPopup eventCompletedPopup;


        public static void ShowCheckInPopup(Action pCallback)
        {
            Debug.LogError($"CommonPopupManager ShowCheckInPopup");
            Instance.checkInPopup.ShowPopup(pCallback);
        }

        public static void ShowIntroEventPopup(Action pCallback)
        {
            Debug.LogError($"CommonPopupManager ShowIntroEventPopup");
            Instance.introEventPopup.Open(pCallback);
        }

        public static void ShowRewardPlacePopup(int pIdPlace, Action pCallback)
        {
            Debug.LogError($"CommonPopupManager ShowRewardPlacePopup pIdPlace = {pIdPlace}");
            Instance.rewardPlacePopup.ShowPopup(pIdPlace, pCallback);
        }

        public static void ShowPlaceInfoPopup(int pIdPlace, bool isUnlocked, Action pCallback)
        {
            Debug.LogError($"CommonPopupManager ShowPlaceInfoPopup pIdPlace = {pIdPlace}, isUnlocked = {isUnlocked}");
            switch (pIdPlace)
            {
                case (int)PlaceID.Place_01_Place1:
                    Instance.placeInfo01.OpenPlaceInfo(pIdPlace, isUnlocked, pCallback);
                    break;
                case (int)PlaceID.Place_02_Place2:
                    Instance.placeInfo02.OpenPlaceInfo(pIdPlace, isUnlocked, pCallback);
                    break;
                case (int)PlaceID.Place_03_Place3:
                    Instance.placeInfo03.OpenPlaceInfo(pIdPlace, isUnlocked, pCallback);
                    break;
                case (int)PlaceID.Place_04_Place4:
                    Instance.placeInfo04.OpenPlaceInfo(pIdPlace, isUnlocked, pCallback);
                    break;
                case (int)PlaceID.Place_05_Place5:
                    Instance.placeInfo05.OpenPlaceInfo(pIdPlace, isUnlocked, pCallback);
                    break;
            }
        }

        public static void ShowEventCompletedPopup(Action pCallback)
        {
            Debug.LogError($"CommonPopupManager ShowEventCompletedPopup");
            Instance.eventCompletedPopup.ShowPopup(pCallback);
        }
    }
}
