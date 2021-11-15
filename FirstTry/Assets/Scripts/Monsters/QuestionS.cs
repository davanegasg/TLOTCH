using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionS : MonoBehaviour
{
    [SerializeField] List<QuestionBase> _base;

    public Question Questions { get; set; }
    int countQuestion = 0;
    List<QuestionBase> randomizedListOfQuestions;
    public void Setup()
    {
        randomizedListOfQuestions = Randomize(_base);
        Questions  = new Question(randomizedListOfQuestions[0]);
        
    }
    public void updateQuestion()
    {
        
        Questions = new Question(randomizedListOfQuestions[countQuestion]);
    }
    public void AddCount()
    {
        if (countQuestion < _base.Count-1)
        {
            
            countQuestion++;
        }
        else
        {
            countQuestion = 0;
        }
        
    }

    public List<QuestionBase> Randomize(List<QuestionBase> questions)
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < questions.Count; i++)
        {
            int j = rand.Next(i, questions.Count);
            QuestionBase temp = questions[i];
            questions[i] = questions[j];
            questions[j] = temp;
        }
        return questions;
    }
}
