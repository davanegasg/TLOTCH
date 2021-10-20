using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, Question,BattleOver,Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] QuestionS questions;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentAnswer;
    MyParty me;
    Monster wildMonster;
    public void StartBattle(MyParty me, Monster wildMonster)
    {
        this.me = me;
        this.wildMonster = wildMonster;
        StartCoroutine(SetupBattle());
    }



    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(me.getMyself());
        enemyUnit.Setup(wildMonster);
        questions.Setup();
        dialogBox.SetMoveNames(playerUnit.Monster.Moves);
        dialogBox.SetAnswers(questions.Questions.Answers);
        yield return StartCoroutine(dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Description}"));
        yield return new WaitForSeconds(1f);

        ActionSelection();
    }

    void BattleOver(bool won)
    {
        state= BattleState.BattleOver;
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    void QuestionMove()
    {
        state = BattleState.Question;
        dialogBox.SetAnswers(questions.Questions.Answers);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableAnswerSelector(true);

    }
    IEnumerator PlayerMove()
    {

        state = BattleState.PerformMove;
        var move = playerUnit.Monster.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
        
        
        

    }
    IEnumerator CorrectAnswer()
    {
        state = BattleState.Busy;
        var answer = questions.Questions.Answers[currentAnswer];
        yield return dialogBox.TypeDialog($"You answered {answer.Base.Name}");
        yield return new WaitForSeconds(1f);

        if (answer.Base==questions.Questions.Base.CorrectAnswer.Base)
        {
            yield return dialogBox.TypeDialog($"Correct answer!");
            yield return new WaitForSeconds(1f);
            questions.AddCount();
            questions.updateQuestion();
            StartCoroutine(PlayerMove());
        }
        else
        {
            yield return dialogBox.TypeDialog($"Wrong answer!");
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog($"{questions.Questions.Base.Feedback} press Z to continue");
            yield return StartCoroutine(WaitForKeyDown(KeyCode.Z));
            questions.AddCount();
            questions.updateQuestion();
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
    }
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        var move = enemyUnit.Monster.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);
        if (state == BattleState.PerformMove)
            ActionSelection();
        
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
       
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();
        bool isFainted = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
        yield return targetUnit.Hud.UpdateHP();

        if (isFainted)
        {
            if(targetUnit.Monster.Base.name == "Myself")
            {
                yield return dialogBox.TypeDialog($"You have fainted");
            }
            else
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.name } fainted");
            }
            
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            CheckForBattleover(targetUnit);
        }
    }

    void CheckForBattleover(BattleUnit faintedUnit)
    {
        if(faintedUnit.IsPlayerUnit)
        {
            BattleOver(false);
        }
        else
        {
            BattleOver(true);
        }
    }    
    public void HandleUpdate()
    {
        if(state== BattleState.ActionSelection)
        {
            HandleActionSelector();
        }
        else if(state== BattleState.MoveSelection)
        {
            HandleMoveSelector();
        }
        else if(state== BattleState.Question)
        {
            HandleAnswerSelector();
        }
        else if(state== BattleState.Run)
        {

        }
    }

    void HandleActionSelector()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow)||Input.GetKeyDown(KeyCode.S))
        {
            if(currentAction<1)
            {
                ++currentAction;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if(currentAction>0)
            {
                --currentAction;
            }
        }
        dialogBox.UpdateActionSelection(currentAction);
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction==0)
            {
                //Fight
                MoveSelection();

            }
            else if(currentAction==1)
            {
                //Run
                StartCoroutine(Run());
            }
        }
    }
    IEnumerator Run()
    {
        state = BattleState.Run;
        dialogBox.EnableActionSelector(false);
        yield return dialogBox.TypeDialog($"You have escaped the battle");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"press Z to continue");
        yield return StartCoroutine(WaitForKeyDown(KeyCode.Z));
        BattleOver(false);
        
    }

    void HandleMoveSelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentMove < playerUnit.Monster.Moves.Count - 1)
            {
                ++currentMove;
            }

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentMove > 0)
            {
                --currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentMove < playerUnit.Monster.Moves.Count - 2)
            {
                currentMove += 2;
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.Moves[currentMove]);
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableAnswerSelector(true);
            QuestionMove();
        }
    }

    void HandleAnswerSelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentAnswer < 3)
            {
                ++currentAnswer;
            }

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentAnswer > 0)
            {
                --currentAnswer;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAnswer < 2)
            {
                currentAnswer += 2;
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAnswer > 1)
            {
                currentAnswer -= 2;
            }
        }
        dialogBox.UpdateAnswerSelection(currentAnswer,questions.Questions);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            
            dialogBox.EnableAnswerSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(CorrectAnswer());
        }
    }
}
