using System;
using TreasureHunt.Places;
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
    }
}
