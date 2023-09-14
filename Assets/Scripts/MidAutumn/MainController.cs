using System;
using System.Collections;
using System.Collections.Generic;
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


		//for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
		//{
		//	if (StaticParamClass.IsMapUnlocked[i])
		//		mapPieces[i].SetActive(false);
		//}
		//curPlace = StaticParamClass.CheckinPlace;
		//   if (curPlace >= 0 && curPlace < StaticParamClass.MAX_PLACE)
		//   {
		//	StaticParamClass.IsMapUnlocked[curPlace] = true;
		//	OpenPlaceInfo(curPlace);
		//   }
		//StaticParamClass.CheckinPlace = (new Random()).Next(StaticParamClass.MAX_PLACE);
		if(StaticParamClass.GoFromInside)
		{
			OpenPlaceInfo(StaticParamClass.CheckinPlace);
			SetUsername();
		} else
		{
			//Check tai khoan
			StartCoroutine(GetData(PlayerPrefs.GetString(StaticParamClass.PrefCheckinNumber)));
			SetUsername();
		}
		
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
			if(a.Contains(i.ToString()))
			{
				StaticParamClass.IsMapUnlocked[i] = true;
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }

    public void OpenPlaceInfo(int placeNum)
    {
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
	    SceneManager.LoadScene("QR Scanner");
    }

	private void SetUsername()
	{
		if(PlayerPrefs.HasKey(StaticParamClass.PrefCheckinName))
		{
			usernameText.text = PlayerPrefs.GetString(StaticParamClass.PrefCheckinName);
		}
	}

	[SerializeField, HideInInspector]
	public List<bool> activated = new List<bool>(StaticParamClass.MAX_PLACE);

	[HideInInspector] public int curPlace = -1;

	public GameObject PlaceInfo;
	public PlaceInfo activatedPlacePopup;
	public PlaceInfo nonActivatedPlacePopup;

	public Text usernameText;

	private PlaceInfo activePlacePopup;

	public List<GameObject> mapPieces;

	public GameObject PlaceInfoPrefab;

	public static MainController Instance;

	public GameObject MainScreen;
}
