using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QuestionContent.text = QuestionConfig.Categories[numPlace].Questions[0].Question;
        maxQuestion = QuestionConfig.Categories[numPlace].Questions.Count;

		ListAnswers[0].text = QuestionConfig.Categories[numPlace].Questions[0].Answer1;
		ListAnswers[1].text = QuestionConfig.Categories[numPlace].Questions[0].Answer2;
		ListAnswers[2].text = QuestionConfig.Categories[numPlace].Questions[0].Answer3;
		ListAnswers[3].text = QuestionConfig.Categories[numPlace].Questions[0].Answer4;
        count = 0;
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
        if(count < maxQuestion)
		{
			if (i == QuestionConfig.Categories[numPlace].Questions[count].CorrectIndex)
			{
				// Chinh' xac'
			}
			else
			{
				// Sai roi
			}
			count++;
			//Go to next question
			GoToNextQuestion(count);
		} else
		{
			ClosePopup();
		}
	}

    public void GoToNextQuestion(int questionIndex)
	{
        QuestionContent.text = QuestionConfig.Categories[numPlace].Questions[questionIndex].Question;
		ListAnswers[0].text = QuestionConfig.Categories[numPlace].Questions[questionIndex].Answer1;
		ListAnswers[1].text = QuestionConfig.Categories[numPlace].Questions[questionIndex].Answer2;
		ListAnswers[2].text = QuestionConfig.Categories[numPlace].Questions[questionIndex].Answer3;
		ListAnswers[3].text = QuestionConfig.Categories[numPlace].Questions[questionIndex].Answer4;
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
}
