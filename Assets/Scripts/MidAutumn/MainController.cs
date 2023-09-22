using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class MainController : MonoBehaviour
{

	private void Awake()
	{
		Instance = this;

	}
	// Start is called before the first frame update
	void Start()
	{
		// Main load. kiem tra xem user da co tai khoan va checkin ở địa điểm nào chưa
		// Nếu đã có tài khoản: thực hiện checkin/set place num các thứ
		// Nếu chưa có tài khoản: load checkin scene để nó checkin.

		// test //
		//StaticParamClass.CheckinPlace = (new Random()).Next(StaticParamClass.MAX_PLACE);
		//StaticParamClass.GoFromInside = true;
		//StaticParamClass.IsMapUnlocked[StaticParamClass.CheckinPlace] = true;
		// test //
		_isStarEffEnabled = true;
		if (StaticParamClass.GoFromInside)
		{
			showMapPieces();
			StartCoroutine(OpenPlaceInfoWithEffect(StaticParamClass.CheckinPlace));
			SetUsername();
		}
		else
		{
			//Check tai khoan
			StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
			SetUsername();
		}

		_starLightCount = 0;


	}

	public IEnumerator GetData(string name)
	{

		SetGetUserData.GetCheckedinPlace_(name, getCheckIn);
		yield return null;
	}

	public void getCheckIn(string a, string name)
	{
		StaticParamClass.CheckedIn = a;
		Debug.Log(StaticParamClass.IsMapUnlocked.Length);
		for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
		{
			if (a.Contains(i.ToString()))
			{
				Debug.Log("Come here moi dung: " + i);
				StaticParamClass.IsMapUnlocked[i] = true;
			}
		}
		showMapPieces();
	}

	// Update is called once per frame
	void Update()
	{
		if (StaticParamClass.GoFromInside)
		{
			return;
		}
		if (IsAllMapUnlocked() && _isStarEffEnabled)
		{
			//Debug.Log("_starLightCount:" + _starLightCount);
			if (_starLightCount <= 0)
			{
				starLight.DoTransformToStarLight();
				_alreadyResetStarLight = false;
				_starLightCount = StarLightInterval;
			}
			else
			{
				_starLightCount -= Time.deltaTime;
				if (_starLightCount <= 2 && !_alreadyResetStarLight)
				{
					starLight.ResetBeforeTransform();
					_alreadyResetStarLight = true;
				}
			}
		}

		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
		{
			if (!_alreadyResetStarLight)
			{
				starLight.ResetBeforeTransform();
				_alreadyResetStarLight = true;
			}

			_isStarEffEnabled = false;

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
		var effect = mapPieces[placeNum].GetComponent<UITransitionEffect>();
		effect.Hide(false);
		yield return new WaitForSeconds(effect.effectPlayer.duration);
		StaticParamClass.GoFromInside = false;
		OpenPlaceInfo(placeNum);

	}
	public void OpenPlaceInfo(int placeNum)
	{
		if (StaticParamClass.GoFromInside)
			return;
		Debug.Log("place == " + placeNum);
		if (placeNum == -1)
			return;
		if (placeNum < 0 || placeNum > StaticParamClass.MAX_PLACE - 1)
		{
			Debug.LogError("Place number out of range [0, MAX_PLACE - 1]");
			return;
		}
		PlaceInfo = Instantiate(PlaceInfoPrefab);
		PlaceInfo.transform.SetParent(MainScreen.transform.parent, false);
		PlaceInfo.name = "Place Info";
		PlaceInfo.GetComponent<PlaceInfoHolder>().OpenPlaceInfo(placeNum, StaticParamClass.IsMapUnlocked[placeNum]);
		MainScreen.SetActive(false);
	}


	public void ClosePlaceInfo()
	{
		MainScreen.SetActive(true);
		Destroy(PlaceInfo);
	}

	public void ClickScan()
	{
		if (StaticParamClass.GoFromInside)
			return;
		SceneManager.LoadScene("QR Scanner");
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
				mapPieces[i].GetComponent<UITransitionEffect>().effectFactor = 0;
			}
		}
	}

	[HideInInspector]
	public GameObject PlaceInfo;

	public Text usernameText;

	public List<GameObject> mapPieces;

	public GameObject PlaceInfoPrefab;

	public static MainController Instance;

	public GameObject MainScreen;

	public StarLightTransformer starLight;

	public float StarLightInterval = 10f;
	private float _starLightCount;
	private bool _alreadyResetStarLight;
	private bool _isStarEffEnabled = true;
}
