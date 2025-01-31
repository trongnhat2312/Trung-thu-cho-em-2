using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    public void OpenPlaceInfo(int i, bool isUnlocked, Action callback = null, Action openQRCallback = null)
    {
        Debug.Log($"PlaceInfoHolder: open place == {i}");
        numPlace = i;
		int offset = isUnlocked ? StaticParamClass.MAX_PLACE : 0;
	    var placeInfo = Instantiate(PlaceInfos[i + offset]);
		placeInfo.transform.SetParent(transform, false);
		placeInfo.name = "PlaceInfo " + i;
        QuestionButton.gameObject.SetActive(isUnlocked && i != 0 && i != 5);

        try
        {
            placeInfo.GetComponent<PlaceInfo>().Open(callback, openQRCallback);
        }
        catch (Exception exception)
        {
        }
    }

    public void ClosePopup()
    {
	    MainController.Instance.ClosePlaceInfo();
    }

    public void OpenQuestion()
    {
		var question = Instantiate(Question);
        question.GetComponent<Question>().SetPlaceInfoHolder(numPlace);
		question.transform.SetParent(transform.parent.transform, false);
		question.name = "Question";
    }

	public GameObject[] PlaceInfos;

	public GameObject Question;

	private int numPlace;
	public Button QuestionButton;
    public int getNumPlace() { return numPlace; }
}
