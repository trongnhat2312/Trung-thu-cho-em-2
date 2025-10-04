using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using iSTEAM.STEAM.ParallelSentences;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

using Cysharp.Threading.Tasks;

public class MainController : MonoBehaviour
{
	public static string SCENENAME_MAIN = "MidAutumn_Main";
	public static string SCENENAME_CHECKIN = "MidAutumn_Checkin";
	public static string SCENENAME_QRSCANNER = "MidAutumn_QRScanner";

	private void Awake()
	{
		Instance = this;

	}

	public bool debugStartFromUrlQR = false;
	public string debugAbsoluteURL = "https://koikinggaming.com/vme/Tet-2025/?CheckinPlace=0";

	// Start is called before the first frame update
	void Start()
	{
		SetupStart();
	}

	protected virtual async void SetupStart()
	{
#if !UNITY_EDITOR
		debugStartFromUrlQR = false;
#endif

		// Main load. kiem tra xem user da co tai khoan va checkin ở địa điểm nào chưa
		// Nếu đã có tài khoản: thực hiện checkin/set place num các thứ
		// Nếu chưa có tài khoản: load checkin scene để nó checkin.
		string absoluteURL = Application.absoluteURL;
#if UNITY_EDITOR
		if (debugStartFromUrlQR)
		{
			absoluteURL = debugAbsoluteURL;
		}
#endif

		int pm = absoluteURL.IndexOf("CheckinPlace");
		Console.WriteLine("Set DaCheckRoi: " + StaticParamClass.DaCheckRoi);

		await UniTask.DelayFrame(3);

		if (pm != -1 && !StaticParamClass.DaCheckRoi)
		{
			Debug.Log($"MainController: First time from Url QR {pm}");
			StaticParamClass.GoFromOutside = true;
			StaticParamClass.CheckinPlace = Int32.Parse(absoluteURL.Split("=")[1]);

			Debug.Log($"MainController: First time from Url QR {pm}, {StaticParamClass.CheckinPlace}");
			StaticParamClass.IsMapUnlocked[StaticParamClass.CheckinPlace] = true;
			Console.WriteLine("Set out: " + StaticParamClass.GoFromOutside);
			Console.WriteLine("Set check: " + StaticParamClass.CheckinPlace);
			SceneManager.LoadScene(SCENENAME_QRSCANNER);
		}
		// test //
		//StaticParamClass.CheckinPlace = (new Random()).Next(StaticParamClass.MAX_PLACE);
		//StaticParamClass.GoFromInside = true;
		//StaticParamClass.IsMapUnlocked[StaticParamClass.CheckinPlace] = true;
		//StaticParamClass.IsMapUnlocked[0] = true;
		//StaticParamClass.IsMapUnlocked[1] = true;
		//StaticParamClass.IsMapUnlocked[2] = true;
		//StaticParamClass.IsMapUnlocked[3] = true;
		//StaticParamClass.IsMapUnlocked[4] = true;
		//StaticParamClass.IsMapUnlocked[5] = true;
		// test //
		_isStarEffEnabled = true;
		//_isPopupOpen = true;
		if (StaticParamClass.GoFromInside)
		{
			Debug.Log($"MainController: Go From InSide, show map piece, place info...");
			showMapPieces();
			StartCoroutine(OpenPlaceInfoWithEffect(StaticParamClass.CheckinPlace));
			SetUsername();
		}
		else
		{
			Debug.Log($"MainController: Go From OutSide... Check Account and user");

			//Check tai khoan
			StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
			SetUsername();
		}

		_starLightCount = 0;
	}

	public IEnumerator GetData(string name)
	{
		Debug.Log($"MainController: do get data by name {name}");
		SetGetUserData.GetCheckedinPlace_(name, getCheckIn);
		yield return null;
	}

