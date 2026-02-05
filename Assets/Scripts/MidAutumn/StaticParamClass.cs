

using UnityEngine;

public static class StaticParamClass
{
	public static string CheckinName { get; set; }

	public static string CheckinNumber { get; set; }

private static int _checkinPlace = -1;

public static int CheckinPlace
{
    get
    {
        Debug.LogError($"[CheckinPlace][GET] value = {_checkinPlace}");
        return _checkinPlace;
    }
    set
    {
        Debug.LogError($"[CheckinPlace][SET] old = {_checkinPlace}, new = {value}");
        _checkinPlace = value;
    }
}


	public static int MAX_PLACE = 6;
	public static string PrefCheckinName = "CheckinName";
	public static string PrefCheckinNumber = "CheckinNumber";
	public static bool GoFromInside = false;

	public static string CheckedIn = "";
	public static bool[] IsMapUnlocked = new bool[MAX_PLACE];

	public static bool GoFromOutside = false;
	public static bool DaCheckRoi = false;

}
