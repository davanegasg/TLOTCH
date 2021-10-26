using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] GameObject answerSelector;
    [SerializeField] GameObject question;
    [SerializeField] List<Text> answerTexts;
    [SerializeField] Text questionText;

    Color highlightedColor;

    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighlightedColor;
    }

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    public void EnableAnswerSelector(bool enabled)
    {
        answerSelector.SetActive(enabled);
        question.SetActive(enabled);
    }
    public void UpdateActionSelection(int selectedAction)
    {
        for(int i=0; i<actionTexts.Count; ++i)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;

            }
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;

            }
            else
                moveTexts[i].color = Color.black;
        }
        ppText.text = $"PP{move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
        ppText.color = Color.red;
        else
        ppText.color= Color.black;
    }

    public void UpdateAnswerSelection(int selectedAnswer,Question question)
    {
        for (int i = 0; i < answerTexts.Count; ++i)
        {
            if (i == selectedAnswer)
            {
                answerTexts[i].color = highlightedColor;

            }
            else
                answerTexts[i].color = Color.black;

        }
        questionText.text = $"{question.Base.Description}";
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

    public void SetAnswers(List<Answer> answers)
    {
        Randomize(answers);
        for(int i = 0; i < answerTexts.Count; i++)
        {
            if (i < answers.Count)
            {
                answerTexts[i].text = answers[i].Base.Name;
            }
            else
            {
                answerTexts[i].text = "-";
            }
        }

    }

    public List<Answer> Randomize(List<Answer> answers)
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < moveTexts.Count; i++)
        {
            int j = rand.Next(i, moveTexts.Count);
            Answer temp = answers[i];
            answers[i] = answers[j];
            answers[j] = temp;
        }
        return answers;
    }

}
