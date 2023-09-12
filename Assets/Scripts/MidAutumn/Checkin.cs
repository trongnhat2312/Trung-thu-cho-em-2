using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab.ServerModels;

public class Checkin : MonoBehaviour
{
	public TMP_InputField nickName;
	public TMP_InputField phoneNumber;
	public TMP_Text ErrorName;
	public TMP_Text ErrorPhone;

	public delegate void ResFromGet(string a);

	// Use this for initialization
	void Start()
	{
		Debug.Log("Checkin Loaded");
		ErrorName.gameObject.SetActive(false);
		ErrorPhone.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
			
	}

	public void setData(string a)
	{
		StaticParamClass.CheckedIn = a;

		Debug.Log(StaticParamClass.CheckedIn);

		SetTitleDataRequest title = new SetTitleDataRequest
		{
			Key = nickName.text.Trim(),
			Value = StaticParamClass.CheckedIn + ";" + StaticParamClass.CheckinPlace
		};

		SetGetUserData.SetCheckinPlace(title);

		SceneManager.LoadScene("Main");
	}

	public void CheckinData()
	{
		string name = nickName.text;
		string number = phoneNumber.text;
		// Text validation
		if(!name.Equals("") && name != null)
		{
			ErrorName.gameObject.SetActive(false);
		}

		if (!phoneNumber.Equals("") && phoneNumber != null)
		{
			ErrorPhone.gameObject.SetActive(false);
		}
		if (name.Equals("") || name == null)
		{
			ErrorName.gameObject.SetActive(true);
			ErrorName.text = "Error: Name cannot be empty";
		}
		else if (number.Equals("") || number == null)
		{
			ErrorPhone.gameObject.SetActive(true);
			ErrorPhone.text = "Error: Phone cannot be empty";
		} else
		{
			int place = StaticParamClass.CheckinPlace;
			PlayerPrefs.SetString(StaticParamClass.PrefCheckinName, nickName.text.Trim());
			PlayerPrefs.SetString(StaticParamClass.PrefCheckinNumber, phoneNumber.text.Trim());
			// Send data to Azure Prefab and go to main

			PlayFabLogin.RegisterUser(nickName.text.Trim(), phoneNumber.text.Trim());

			Debug.Log("Name: " + PlayerPrefs.GetString("CheckinName"));
			Debug.Log("Number: " + PlayerPrefs.GetString("CheckinNumber"));

			StartCoroutine(SetGetUserData.GetCheckedinPlace(nickName.text.Trim(), setData));
		}
		
	}

	public static void CheckinPre(string name, string number, int place)
	{
		// Save data to Azure and go to main;
		Debug.Log("ABC");



		SceneManager.LoadScene("Main");
	}
}

