using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{

	private void Awake()
	{
		Instance = this;
		//for (int i = 0; i < MAX_PLACE; i++)
		//{
		//	StaticParamClass.IsMapUnlocked.Add(false);
		//}
		// load user data here

		//for(int i = 0; i < MAX_PLACE; i++)
		//{

		//}
		//activated = StaticParamClass.IsMapUnlocked;
	}
    // Start is called before the first frame update
    void Start()
    {
		
		curPlace = StaticParamClass.CheckinPlace;
	    if (curPlace >= 0 && curPlace < StaticParamClass.MAX_PLACE)
	    {
			StaticParamClass.IsMapUnlocked[curPlace] = true;
			OpenPlaceInfo(curPlace);
	    }

		SetUsername();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPlaceInfo(int placeNum)
    {
		Debug.Log("place == " + placeNum);
	    if (placeNum < 0 || placeNum > StaticParamClass.MAX_PLACE - 1)
	    {
			Debug.LogError("Place number out of range [0, MAX_PLACE - 1]");
	    }

		curPlace = placeNum;
		placeInfo.SetActive(true);
		activatedPlacePopup.gameObject.SetActive(false);
		nonActivatedPlacePopup.gameObject.SetActive(false);
		if (activated[placeNum])
		{
			this.activePlacePopup = activatedPlacePopup;
		}
		else
		{
			this.activePlacePopup = nonActivatedPlacePopup;
		}
		this.activePlacePopup.gameObject.SetActive(true);
		this.activePlacePopup.SetPlaceNum(placeNum);

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
	[HideInInspector] public string userName;
	[HideInInspector] public string phoneNumber;

	[SerializeField] public static int MAX_PLACE = 6;

	public GameObject placeInfo;
	public PlaceInfo activatedPlacePopup;
	public PlaceInfo nonActivatedPlacePopup;

	public Text usernameText;

	private PlaceInfo activePlacePopup;

	public static MainController Instance;
}
