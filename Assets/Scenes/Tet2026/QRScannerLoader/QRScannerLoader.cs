using System;
using TreasureHunt.MenuGame;
using UnityEngine;

namespace TreasureHunt.QRScanner
{

    public class QRScannerLoader : MonoBehaviour
    {
        public static QRScannerLoader Instance;
        [SerializeField] private QRScannerController qrScannerController;

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
            LoadQRScanner(jsonData);
        }

        private void LoadQRScanner(string jsonData)
        {
            qrScannerController.ShowQRScanner();
        }

        public void AddOnCloseQRScannerListener(Action listener)
        {
            qrScannerController.AddOnCloseQRScannerListener(listener);
        } 
    }

}
