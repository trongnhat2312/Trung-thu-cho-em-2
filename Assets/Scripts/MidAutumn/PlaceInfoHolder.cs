using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceInfoHolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenPlaceInfo(int i, bool isUnlocked)
    {
		int offset = isUnlocked ? StaticParamClass.MAX_PLACE : 0;
	    var placeInfo = Instantiate(PlaceInfos[i + offset]);
		placeInfo.transform.SetParent(transform, false);
		placeInfo.name = "PlaceInfo " + i;
    }

    public void ClosePopup()
    {
	    MainController.Instance.ClosePlaceInfo();
    }

	public GameObject[] PlaceInfos;
}
