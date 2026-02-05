using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StickerSort.MenuGame
{
    [Serializable]
    public class MenuLoaderData
    {
        public MenuDailyChallengeData menu_daily_challenge_data = new();
    }

    [Serializable]
    public class MenuDailyChallengeData
    {
        public bool is_completed_dailychallenge = false;
        public int id_challenge = 0;
    }


}
