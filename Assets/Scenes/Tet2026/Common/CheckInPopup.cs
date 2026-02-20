using UnityEngine; 
using UnityEngine.UI; 
using System;
using TreasureHunt.Data;

public class CheckInPopup : MonoBehaviour
{
	private Action OnClosePopupListener;

	public InputField nickName;
	public InputField phoneNumber;
	public Text ErrorName;
	public Text ErrorPhone;
	[SerializeField] Button btnOk;
 

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

	/// <summary>
	/// Gọi khi bấm vào button OK sau khi nhập thông tin
	/// </summary>
	public void CheckinData()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		string name = nickName.text;
		string number = phoneNumber.text;
		
		bool isValid = IsUserDataValidate(name, number);
		if(isValid)
		{
			DataManager.UpdateCheckInData(name, number);
			OnClosePopupListener?.Invoke();
			PushUserCheckInDataToPlayFab(name, number);
			HidePopup();
		}
	}

	private bool IsUserDataValidate(string userName, string phoneNumber)
	{
		bool result = false;
		// Text validation 

		bool isNameValid = !string.IsNullOrEmpty(userName);
		bool isPhoneValid = !string.IsNullOrEmpty(phoneNumber);

		result = isNameValid && isPhoneValid;

		ErrorName.text = "Error: UserName cannot be empty";
		ErrorPhone.text = "Error: PhoneNumber cannot be empty";
		ErrorName.gameObject.SetActive(!isNameValid);
		ErrorPhone.gameObject.SetActive(!isPhoneValid);

		return result;
	}

	private void HidePopup()
	{
		gameObject.SetActive(false);
	}

	private void PushUserCheckInDataToPlayFab(string name, string number)
	{ 
		// đăng ký
		PlayFabLogin.RegisterUser(nickName.text.Trim(), phoneNumber.text.Trim());
	} 
}

