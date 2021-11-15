using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question 
{
    public QuestionBase Base { get; set; }
    public List<Answer> Answers { get; set; }

    public Question(QuestionBase mBase)
    {
        Base = mBase;
        Answers = new List<Answer>();
        foreach(var answer in Base.Answers)
        {
            Answers.Add(new Answer(answer.Base));
        }

    }

    


}
