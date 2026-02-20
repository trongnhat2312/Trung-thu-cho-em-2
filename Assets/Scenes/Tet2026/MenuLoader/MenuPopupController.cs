using System;
using TreasureHunt.Common;
using TreasureHunt.Places;
using Unity.VisualScripting;
using UnityEngine;

namespace TreasureHunt.MenuGame
{

    public class MenuPopupController : MonoBehaviour
    {
        [SerializeField] private PlaceInfoBase placeInfoHolderBase;
        
        public void ShowPlaceInfo(int pPlaceId, bool pIsPlaceUnlocked, Action pCallback = null, Action pCallbackScan = null)
        {
            CommonPopupManager.ShowPlaceInfoPopup(pPlaceId, pIsPlaceUnlocked, pCallback, pCallbackScan); 
        } 
    }
}
