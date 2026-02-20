using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunt.Places
{
	public class IntroEvent2026Popup : MonoBehaviour
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

		Action OnClosePopupListener; 
		[SerializeField] PaceInfoUIBase placeInfoUIBase;
		[SerializeField] List<PageData> listPages = new List<PageData>();

		int pageId = 0;

		// Start is called before the first frame update
		void Start()
		{
			InitListener();

			CheckNextBack();
		}

		void InitListener()
		{
			placeInfoUIBase.AddOnBtnOkClickedListener(OnOkClicked); 
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

		public void ShowPopup(Action callback)
		{
			Debug.LogError($"IntroEvent2026Popup  place == {name}, Add listener"); 
			OnClosePopupListener = callback;  

			SelectPage(); 
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
					Debug.LogError($"IntroEvent2026Popup SelectPage exception: {exception.Message}");
				}
			}
		}  

		public void ClosePopup()
		{
			gameObject.SetActive(false);
			OnClosePopupListener?.Invoke();
		} 
	}
}
