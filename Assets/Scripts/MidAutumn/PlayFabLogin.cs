using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public static class PlayFabLogin
{
	public static void RegisterUser(string name, string phone)
    {
        if(string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
		{
            PlayFabSettings.staticSettings.TitleId = "E4842";
		}
        var request = new RegisterPlayFabUserRequest { Username = phone, DisplayName= name, Password="abc12345", RequireBothUsernameAndEmail=false};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

	private static void OnRegisterSuccess(RegisterPlayFabUserResult obj)
	{
		Debug.Log("Register successfully: " + obj.ToString());
	}


    private static void OnRegisterFailure(PlayFabError error)
	{
		Debug.LogError("Register successfully: " + error.ToString());
	}
}
