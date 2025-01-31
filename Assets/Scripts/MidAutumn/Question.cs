using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
	[SerializeField] GameObject checkCorrectRoot;

    // Start is called before the first frame update
    void Start()
    {
		if(QuestionConfig.Categories[numPlace].Questions.Count > 0)
		{
			QuestionContent.text = QuestionConfig.Categories[numPlace].Questions[0].Question;
			maxQuestion = QuestionConfig.Categories[numPlace].Questions.Count;

			ListAnswers[0].text = QuestionConfig.Categories[numPlace].Questions[0].Answer1;
			ListAnswers[1].text = QuestionConfig.Categories[numPlace].Questions[0].Answer2;
			ListAnswers[2].text = QuestionConfig.Categories[numPlace].Questions[0].Answer3;
			ListAnswers[3].text = QuestionConfig.Categories[numPlace].Questions[0].Answer4;
			count = 0;
		} else
		{
			ClosePopup();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClosePopup()
    {
		Destroy(gameObject);
    }

    public void ChooseAnswer(int i)
    {
		Debug.Log(count);
        if(count < maxQuestion)
		{
			if (i == QuestionConfig.Categories[numPlace].Questions[count].CorrectIndex)
			{
				// Chinh' xac'
				ShowResult(true, i, i);
			}
			else
			{
				// Sai roi
				ShowResult(false, i, QuestionConfig.Categories[numPlace].Questions[count].CorrectIndex);
			}
			count++;
			////Go to next question
			if(count == maxQuestion)
			{
				//GoToNext.SetActive(false);

			}
		}
	}

	public void ShowResult(bool isCorrect,int selectedIndex, int correctIndex)
	{
		try
		{
			checkCorrectRoot.gameObject.SetActive(true);
			checkCorrectRoot.transform.localPosition = Answers[correctIndex].transform.localPosition;
		}
		catch (System.Exception exception)
		{
		}

		GoToNext.SetActive(true);
		if (isCorrect)
		{
			for(int i = 0; i < Answers.Length; i++)
			{
				if(i != selectedIndex)
				{
					Answers[i].SetActive(false);
				}
			}
			ListAnswers[selectedIndex].color = Color.blue;
		} else
		{
			for (int i = 0; i < Answers.Length; i++)
			{
				if (i != selectedIndex && i!= correctIndex)
				{
					Answers[i].SetActive(false);
				}
			}
			ListAnswers[correctIndex].color = Color.blue;
			ListAnswers[selectedIndex].color = Color.red;
		}
	}

    public void GoToNextQuestion()
	{
		if(count < maxQuestion)
		{
			for(int i = 0; i < ListAnswers.Length; i++)
			{
				ListAnswers[i].color = Color.black;
				Answers[i].SetActive(true);
			}

			QuestionContent.text = QuestionConfig.Categories[numPlace].Questions[count].Question;
			ListAnswers[0].text = QuestionConfig.Categories[numPlace].Questions[count].Answer1;
			ListAnswers[1].text = QuestionConfig.Categories[numPlace].Questions[count].Answer2;
			ListAnswers[2].text = QuestionConfig.Categories[numPlace].Questions[count].Answer3;
			ListAnswers[3].text = QuestionConfig.Categories[numPlace].Questions[count].Answer4;
		} else
		{
			// Ket thuc
			ClosePopup();
		}

		// Set lai hieu ung
	}

    public void SetPlaceInfoHolder(int numPlace)
	{
        this.numPlace = numPlace;
	}

    private int numPlace = 0;
    private int count = 0;
    private int maxQuestion = 0;

    public Text QuestionContent;
	public int CorrectAnswer;
    public GameObject[] Answers;
    public Text[] ListAnswers;
    public GameModel QuestionConfig;
	public GameObject GoToNext;
}
