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


    public void OpenPlaceInfo(int i, bool isUnlocked)
    {
	    numPlace = i;
		int offset = isUnlocked ? StaticParamClass.MAX_PLACE : 0;
	    var placeInfo = Instantiate(PlaceInfos[i + offset]);
		placeInfo.transform.SetParent(transform, false);
		placeInfo.name = "PlaceInfo " + i;
		if (!isUnlocked)
		{
			QuestionButton.gameObject.SetActive(false);
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
