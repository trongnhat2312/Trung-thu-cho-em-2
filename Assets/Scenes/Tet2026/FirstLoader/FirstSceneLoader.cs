using Cysharp.Threading.Tasks; 
using System;
using UnityEngine;

namespace TreasureHunt.FirstLoad
{
    public class FirstSceneLoader : MonoBehaviour
    {

        public string jsonSceneData;

        private Action OnCompletedFirstLoadListener;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Time.timeScale = 1.0f;
        }

        public void SetupDataLoaded(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
            }
            else
            {
                jsonSceneData = jsonData;
            } 
            CheckServiceReady();
        } 

        public void AddOnCompletedFirstLoadListener(Action pListener)
        {
            OnCompletedFirstLoadListener -= pListener;
            OnCompletedFirstLoadListener += pListener;
        }

        private async void CheckServiceReady()
        {
            try
            {
                bool isGameReady = IsGameReady();
                int count = 0;
                while (isGameReady == false && count < 30)
                {
                    count++;
                    await UniTask.DelayFrame(1);
                    //Debug.LogError($"FirstSceneLoaded after await 1 frame");
                    isGameReady = IsGameReady();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"FirstSceneLoader CheckServiceReady exception: {e.Message}");
            }

            OnCompletedFirstLoadListener?.Invoke();
        }

        private bool IsGameReady()
        {
            bool isUIDataManagerReady = true;//UIDataManager.IsInited;
            bool isDataManagerReady = true;//DataManager.IsAvailable;
            bool isDataReady = isUIDataManagerReady && isDataManagerReady;

            //Check Resource ready  
            bool isResourceReady = true;

            //Check allReady
            //Debug.LogError("FirstSceneLoader checkAllReady: data: " + isDataReady + "::resource:" + isResourceReady);
            bool isGameReady = isDataReady && isResourceReady;
            //ToastUI.ShowToast($"FirstSceneLoaded isGameReady ={isGameReady}_ isUIDataManagerReady={isUIDataManagerReady}_ isDataManagerReady={isDataManagerReady}_ isDinoTypeManagerReady={isDinoTypeManagerReady}");
            //Debug.LogError("FirstSceneLoaded: isGameReady:" + isGameReady);
            return isGameReady;
        } 
    }
}