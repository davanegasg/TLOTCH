using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{

    public AnswerBase Base { get; set; }
    public Answer(AnswerBase aBase)
    {
        Base = aBase;
    }
}