using System;
using System.Collections.Generic;

public static class StaticParamClass
{
	public static string CheckinName { get; set; }

	public static string CheckinNumber { get; set; }

	public static int CheckinPlace { get; set; } = -1;


	public static int MAX_PLACE = 6;
	public static string PrefCheckinName = "CheckinName";
	public static string PrefCheckinNumber = "CheckinNumber";

	public static string CheckedIn = "";
	public static List<bool> IsMapUnlocked = new List<bool>(MAX_PLACE);

}
