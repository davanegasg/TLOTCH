using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionS : MonoBehaviour
{
    [SerializeField] List<QuestionBase> _base;

    public Question Questions { get; set; }
    int countQuestion = 0;
    public void Setup()
    {
        Questions  = new Question(_base[0]);
        
    }
    public void updateQuestion()
    {
        
        Questions = new Question(_base[countQuestion]);
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
}
