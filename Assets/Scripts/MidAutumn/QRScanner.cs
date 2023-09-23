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

	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start () {

		ChucmungObj.SetActive(false);
		ChucmungComplete.SetActive(false);
		BarcodeScanner = new Scanner();
		if (StaticParamClass.GoFromOutside == true)
		{
			if (PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName) && !PlayerPrefs.GetString(StaticParamClass.PrefCheckinName).Equals(""))
			{
				StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
			}
			else
			{
				GotoCheckin(StaticParamClass.CheckinPlace);
			}
		} else
		{
			// Create a basic scanner
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

	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		foreach (WebCamDevice wd in WebCamTexture.devices)
		{
			ListCamera.text += wd.name + "-" + wd.isFrontFacing + "\n";
		}

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


					if (PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName) && !PlayerPrefs.GetString(StaticParamClass.PrefCheckinName).Equals(""))
					{
						// Check in and go to Main;
						Debug.Log("Go to main directly: " + PlayerPrefs.GetString(StaticParamClass.PrefCheckinName));

						StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
					}
					else
					{
						
						GotoCheckin(StaticParamClass.CheckinPlace);
					}


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



	public void getCheckIn(string a, string name)
	{
		StaticParamClass.CheckedIn = a;
		if (!StaticParamClass.CheckedIn.Contains(StaticParamClass.CheckinPlace.ToString()))
		{
			PlaceNum.text = "SỐ "  + (StaticParamClass.CheckinPlace + 1);
			SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.checkIn);
			StaticParamClass.CheckedIn += ";" + StaticParamClass.CheckinPlace.ToString();
			ChucmungObj.SetActive(true);
			for (int i = 0; i < areaPieces.Count; i++)
			{
				if (i == StaticParamClass.CheckinPlace)
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
			//Checkin checkin = new Checkin();
			//Debug.Log(checkin);
			StartCoroutine(Checkin.CheckinPre(
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinName),
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber),
				StaticParamClass.CheckinPlace)
				);
		}
	}

	public IEnumerator GetData(string name)
	{

		SetGetUserData.GetCheckedinPlace_(name, getCheckIn);
		yield return null;
	}



	/// <summary>
	/// The Update method from unity need to be propagated
	/// </summary>
	void Update()
	{
		if(!isChange)
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

	public void GotoCheckin(int place)
	{
		PlaceNum.text = "SỐ " + (StaticParamClass.CheckinPlace + 1);
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.checkIn);
		ChucmungObj.SetActive(true);
		StaticParamClass.CheckedIn += ";" + StaticParamClass.CheckinPlace.ToString();
		for (int i = 0; i < areaPieces.Count; i++)
		{
			if(i == place)
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

	public void OKButtonChucmung()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		if (PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName) && !PlayerPrefs.GetString(StaticParamClass.PrefCheckinName).Equals(""))
		{
			if(StaticParamClass.CheckedIn.Contains("0") &&
				StaticParamClass.CheckedIn.Contains("1") &&
				StaticParamClass.CheckedIn.Contains("2") &&
				StaticParamClass.CheckedIn.Contains("3") &&
				StaticParamClass.CheckedIn.Contains("4") &&
				StaticParamClass.CheckedIn.Contains("5"))
			{
				ChucmungComplete.SetActive(true);
				ChucmungObj.SetActive(false);
			} else
			{
				StartCoroutine(Checkin.CheckinPre(
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinName),
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber),
				StaticParamClass.CheckinPlace)
				);
			}
		}
		else
		{

			StartCoroutine(StopCamera(() => {
				SceneManager.LoadScene("Checkin");
			}));
		}
	}

	public void OKButtonComplete()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		StartCoroutine(Checkin.CheckinPre(
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinName),
				PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber),
				StaticParamClass.CheckinPlace)
				);
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
		BarcodeScanner.Destroy();
		BarcodeScanner = null;

		// Wait a bit
		yield return new WaitForSeconds(0.1f);

		callback.Invoke();
	}

	#endregion
}
