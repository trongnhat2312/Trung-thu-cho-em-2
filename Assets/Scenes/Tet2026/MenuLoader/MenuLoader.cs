using Cysharp.Threading.Tasks; 
using System;
using UnityEngine;

namespace TreasureHunt.MenuGame
{
    public class MenuLoader : MonoBehaviour
    {
        public static MenuLoader Instance;
        [SerializeField] private MenuGameController menuNewController;

        public string jsonSceneData;

        private void Awake()
        {
            if (Instance == null)
            {
                if (transform.parent == null)
                {
                    Instance = this;
                    GameObject.DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }

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
            LoadMenu(jsonData);
        }

        private void LoadMenu(string jsonData)
        {
            menuNewController.ShowMenuUI(jsonData); 
        } 

        #region Listener 

        public void AddOnCloseQRScannerListener(Action<string> pListener)
        {
            menuNewController.AddOnCloseQRScannerListener(pListener);
        } 
        #endregion
    }
}
