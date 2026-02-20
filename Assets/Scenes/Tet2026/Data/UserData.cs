using System;
using System.Collections.Generic;

namespace TreasureHunt.Data
{

    [Serializable]
    public class UserData
    {
        public string userName;
        public string phoneNumber;
        public bool isFirstScan = true;
        public List<int> placeUnlocked = new();

        public bool IsCheckInDone => !string.IsNullOrEmpty(userName);
        public bool IsFirstScan => isFirstScan;
        public string UserName => userName;


        public void InitFirstData()
        {
            userName = "";
            phoneNumber = "";
            isFirstScan = true;
            placeUnlocked = new();
        }


        public void UpdateCheckInData(string userName, string phoneNumber)
        {
            this.userName = userName;
            this.phoneNumber = phoneNumber;
        }

        public void UpdateFirstScaned()
        {
            isFirstScan = false;
        }

        public void UpdatePlaceUnlocked(int pId)
        {
            placeUnlocked.Add(pId);
        }

        public bool IsPlaceUnlocked(int pId)
        {
            bool result = false;
            foreach (var item in placeUnlocked)
            {
                if (item == pId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }

}
