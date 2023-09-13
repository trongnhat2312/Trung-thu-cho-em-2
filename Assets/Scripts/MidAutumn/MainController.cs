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
		OpenPlaceInfo(StaticParamClass.CheckinPlace);
		SetUsername();
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
