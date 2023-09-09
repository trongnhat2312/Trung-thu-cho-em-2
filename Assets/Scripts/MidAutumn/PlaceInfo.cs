using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaceInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetPlaceNum(int placeNum)
	{
		this.placeNum = placeNum;
		Title.text = "Bản đồ số " + (placeNum + 1);
	}

	public TMP_Text Title;
	private int placeNum;
}
