using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceInfo : MonoBehaviour
{
	[Serializable]
	public class PageData
	{
		public GameObject content;
		public GameObject infoData;

		public void SetVisible(bool isVisible)
		{
			content.SetActive(isVisible);
			infoData.SetActive(isVisible);
		}
	}

	[SerializeField]
	List<PageData> listPages = new List<PageData>();

	[SerializeField]
	Button m_OkButton;

	[SerializeField]
	Button m_QRButton;

	[SerializeField]
	Button m_BackButton;

	[SerializeField]
	Button m_NextButton;

	int pageId = 0;

	// Start is called before the first frame update
	void Start()
	{
		InitListener();

		SelectPage();

		CheckNextBack();
	}

	void InitListener()
	{
		Debug.Log($"PlaceInfo: place == {name}, Add listener");

		try
		{
			if (m_OkButton != null)
			{
				m_OkButton.onClick.AddListener(OnOkClicked);
			}
			
			if (m_QRButton != null)
			{
				Debug.Log($"PlaceInfo: place == {name}, Add QR Clicked listener");
				m_QRButton.onClick.AddListener(OnQRClicked);
			}

			if (m_NextButton != null)
			{
				m_NextButton.onClick.AddListener(OnNextClicked);
			}

			if (m_BackButton != null)
			{
				m_BackButton.onClick.AddListener(OnBackClicked);
			}
		}
		catch (Exception exception)
		{
		}
	}

	void OnOkClicked()
	{
		if (N_Page > 0 && pageId < N_Page - 1)
		{
			OnNextClicked();
		}
		else
		{
			ClosePopup();
		}
	}

	void OnQRClicked()
	{
		Debug.Log($"PlaceInfo: place == {name}, On QR Clicked");
		try
		{
			MainController.Instance.ClosePlaceInfo();
		}
		catch (Exception exception)
		{
		}

		m_OpenQRCallback?.Invoke();
	}

	Action m_Callback;
	Action m_OpenQRCallback;
	public void Open(Action callback, Action openQRCallback = null)
	{
		Debug.Log($"PlaceInfo: open place == {name}, callback = {callback != null}, openQRCallback = {openQRCallback != null}");
		m_Callback = callback;
		m_OpenQRCallback = openQRCallback;
		if (m_OpenQRCallback != null && m_QRButton.gameObject != null)
		{
			m_QRButton.gameObject.SetActive(true);
		}
	}

	int N_Page => listPages != null ? listPages.Count : 0;

	void OnNextClicked()
	{
		pageId++;
		pageId = Mathf.Min(pageId, N_Page - 1);

		SelectPage();

		CheckNextBack();
	}

	void OnBackClicked()
	{
		pageId--;
		pageId = Mathf.Max(pageId, 0);

		SelectPage();

		CheckNextBack();
	}

	void CheckNextBack()
	{
		try
		{
			m_NextButton.gameObject.SetActive(pageId < N_Page - 1);
			m_BackButton.gameObject.SetActive(pageId > 0);
		}
		catch (Exception exception)
		{
		}
	}

	void SelectPage()
	{
		for (int i = 0; i < N_Page; i++)
		{
			try
			{
				listPages[i].SetVisible(i == pageId);
			}
			catch (Exception exception)
			{
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetPlaceNum(int placeNum)
	{
		this.placeNum = placeNum;
		//Title.text = "Bản đồ số " + (placeNum + 1);
	}

	public void ClosePopup()
	{
		try
		{
			MainController.Instance.ClosePlaceInfo();
		}
		catch (Exception exception)
		{
		}

		m_Callback?.Invoke();
	}

	public TMP_Text Title;
	private int placeNum;
}
