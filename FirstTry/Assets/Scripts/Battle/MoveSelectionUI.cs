using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    int currentSelection = 0;


    public void SetMoveData(List<MoveBase> currentmoves, MoveBase newMove)
    {
        for(int i=0;i<currentmoves.Count;i++)
        {
            moveTexts[i].text = currentmoves[i].name;
        }
        moveTexts[currentmoves.Count].text = newMove.name;
    }

    public void HandleMoveSelector(Action<int> onSelected )
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            ++currentSelection;

        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, MonsterBase.MaxNumOfMoves);
        UpdateMoveSelection(currentSelection);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentSelection);
        }
       
    }
    public void UpdateMoveSelection(int selection)
    {
        for(int i=0; i<MonsterBase.MaxNumOfMoves+1;i++)
        {
            if(i==selection)
            {
                moveTexts[i].color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }
}
