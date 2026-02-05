using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ServerModels;
using System;
using Cysharp.Threading.Tasks;

public class CheckInPopup : MonoBehaviour
{
	private Action OnClosePopupListener;

	public InputField nickName;
	public InputField phoneNumber;
	public Text ErrorName;
	public Text ErrorPhone;
	[SerializeField] Button btnOk;

	public delegate void ResFromGet(string a);
	public delegate void ResFromGet_(string a, string name);

	void Start()
	{
		btnOk.onClick.AddListener(OnBtnOkClicked);
	}

	private void OnBtnOkClicked()
	{
		CheckinData();
	}

	public void ShowPopup(Action pCallback)
	{
		OnClosePopupListener = pCallback;
		gameObject.SetActive(true);
		ErrorName.gameObject.SetActive(false);
		ErrorPhone.gameObject.SetActive(false);
	}

	public void setData(string a)
	{
		StaticParamClass.CheckedIn = a;

		Debug.Log(StaticParamClass.MAX_PLACE);
		//StaticParamClass.IsMapUnlocked = new List<bool>(StaticParamClass.MAX_PLACE);
		Debug.Log(StaticParamClass.IsMapUnlocked.Length);

		for (int i = 0; i < StaticParamClass.MAX_PLACE; i++)
		{
			if (a.Contains(i.ToString()))
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

		Debug.LogError($"CheckInPopup set GoFromInside true");
		StaticParamClass.GoFromInside = true;
		// SceneManager.LoadScene(MainController.SCENENAME_MAIN);
	}

	/// <summary>
	/// Gọi khi bấm vào button OK sau khi nhập thông tin
	/// </summary>
	public void CheckinData()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		string name = nickName.text;
		string number = phoneNumber.text;
		// Text validation
		if (!name.Equals("") && name != null)
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
		}
		else
		{
			try
			{
				int place = StaticParamClass.CheckinPlace;
				PlayerPrefs.SetString(StaticParamClass.PrefCheckinName, nickName.text.Trim());
				PlayerPrefs.SetString(StaticParamClass.PrefCheckinNumber, phoneNumber.text.Trim());
				// Send data to Azure Prefab and go to main

				// đăng ký
				PlayFabLogin.RegisterUser(nickName.text.Trim(), phoneNumber.text.Trim());

				Debug.Log("Name: " + PlayerPrefs.GetString("CheckinName"));
				Debug.Log("Number: " + PlayerPrefs.GetString("CheckinNumber"));

				// load data
				StartCoroutine(SetGetUserData.GetCheckedinPlace(phoneNumber.text.Trim(), setData));
			}
			catch (Exception e)
			{
				Debug.LogError($"CheckInpopup Exception {e.Message}");
			}

			Debug.LogError($"CheckInPopup CheckInData name = {name}, number = {number} IsValidated callback close popup now");

			Debug.LogError($"CheckInPopup set GoFromInside true");
			StaticParamClass.GoFromInside = true;

			SetTitleDataRequest title = new SetTitleDataRequest
			{
				Key = phoneNumber.text.Trim(),
				Value = StaticParamClass.CheckedIn + ";" + StaticParamClass.CheckinPlace
			};
			SetGetUserData.SetCheckinPlace(title);
			StaticParamClass.CheckedIn = number;
			OnClosePopupListener?.Invoke();
			gameObject.SetActive(false);
		}

	}


	#region  CheckIN

	public static bool isCheckInCallBackDone = false;

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
		// StaticParamClass.GoFromInside = true;
		isCheckInCallBackDone = true;
	}

	private void OnClosePopup()
	{
		gameObject.SetActive(false);
		OnClosePopupListener?.Invoke();
	}

	public static IEnumerator CheckinPre(string name, string number, int place, Action pCallback)
	{
		isCheckInCallBackDone = false;

		Debug.Log("come here" + name);
		SetGetUserData.GetCheckedinPlace_(number, setData_);
		DelayCallbackCheckInPre(pCallback);
		yield return null;
		//Debug.LogError("go continue");
		//SceneManager.LoadScene(MainController.SCENENAME_MAIN);
	}

	public static async void DelayCallbackCheckInPre(Action pCallback)
	{
		float timeDelay = 0;
		float timeDelayMax = 10;//10s
								// while (timeDelay < timeDelayMax && !isCheckInCallBackDone)
								// {
								// 	await UniTask.DelayFrame(1);
								// 	timeDelay += Time.deltaTime;
								// }

		// if (!isCheckInCallBackDone)
		// {
		// 	Debug.LogError($"CheckInPopup DelayCallbackCheckInPre isCheckInCallBackDone = false => back to main");
		// 	StaticParamClass.GoFromInside = true;
		// }

		Debug.LogError($"CheckInpopup set GoFromInside false");
		StaticParamClass.GoFromInside = true;
		pCallback?.Invoke();
	}
	#endregion
}