	public void getCheckIn(string a, string name)
	{
		StaticParamClass.CheckedIn = a;
		Debug.Log($"MainController: get Data result: {StaticParamClass.IsMapUnlocked.Length}");
		for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
		{
			if (a.Contains(i.ToString()))
			{
				Debug.Log("MainController: Come here moi dung: " + i);
				StaticParamClass.IsMapUnlocked[i] = true;
			}
		}
		showMapPieces();
	}

	// Update is called once per frame
	void Update()
	{
		DoUpdate();
	}

	protected virtual void DoUpdate()
	{
		if (StaticParamClass.GoFromInside)
		{
			return;
		}
		if (IsAllMapUnlocked())
		{
			ShowVictoryUI();
		}
	}

	protected virtual async void ShowVictoryUI()
	{

		Debug.Log($"MainController: update: all map unlocked => update");
		if (!isAnimCompleted)
		{
			isAnimCompleted = true;
			SetupPieceEffectMode();
			starLightTransformer.DoTransformToStarLight();
			await UniTask.Delay(2000);
			txtComplete.gameObject.SetActive(true);
		}

	}

	void SetupPieceEffectMode()
	{
		for (int i = 0; i < mapPieces.Count; i++)
		{
			int placeNum = i;
			if (mapPieces != null && mapPieces.Count > placeNum && mapPieces[placeNum] != null)
			{
				var effect = mapPieces[placeNum].GetComponent<UITransitionEffect>();
				if (effect != null)
				{
					// effect.Hide(false);
					// yield return new WaitForSeconds(effect.effectPlayer.duration);
					effect.effectMode = UITransitionEffect.EffectMode.Fade;
					effect.effectFactor = 0.36f;
				}
			}
		}
	}

