using System;
using TreasureHunt.Places;
using Unity.VisualScripting;
using UnityEngine;

namespace TreasureHunt.MenuGame
{

    public class MenuPopupController : MonoBehaviour
    {
        [SerializeField] private PlaceInfoBase placeInfoHolderBase;
        
        public void ShowPlaceInfo1(int pPlaceId, bool pIsPlaceUnlocked, Action pCallback = null, Action pOpenQRCallback = null)
        {
            placeInfoHolderBase.gameObject.SetActive(true);
            placeInfoHolderBase.OpenPlaceInfo(pPlaceId, pIsPlaceUnlocked, pCallback);
        }

        public void ShowScanQR()
        { 
        }
    }
}
