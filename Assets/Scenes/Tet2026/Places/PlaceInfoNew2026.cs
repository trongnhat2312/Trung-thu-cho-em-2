using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunt.Places
{
	public class PlaceInfoNew2026 : MonoBehaviour
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

		[SerializeField] PaceInfoUIBase placeInfoUIBase;
		[SerializeField] List<PageData> listPages = new List<PageData>();

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
			placeInfoUIBase.AddOnBtnOkClickedListener(OnOkClicked);
			placeInfoUIBase.AddOnBtnQRClickedListener(OnQRClicked);
			placeInfoUIBase.AddOnBtnBackClickedListener(OnBackClicked);
			placeInfoUIBase.AddOnBtnNextClickedListener(OnNextClicked);
			placeInfoUIBase.AddOnCloseClickedListener(ClosePopup);
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
				SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
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
			bool isOnQRBtn = m_OpenQRCallback != null;
			placeInfoUIBase.SetONOFFQRBtn(isOnQRBtn);
			gameObject.SetActive(true);
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
			placeInfoUIBase.CheckNextBack(pageId, N_Page);
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
			m_Callback?.Invoke();
			gameObject.SetActive(false);
		}

		private int placeNum;
	}
}