	void UpdateVersionThachSanh()
	{
		if (_isStarEffEnabled)
		{
			//Debug.Log("_starLightCount:" + _starLightCount);
			if (_starLightCount <= 0)
			{
				Debug.Log($"MainController: update: show Victory");
				//starLight.DoTransformToStarLight();
				//parallelSentence.OnCompleted();
				ThachSanhVictory.gameObject.SetActive(true);
				SetBaseMenuComponentVisible(false);
				_alreadyResetStarLight = false;
				_starLightCount = StarLightInterval;
			}
			else
			{
				//_starLightCount -= Time.deltaTime;
				//if (_starLightCount <= 1 && !_alreadyResetStarLight)
				//{
				//	starLight.ResetBeforeTransform();
				//	_alreadyResetStarLight = true;
				//	_isPlayedOnce = true;
				//}
				if (_starLightCount > 1)
				{
					_starLightCount -= Time.deltaTime;
				}
				if (_starLightCount <= 1)
				{
					_isPlayedOnce = true;
				}
			}
		}

		if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && _isPlayedOnce)
		{
			//parallelSentence.ResetBeforeTransform();
			//ThachSanhVictory.gameObject.SetActive(false);
			_alreadyResetStarLight = true;
			_isStarEffEnabled = false;

		}
	}

	void SetBaseMenuComponentVisible(bool isVisible)
	{
		foreach (var pGameObject in m_MenuBaseComponents)
		{
			try
			{
				if (pGameObject != null)
				{
					pGameObject.SetActive(isVisible);
				}
			}
			catch (Exception exception)
			{
			}
		}
	}

	public IEnumerator OpenPlaceInfoWithEffect(int placeNum)
	{
		Debug.Log("place == " + placeNum);
		if (placeNum == -1)
			yield break;
		if (placeNum < 0 || placeNum > StaticParamClass.MAX_PLACE - 1)
		{
			Debug.LogError("Place number out of range [0, MAX_PLACE - 1]");
			yield break;
		}

		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.pieceDisappear);
		if (mapPieces != null && mapPieces.Count > placeNum && mapPieces[placeNum] != null)
		{
			var effect = mapPieces[placeNum].GetComponent<UITransitionEffect>();
			if (effect != null)
			{
				effect.Hide(false);
				yield return new WaitForSeconds(effect.effectPlayer.duration);
			}
		}


		yield return new WaitForSeconds(0.5f);
		StaticParamClass.GoFromInside = false;
		OpenPlaceInfo(placeNum);

	}
	public void OpenPlaceInfo(int placeNum)
	{

		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		if (StaticParamClass.GoFromInside)
			return;
		Debug.Log("place == " + placeNum);
		if (placeNum == -1)
			return;
		if (placeNum < 0 || (placeNum > StaticParamClass.MAX_PLACE - 1))
		{
			Debug.LogError("Place number out of range [0, MAX_PLACE - 1]");
			return;
		}
		if (IsAllMapUnlocked())
		{
			// todo - dont need to show. or must show then close then show completed anim
			// return;
		}

		PlaceInfo = Instantiate(PlaceInfoPrefab);
		PlaceInfo.transform.SetParent(MainScreen.transform.parent, false);
		PlaceInfo.name = "Place Info";
		PlaceInfo.GetComponent<PlaceInfoHolder>().OpenPlaceInfo(placeNum, StaticParamClass.IsMapUnlocked[placeNum], null, () =>
		{
			Debug.Log($"MainController: place == {placeNum}, close and open QR");
			// process open qr here
			ClickScan();
		});
		MainScreen.SetActive(false);
	}


	public void ClosePlaceInfo()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		//_isPopupOpen = false;
		MainScreen.SetActive(true);
		Destroy(PlaceInfo);
	}

	public void ClickScan()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		StaticParamClass.GoFromOutside = false;
		if (StaticParamClass.GoFromInside)
			return;
		SceneManager.LoadScene(SCENENAME_QRSCANNER);
	}

	private void SetUsername()
	{
		if (PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName))
		{
			usernameText.text = PlayerPrefs.GetString(StaticParamClass.PrefCheckinName);
		}
	}

	public bool IsAllMapUnlocked()
	{
		bool b = true;
		for (int i = 0; i < StaticParamClass.IsMapUnlocked.Length; i++)
		{
			if (!StaticParamClass.IsMapUnlocked[i])
			{
				//Debug.Log(i);
				b = false;
				break;
			}
		}
		return b;
	}

	public void showMapPieces()
	{
		for (int i = 0; i < StaticParamClass.IsMapUnlocked.Length; i++)
		{
			if (StaticParamClass.IsMapUnlocked[i])
			{
				try
				{
					mapPieces[i].GetComponent<UITransitionEffect>().effectFactor = 0;
				}
				catch (Exception exception)
				{
				}

				try
				{
					//var child = mapPieces[i].transform.GetChild(0);
					//mapPieces[i].GetComponent<UITransitionEffect>().effectFactor = 0;
					//var image = child.GetComponent<Image>();
					//image.color = Color.white;
					MapCheckpointBase checkpointBase = mapPieces[i].GetComponent<MapCheckpointBase>();
					if (checkpointBase != null)
					{
						checkpointBase.SetActiveState(true);
					}
				}
				catch (Exception exception)
				{
				}
			}
		}
		if (IsAllMapUnlocked())
		{
			ScanButton.SetActive(false);
		}

	}

	// [HideInInspector]
	public GameObject PlaceInfo;

	public Text usernameText;

	public List<GameObject> mapPieces;

	public GameObject PlaceInfoPrefab;

	public static MainController Instance;

	public GameObject MainScreen;

	//public StarLightTransformer starLight;
	//public ParallelSentencesController parallelSentence;
	public GameObject ThachSanhVictory;

	public GameObject ScanButton;
	public List<GameObject> m_MenuBaseComponents;

	public float StarLightInterval = 10f;
	private float _starLightCount;
	private bool _alreadyResetStarLight;
	private bool _isStarEffEnabled = true;
	private bool _isPopupOpen = true;
	private bool _isPlayedOnce = false;



	[SerializeField] StarLightTransformer starLightTransformer;
	[SerializeField] Text txtComplete;
	protected bool isAnimCompleted = false;

	public void TestCheckin()
	{ 
		SceneManager.LoadScene(MainController.SCENENAME_CHECKIN);
	}
}
