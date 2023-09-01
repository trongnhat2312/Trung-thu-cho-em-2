using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{

	private void Awake()
	{
		Instance = this;
		for (int i = 0; i < MAX_PLACE; i++)
		{
			activated.Add(false);
		}
		// load user data here

	}
    // Start is called before the first frame update
    void Start()
    {
	    if (curPlace >= 0 && curPlace < MAX_PLACE)
	    {
			OpenPlaceInfo(curPlace);
	    }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPlaceInfo(int placeNum)
    {
		Debug.Log("place == " + placeNum);
	    if (placeNum < 0 || placeNum > MAX_PLACE - 1)
	    {
			Debug.LogError("Place number out of range [0, MAX_PLACE - 1]");
	    }
		curPlace = placeNum;
		placeInfo.SetActive(true);
		if (activated[placeNum])
		{
			activatedPlacePopup.SetActive(true);
			nonActivatedPlacePopup.SetActive(false);
		}
		else
		{

			activatedPlacePopup.SetActive(false);
			nonActivatedPlacePopup.SetActive(true);
		}
    }

    public void ClickScan()
    {
	    SceneManager.LoadScene("QR Scanner");
    }

	[SerializeField, HideInInspector]
	public List<bool> activated = new List<bool>(MAX_PLACE);

	[HideInInspector] public int curPlace = -1;
	[HideInInspector] public string userName;
	[HideInInspector] public string phoneNumber;

	[SerializeField] public static int MAX_PLACE = 6;

	public GameObject placeInfo;
	public GameObject activatedPlacePopup;
	public GameObject nonActivatedPlacePopup;

	public static MainController Instance;
}
