using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionS : MonoBehaviour
{
    [SerializeField] QuestionBase _base;

    public Question Questions { get; set; }
    public void Setup()
    {
        Questions  = new Question(_base);
        
    }
}
