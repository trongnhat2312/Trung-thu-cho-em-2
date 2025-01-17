using BarcodeScanner;
using BarcodeScanner.Scanner;
using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QRScanner : MonoBehaviour {

	public string[] words = new string[]
		{
			"Xuân",
			"An, Khang",
			"Đức, Tài, Như, Ý",
			"Niên",
			"Thịnh, Vượng",
			"Phúc, Thọ, Vô, Biên"
		};

	private IScanner BarcodeScanner;
	public Text TextHeader;
	public RawImage Image;
	public AudioSource Audio;
	private float RestartTime;
	public Text ListCamera;
	public Text PlaceNum;
	public List<GameObject> areaPieces;

	public GameObject ChucmungObj;
	public GameObject ChucmungComplete;
	private bool isChange = false;

	public GameObject PlaceInfoPrefab;

	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start() {

		ChucmungObj.SetActive(false);
		ChucmungComplete.SetActive(false);

		Console.WriteLine("Out: " + StaticParamClass.GoFromOutside);
		if (StaticParamClass.GoFromOutside == true)
		{
			// nếu là vào từ bên ngoài => kiểm tra xem login chưa???
			StaticParamClass.DaCheckRoi = true;
			Console.WriteLine("DaCheckRoi: " + StaticParamClass.DaCheckRoi);

			ProcessScannedQR(true);
		} else
		{
			// Create a basic scanner
			BarcodeScanner = new Scanner();
			BarcodeScanner.Camera.Play();

			// Display the camera texture through a RawImage
			BarcodeScanner.OnReady += (sender, arg) => {
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



	}

	void ProcessScannedQR(bool fromOpenWeb = false)
	{
		// nếu đã lưu Checkin Name vào máy rồi => chỉ việc load data về rồi xử lý
		if (IsSignedUp())
		{
			// đã đăng ký => load data và xử lý sau khi load
			// Check in and go to Main;
			Debug.Log("Go to main directly: " + PlayerPrefs.GetString(StaticParamClass.PrefCheckinName));

			StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
		}
		else
		{
			// nếu chưa lưu Checkin Name vào máy => là mới => intro => sau đó xem xét để chúc mừng
			PlaceInfo = Instantiate(PlaceInfoPrefab, root);
			PlaceInfo.name = "Place Info";
			PlaceInfo.GetComponent<PlaceInfoHolder>().OpenPlaceInfo(0, StaticParamClass.IsMapUnlocked[0],
				() =>
				{
					GotoCongrats(StaticParamClass.CheckinPlace);
				});
		}
	}
	[HideInInspector]
	public GameObject PlaceInfo;

	[SerializeField]
	public Transform root;

	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		foreach (WebCamDevice wd in WebCamTexture.devices)
		{
			ListCamera.text += wd.name + "-" + wd.isFrontFacing + "\n";
		}
		StaticParamClass.DaCheckRoi = true;
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			Debug.Log(barCodeType + " -- " + barCodeValue);

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
					if (d.Contains("CheckinPlace"))
					{
						StaticParamClass.CheckinPlace = Int32.Parse(d.Split("=")[1]);
						StaticParamClass.IsMapUnlocked[StaticParamClass.CheckinPlace] = true;
					}

					// xử lý thông tin sau khi nhận QR Code
					ProcessScannedQR();
				}
			} else
			{
				//TextHeader.text += "Error barcode: " + barCodeType + " / " + barCodeValue + "\n";
				Debug.Log("Error barcode: " + barCodeType + " / " + barCodeValue + "\n");
				StartScanner();
			}



			// Save the place info here -- Redirect to Main or Checkin
			//int place = (new System.Random()).Next(MainController.MAX_PLACE);
			//MainController.Instance.curPlace = place;
			//MainController.Instance.activated[place] = true;
			///
			//RestartTime += Time.realtimeSinceStartup + 1f;

			// Feedback
			//Audio.Play();

#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
#endif
		});
	}

	

	public void ChangeCamera()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		isChange = true;
		ScannerSettings cSetting = BarcodeScanner.Settings;
		string name = cSetting.WebcamDefaultDeviceName;
		StartCoroutine(StopCamera(() => {
			ScannerSettings ss = new ScannerSettings(name);
			Debug.Log(ss.WebcamDefaultDeviceName);
			BarcodeScanner = new Scanner(ss);
			BarcodeScanner.Camera.Play();

			// Display the camera texture through a RawImage
			BarcodeScanner.OnReady += (sender, arg) => {
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


	/// <summary>
	/// a: là string data các địa điểm user đã checkin
	/// </summary>
	/// <param name="a"></param>
	/// <param name="name"></param>
	public void OnDataLoaded(string a, string name)
	{
		StaticParamClass.CheckedIn = a;
		int currentPlace = StaticParamClass.CheckinPlace;
		if (!StaticParamClass.CheckedIn.Contains(currentPlace.ToString())
			&& IsTargetPlace(currentPlace))
		{
			// nếu chưa checkin địa điểm này => đây là địa điểm mới!!!
			// và địa điểm mới này là địa điểm target

			//PlaceNum.text = "SỐ "  + (StaticParamClass.CheckinPlace + 1);
			PlaceNum.text = WordOfPlace(currentPlace);
			SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.checkIn);
			StaticParamClass.CheckedIn += ";" + currentPlace.ToString();

			// show chúc mừng đã checkin được địa điểm
			ChucmungObj.SetActive(true);
			for (int i = 0; i < areaPieces.Count; i++)
			{
				if (i == currentPlace)
				{
					areaPieces[i].SetActive(true);
				}
				else
				{
					areaPieces[i].SetActive(false);
				}
			}
		}
		else
		{
			// nếu đã checkin rồi => update lại data...
			// hoặc là địa điểm này không cần phải target (không cần show kết quả).

			//Checkin checkin = new Checkin();
			//Debug.Log(checkin);
			SaveDataAndBackToMain();
		}
	}

	public IEnumerator GetData(string name)
	{

		SetGetUserData.GetCheckedinPlace_(name, OnDataLoaded);
		yield return null;
	}

	public string WordOfPlace(int placeId)
	{
		return words[placeId];
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

	public void GotoCongrats(int place)
	{
		// nếu không phải target => xử lý khác
		if (!IsTargetPlace(place))
		{
			if (IsSignedUp())
			{
				// đã đăng ký => về main luôn
				SaveDataAndBackToMain();
			}
			else
			{
				// chưa đăng ký thì đi đăng ký
				GoToSignUp();
			}
			return;
		}


		// show chúc mừng

		//PlaceNum.text = "SỐ " + (StaticParamClass.CheckinPlace + 1);
		PlaceNum.text = WordOfPlace(StaticParamClass.CheckinPlace);
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.checkIn);
		ChucmungObj.SetActive(true);
		StaticParamClass.CheckedIn += ";" + StaticParamClass.CheckinPlace.ToString();
		for (int i = 0; i < areaPieces.Count; i++)
		{
			if (i == place)
			{
				areaPieces[i].SetActive(true);
			} else
			{
				areaPieces[i].SetActive(false);
			}
		}


		//StartCoroutine(StopCamera(() => {
		//	SceneManager.LoadScene("Checkin");
		//}));
	}

	/// <summary>
	/// Event được gọi khi Close button chúc mừng.
	/// </summary>
	public void OKButtonChucmung()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

		// check lại quá trình, xem đã check in chưa
		if (IsSignedUp())
		{
			// nếu đã checkin rồi => kiểm tra xem hoàn thành chưa

			if (
				//StaticParamClass.CheckedIn.Contains("0") &&
				StaticParamClass.CheckedIn.Contains("1") &&
				StaticParamClass.CheckedIn.Contains("2") &&
				StaticParamClass.CheckedIn.Contains("3") &&
				StaticParamClass.CheckedIn.Contains("4") &&
				StaticParamClass.CheckedIn.Contains("5"))
			{
				// nếu hoàn thành rồi => mở completed
				ChucmungComplete.SetActive(true);
				ChucmungObj.SetActive(false);
			} else
			{

				// chea hoàn thành => save data và về Main
				SaveDataAndBackToMain();
			}
		}
		else
		{
			GoToSignUp();
		}
	}

	void GoToSignUp()
	{
		StartCoroutine(StopCamera(() => {
			SceneManager.LoadScene("Checkin");
		}));
	}

	bool IsSignedUp()
	{
		bool isSavedName = PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName);
		if (isSavedName)
		{
			bool isSavedNameNotNull = !PlayerPrefs.GetString(StaticParamClass.PrefCheckinName).Equals("");
			return isSavedNameNotNull;
		}
		return false;
	}

	void SaveDataAndBackToMain()
	{
		StartCoroutine(Checkin.CheckinPre(
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinName),
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber),
				StaticParamClass.CheckinPlace)
				);
	}

	public void OKButtonComplete()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

		SaveDataAndBackToMain();
	}

	#region UI Buttons

	public void ClickBack()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		// Try to stop the camera before loading another scene
		StartCoroutine(StopCamera(() => {
			//StaticParamClass.GoFromInside = true;
			SceneManager.LoadScene("Main");
		}));
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
		if(BarcodeScanner != null)
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
