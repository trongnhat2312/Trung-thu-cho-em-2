using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    public int CorrectAnswer;
    public GameObject[] Answers;
}
