using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Monsters/Create new question")]
public class QuestionBase : ScriptableObject
{
    [SerializeField] string _name;

    [TextArea]
    [SerializeField] string description;
    [TextArea]
    [SerializeField] string feedback;
    [SerializeField] PossibleAnswer correctAnswer;

    [SerializeField] List<PossibleAnswer> answers;

    public string Name { get { return _name; } }
    public string Description { get { return description; } }
    public string Feedback { get { return feedback; } }
    public List<PossibleAnswer> Answers { get { return answers; } }
    public PossibleAnswer CorrectAnswer { get { return correctAnswer; } }
}
[System.Serializable]

public class PossibleAnswer
{
    [SerializeField] AnswerBase answerBase;
    public AnswerBase Base { get { return answerBase; } }
}


