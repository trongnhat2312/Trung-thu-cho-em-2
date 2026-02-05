using System;
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


        public static void ShowCheckInPopup(Action pCallback)
        {
            Instance.checkInPopup.ShowPopup(pCallback);
        }
    }
}
