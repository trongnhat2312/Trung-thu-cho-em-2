using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunt.Data
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadData();
            }
        }


        private void LoadData()
        {
            LoadUserData();
        }

        private const string USER_DATA_KEY = "USER_DATA_KEY";
        public UserData _userData;
        public static UserData userData => Instance._userData;

        public static bool IsFirstScan => userData.IsFirstScan;

        public static string UserName => userData.UserName;
        public static string PhoneNumber => userData.PhoneNumber;
        public static List<int> PlaceUnlocked => userData.PlaceUnlocked;

        public static void UpdateCheckInData(string userName, string phoneNumber)
        {
            userData.UpdateCheckInData(userName, phoneNumber);
            Instance.SaveUserData();
        }

        public static void UpdateFirstScaned()
        {
            userData.UpdateFirstScaned();
            Instance.SaveUserData();
        }


        public static void UpdatePlaceUnlocked(int pId)
        {
            userData.UpdatePlaceUnlocked(pId);
            Instance.SaveUserData();
        }

        public static bool IsPlaceUnlocked(int pId)
        {
            bool result = userData.IsPlaceUnlocked(pId);
            return result;
        }

        public static bool IsCheckInDone => userData.IsCheckInDone;

        private void LoadUserData()
        {
            string json = PlayerPrefs.GetString(USER_DATA_KEY, "");
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("DataManager Userdata first create");
                Instance._userData = new UserData();
                Instance._userData.InitFirstData();
            }
            else
            {
                Debug.LogError("DataManager Exist Userdata");
                Instance._userData = JsonUtility.FromJson<UserData>(json);
            }
        }

        private void SaveUserData(bool real = true)
        {
            if (real)
            {
                PlayerPrefs.SetString(USER_DATA_KEY, JsonUtility.ToJson(userData));
                PlayerPrefs.Save();
            }
        }
    }
}
