using System;

namespace TreasureHunt.Data
{

    [Serializable]
    public class UserData
    {
        public string userName;
        public string phoneNumber;
        public bool isFirstScan = true;

        public bool IsCheckInDone => !string.IsNullOrEmpty(userName);
        public bool IsFirstScan => isFirstScan;


        public void InitFirstData()
        {
            userName = "";
            phoneNumber = "";
            isFirstScan = true;
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
    }

}
