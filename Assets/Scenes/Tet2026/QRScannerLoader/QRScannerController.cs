using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarcodeScanner;
using BarcodeScanner.Scanner;
using Cysharp.Threading.Tasks;
using PlayFab.ServerModels;
using TreasureHunt.Common;
using TreasureHunt.Data;
using TreasureHunt.Places;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureHunt.QRScanner
{

    public class QRScannerController : MonoBehaviour
    {
        private Action OnCloseQRScannerListener;

        private IScanner BarcodeScanner;
        public Text TextHeader;
        public RawImage Image;
        public AudioSource Audio;
        [SerializeField] Button btnBack;
        [SerializeField] Button btnChangeCamera;
        private float RestartTime;
        public Text ListCamera;
        public Image fog;
        private bool isChange = false;

        // Disable Screen Rotation on that screen
        void Awake()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }

        void Start()
        {
            Debug.LogError($"QRScannerController: Start");
            btnBack.onClick.AddListener(OnBtnBackClicked);
            btnChangeCamera.onClick.AddListener(() =>
            {
                ChangeCamera(true);
            });

            //request camera permission
            RequestCameraPermissionIfNeed();
        }

        /// <summary>
        /// The Update method from unity need to be propagated
        /// </summary>
        void Update()
        {
            if (!isChange)
            {
                if (BarcodeScanner != null)
                {
                    // Debug.LogError($"QRScannerController: Update BarcodeScanner NULL = {BarcodeScanner == null}");
                    BarcodeScanner.Update();
                }
            }

            // Check if the Scanner need to be started or restarted
            if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
            {
                StartScanner();
                RestartTime = 0;
            }
        }

        #region request permission
        private void RequestCameraPermissionIfNeed()
        {
            StartCoroutine(DoRequestCameraPermissionIfNeed());
        }

        private IEnumerator DoRequestCameraPermissionIfNeed()
        {
            Debug.LogError("QRScannerController: Xin quyền Camera");
            // Yêu cầu quyền truy cập trước khi khởi tạo scan
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }

            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogError("QRScannerController: Đã có quyền Camera");
                // Gọi hàm khởi tạo ZXing ở đây
            }
            else
            {
                Debug.LogError("QRScannerController: Người dùng từ chối hoặc trình duyệt chặn Camera");
            }
        }
        #endregion

        public void AddOnCloseQRScannerListener(Action listener)
        {
            OnCloseQRScannerListener -= listener;
            OnCloseQRScannerListener += listener;
        }

        public async void ShowQRScanner()
        {
            //request camera permission
            RequestCameraPermissionIfNeed();

            fog.gameObject.SetActive(true);
            await UniTask.DelayFrame(1);
            fog.gameObject.SetActive(false);

            await UniTask.DelayFrame(1);

            if (StaticParamClass.GoFromOutside == true)
            {
	            // nếu là vào từ bên ngoài => kiểm tra xem login chưa???
	            StaticParamClass.DaCheckRoi = true;
	            Console.WriteLine($"QRScannerController: DaCheckRoi: " + StaticParamClass.DaCheckRoi);

	            ProcessScannedQR(StaticParamClass.CheckinPlace, true);
	            return;
            }


            await UniTask.DelayFrame(5);
            // Create a basic scanner
            BarcodeScanner = new Scanner();
            BarcodeScanner.Camera.Play();
            // ChangeCamera(false);

            // Display the camera texture through a RawImage
            BarcodeScanner.OnReady += (sender, arg) =>
            {
                Debug.LogError("QRScannerController: OnReady");
                // Set Orientation & Texture
                Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
                Image.transform.localScale = BarcodeScanner.Camera.GetScale();
                Image.texture = BarcodeScanner.Camera.Texture;

                // Keep Image Aspect Ratio
                var rect = Image.GetComponent<RectTransform>();
                var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

                RestartTime = Time.realtimeSinceStartup;
            };
        }

        async void ProcessScannedQR(int pIdPlace, bool fromOpenWeb = false)
        {
            Debug.LogError($"QRScannerController ProcessScannedQR pPlaceId = {pIdPlace} fromOpenWeb = {fromOpenWeb} ");
            //step1: check is Intro
            bool isNeedGuideIntroEvent = !DataManager.IsPlaceUnlocked((int)PlaceID.Place_00_IntroEvent);
            bool isScanIntroPlace = pIdPlace == (int)PlaceID.Place_00_IntroEvent;
            bool isNeedShowIntro = isNeedGuideIntroEvent || isScanIntroPlace;
            Debug.LogError($"QRScannerController ProcessScannedQR isNeedShowIntro = {isNeedShowIntro}  isScanIntroPlace = {isScanIntroPlace}  isNeedGuideIntroEvent = {isNeedGuideIntroEvent} placeId = {pIdPlace}");
            if (isNeedShowIntro)
            {
                //force show intro event
                int idPlaceIntro = (int)PlaceID.Place_00_IntroEvent;
                DataManager.UpdatePlaceUnlocked(idPlaceIntro);
                CommonPopupManager.ShowPlaceInfoPopup(idPlaceIntro, true, (() =>
                {
                    OnIntroPopupClose(pIdPlace);
                }), null);
                PushPlayfabCheckInPlace(idPlaceIntro);
                return;
            }

            Debug.LogError($"QRScannerController ProcessScannedQR placeId = {pIdPlace} not need show intro or checkin");
            //case 2: scan other place (not introEvent)
            ShowRewardPlacePopup(pIdPlace);

            // //step 2: check is login
            // if (IsSignedUp())
            // {
            //     // đã đăng ký => load data và xử lý sau khi load
            //     // Check in and go to Main;
            //     StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
            // }
            // else
            // {
            //     // nếu chưa lưu Checkin Name vào máy => là mới => intro => sau đó xem xét để chúc mừng
            //     int idPlaceIntro = 0;
            //     StaticParamClass.IsMapUnlocked[idPlaceIntro] = true;
            //     placeInfo = Instantiate(PlaceInfoPrefab, root);
            //     placeInfo.name = "Place Info";
            //     Debug.LogError($"QRScannerController ProcessScannedQR Show place, intro, completed with id = {idPlaceIntro}, isIDPlaceUnlock = {StaticParamClass.IsMapUnlocked[idPlaceIntro]}");
            //     placeInfo.GetComponent<PlaceInfoBase>().OpenPlaceInfo(idPlaceIntro, StaticParamClass.IsMapUnlocked[idPlaceIntro],
            //         () =>
            //         {
            //             Debug.Log($"QRScannerController ProcessScannedQR, Intro Done => Congrat");
            //             GotoCongrats(StaticParamClass.CheckinPlace);
            //             Destroy(placeInfo);
            //         });
            //     fog.gameObject.SetActive(false);
            // }
        }

        private void OnIntroPopupClose(int pIdPlace)
        {
            bool isSignedUp = DataManager.IsCheckInDone;
            if (!isSignedUp)
            {
                ShowCheckInPopup(() =>
                {
                    OnCheckInPopupClosed(pIdPlace);
                });
            }
            else
            {
                OnCheckInPopupClosed(pIdPlace);
            }
        }

        private void OnCheckInPopupClosed(int pIdPlace)
        {
            bool isPlaceIntroEvent = pIdPlace == (int)PlaceID.Place_00_IntroEvent;
            if (isPlaceIntroEvent)
            {
                DoBackToMenu();
            }
            else
            {
                ShowRewardPlacePopup(pIdPlace);
            }
        }

        private void ShowRewardPlacePopup(int pIdPlace)
        {
            //show reward place -> show place info
            CommonPopupManager.ShowRewardPlacePopup(pIdPlace, () =>
            {
                OnRewardPlaceClosed(pIdPlace);
            });
            PushPlayfabCheckInPlace(pIdPlace);
        }

        private void PushPlayfabCheckInPlace(int pIdPlace)
        {
            try
            {
                string sPhoneNumber = DataManager.PhoneNumber;
                string listStringCheckInPlace = "";
                List<int> listCheckInPlace = DataManager.PlaceUnlocked;
                for (int i = 0; i < listCheckInPlace.Count; i++)
                {
                    bool isFinal = i == listCheckInPlace.Count - 1;
                    listStringCheckInPlace += listCheckInPlace[i];
                    if (!isFinal)
                    {
                        listStringCheckInPlace += ";";
                    }
                }

                SetTitleDataRequest title = new()
                {
                    Key = sPhoneNumber,
                    Value = listStringCheckInPlace
                };

                SetGetUserData.SetCheckinPlace(title);
            }
            catch (Exception e)
            {
                Debug.LogError($"PushPlayfabCheckInPlace exception: {e.Message}");
            }
        }

        private void OnRewardPlaceClosed(int pIdPlace)
        {
            bool isCompletedFullEvent = false;
            if (isCompletedFullEvent)
            {
                CommonPopupManager.ShowEventCompletedPopup(() =>
                {
                    ShowPlaceInfoPopup(pIdPlace);
                });
            }
            else
            {
                DataManager.UpdatePlaceUnlocked(pIdPlace);
                ShowPlaceInfoPopup(pIdPlace);
            }
        }

        private void ShowPlaceInfoPopup(int pIdPlace)
        {
            CommonPopupManager.ShowPlaceInfoPopup(pIdPlace, true, () =>
            {
                DoBackToMenu();
            }, null);
        }


        private void DoBackToMenu()
        {
            StartCoroutine(StopCamera(() =>
            {
                OnCloseQRScannerListener?.Invoke();
                gameObject.SetActive(false);
            }));
        }

        private void ShowCheckInPopup(Action pCallback)
        {
            CommonPopupManager.ShowCheckInPopup(pCallback);
        }

        /// <summary>
        /// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
        /// </summary>
        private async void StartScanner()
        {
            Debug.LogError($"QRScannerController StartScanner");
            await UniTask.DelayFrame(3);
            foreach (WebCamDevice wd in WebCamTexture.devices)
            {
                ListCamera.text += wd.name + "-" + wd.isFrontFacing + "\n";
            }
            StaticParamClass.DaCheckRoi = true;
            BarcodeScanner.Scan((barCodeType, barCodeValue) =>
            {
                Debug.Log(barCodeType + " -- " + barCodeValue);

                Debug.LogError($"QRScannerController StartScanner barCodeType = {barCodeType} barCodeValue = {barCodeValue}");
                //if (TextHeader.text.Length > 250)
                //{
                //	TextHeader.text = "";
                //}
                //TextHeader.text += "Found: " + barCodeType + " / " + barCodeValue + "\n";
                String value = barCodeValue;
                string[] arrayV = null;
                if (value.Contains("CheckinPlace="))
                {
                    Debug.Log("CheckinPlace:  " + barCodeType + " -- " + barCodeValue);
                    BarcodeScanner.Stop();
                    arrayV = value.Split("&");

                    foreach (String d in arrayV)
                    {
                        //if(d.Contains("CheckinName"))
                        //{
                        //	StaticParamClass.CheckinName = d.Split("=")[1];
                        //}
                        //if(d.Contains("CheckinNumber"))
                        //{
                        //	StaticParamClass.CheckinNumber = d.Split("=")[1];
                        //}
                        int placeId = 0;
                        Debug.LogError($"QRScannerController StartScanner d = {d}");
                        if (d.Contains("CheckinPlace"))
                        {
                            // StaticParamClass.CheckinPlace = Int32.Parse(d.Split("=")[1]);
                            // StaticParamClass.IsMapUnlocked[StaticParamClass.CheckinPlace] = true;
                            placeId = Int32.Parse(d.Split("=")[1]);
                        }
                        ProcessScannedQR(placeId);

                        // xử lý thông tin sau khi nhận QR Code
                    }
                }
                else
                {
                    //TextHeader.text += "Error barcode: " + barCodeType + " / " + barCodeValue + "\n";
                    Debug.Log("Error barcode: " + barCodeType + " / " + barCodeValue + "\n");
                    StartScanner();
                }

#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
#endif
            });
        }



        private void ChangeCamera(bool playSE = true)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
            isChange = true;
            ScannerSettings cSetting = BarcodeScanner.Settings;
            string name = cSetting.WebcamDefaultDeviceName;
            StartCoroutine(StopCamera(() =>
            {
                ScannerSettings ss = new ScannerSettings(name);
                Debug.Log(ss.WebcamDefaultDeviceName);
                BarcodeScanner = new Scanner(ss);
                BarcodeScanner.Camera.Play();

                // Display the camera texture through a RawImage
                BarcodeScanner.OnReady += (sender, arg) =>
                {
                    // Set Orientation & Texture

                    Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
                    Image.transform.localScale = BarcodeScanner.Camera.GetScale();
                    Image.texture = BarcodeScanner.Camera.Texture;

                    // Keep Image Aspect Ratio
                    var rect = Image.GetComponent<RectTransform>();
                    var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

                    RestartTime = Time.realtimeSinceStartup;
                };

                //if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
                //{
                //	StartScanner();
                //	RestartTime = 0;
                isChange = false;
                Debug.Log(isChange);
                //}
            }));
        }


        protected bool IsTargetPlace(int placeId)
        {
            // fix cứng Tết 2025:
            // địa điểm số 0 không cần target
            return placeId != 0;
        }

        #region UI Buttons

        private void OnBtnBackClicked()
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
            // Try to stop the camera before loading another scene
            DoBackToMenu();
        }

        /// <summary>
        /// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
        /// Trying to stop the camera in OnDestroy provoke random crash on Android
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator StopCamera(Action callback)
        {
            // Stop Scanning
            //Image = null;
            if (BarcodeScanner != null)
            {
                BarcodeScanner.Destroy();
            }

            BarcodeScanner = null;

            // Wait a bit
            yield return new WaitForSeconds(0.1f);

            callback.Invoke();
        }

        #endregion
    }

}
