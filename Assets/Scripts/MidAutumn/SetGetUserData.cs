using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ServerModels;
using UnityEngine;

public static class SetGetUserData
{
	public static void SetCheckinPlace(SetTitleDataRequest titleDataRequest)
	{
		if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
		{
			PlayFabSettings.staticSettings.TitleId = "E4842";
		}

		PlayFabServerAPI.SetTitleData(titleDataRequest,
			result =>
			{
				Debug.Log("Set title data success:" + result.ToString());
			},
			error =>
			{
				Debug.LogError("Got error:" + error.ToString());
			});
	}

	public static IEnumerator GetCheckedinPlace(string username, Checkin.ResFromGet callback)
	{
		PlayFabServerAPI.GetTitleData(
			new GetTitleDataRequest(),
			result => 
			{
				Debug.Log("Get title data success:" + result.Data[username]);
				if (result.Data != null && result.Data.ContainsKey(username))
				{
					StaticParamClass.CheckedIn = result.Data[username];
					callback(result.Data[username]);
				}
			},
			error =>
			{
				Debug.LogError("Got error Get data:" + error.ToString());
			});
		yield return null;
	}

	public static IEnumerator GetCheckedinPlace_(string username, Checkin.ResFromGet_ callback)
	{
		PlayFabServerAPI.GetTitleData(
			new GetTitleDataRequest(),
			result =>
			{
				Debug.Log("Get title data success:" + result.Data[username]);
				if (result.Data != null && result.Data.ContainsKey(username))
				{
					StaticParamClass.CheckedIn = result.Data[username];
					callback(result.Data[username], username);
				}
			},
			error =>
			{
				Debug.LogError("Got error Get data:" + error.ToString());
			});
		yield return null;
	}

}
