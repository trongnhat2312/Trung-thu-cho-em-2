using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab.ServerModels;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Checkin : MonoBehaviour
{
	public InputField nickName;
	public InputField phoneNumber;
	public Text ErrorName;
	public Text ErrorPhone;

	public delegate void ResFromGet(string a);
	public delegate void ResFromGet_(string a, string name);

	void Awake()
	{
		// Save a Reference to the component as our singleton instance
		Instance = this;
	}

	public static Checkin Instance { get; private set; }

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

		Debug.Log(StaticParamClass.MAX_PLACE);
		//StaticParamClass.IsMapUnlocked = new List<bool>(StaticParamClass.MAX_PLACE);
		Debug.Log(StaticParamClass.IsMapUnlocked.Length);

		for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
		{
			if(a.Contains(i.ToString()))
			{
				Debug.Log(i + "--" + StaticParamClass.IsMapUnlocked);
				StaticParamClass.IsMapUnlocked[i] = true;
			}
		}

		Debug.Log(StaticParamClass.IsMapUnlocked.Length + "_-" + StaticParamClass.IsMapUnlocked);

		SetTitleDataRequest title = new SetTitleDataRequest
		{
			Key = phoneNumber.text.Trim(),
			Value = StaticParamClass.CheckedIn + ";" + StaticParamClass.CheckinPlace
		};

		SetGetUserData.SetCheckinPlace(title);
		StaticParamClass.GoFromInside = true;
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

			StartCoroutine(SetGetUserData.GetCheckedinPlace(phoneNumber.text.Trim(), setData));
		}
		
	}

	public static void setData_(string a, string name)
	{
		StaticParamClass.CheckedIn = a;

		//Debug.Log(StaticParamClass.CheckedIn);
		//StaticParamClass.IsMapUnlocked = new List<bool>(StaticParamClass.MAX_PLACE);

		for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
		{
			if (a.Contains(i.ToString()))
			{
				StaticParamClass.IsMapUnlocked[i] = true;
			}
		}

		Debug.Log(StaticParamClass.IsMapUnlocked.Length + "__-" + StaticParamClass.IsMapUnlocked);

		SetTitleDataRequest title = new SetTitleDataRequest
		{
			Key = name,
			Value = StaticParamClass.CheckedIn + ";" + StaticParamClass.CheckinPlace
		};

		SetGetUserData.SetCheckinPlace(title);
		StaticParamClass.GoFromInside = true;
		SceneManager.LoadScene("Main");
	}

	public static IEnumerator CheckinPre(string name, string number, int place)
	{

		Debug.Log("come here" + name);
		SetGetUserData.GetCheckedinPlace_(number, setData_);
		yield return null;
		//Debug.LogError("go continue");
		//SceneManager.LoadScene("Main");
	}
}

