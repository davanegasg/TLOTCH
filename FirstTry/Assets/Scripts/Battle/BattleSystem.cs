using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, Question }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] QuestionS question;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentAnswer;
    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerUnit.Monster);
        playerHUD.SetData(playerUnit.Monster);
        enemyUnit.Setup(enemyUnit.Monster);
        question.Setup();
        enemyHUD.SetData(enemyUnit.Monster);
        dialogBox.SetMoveNames(playerUnit.Monster.Moves);
        dialogBox.SetAnswers(question.Questions.Answers);
        yield return StartCoroutine(dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Description}"));
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    void QuestionMove()
    {
        state = BattleState.Question;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableAnswerSelector(true);

    }
    IEnumerator PerformPlayerMove()
    {

        state = BattleState.Busy;
        var move = playerUnit.Monster.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} used {move.Base.Name}");
        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        bool isFainted = enemyUnit.Monster.TakeDamage(move, playerUnit.Monster);
        yield return enemyHUD.UpdateHP();

        if(isFainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.name } fainted");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

    }
    IEnumerator CorrectAnswer()
    {
        state = BattleState.Busy;
        var answer = question.Questions.Answers[currentAnswer];
        yield return dialogBox.TypeDialog($"You answered {answer.Base.Name}");
        yield return new WaitForSeconds(1f);

        if (answer.Base==question.Questions.Base.CorrectAnswer.Base)
        {
            yield return dialogBox.TypeDialog($"Correct answer!");
            yield return new WaitForSeconds(1f);
            StartCoroutine(PerformPlayerMove());
        }
        else
        {
            yield return dialogBox.TypeDialog($"Wrong answer!");
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog($"{question.Questions.Base.Feedback} press Z to continue");
            yield return StartCoroutine(WaitForKeyDown(KeyCode.Z));
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
        state = BattleState.EnemyMove;
        var move = enemyUnit.Monster.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} used {move.Base.Name}");
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerUnit.PlayHitAnimation();
        bool isFainted = playerUnit.Monster.TakeDamage(move, enemyUnit.Monster);
        yield return playerHUD.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.name } fainted");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            OnBattleOver(false);

        }
        else
        {
            PlayerAction();
        }
    }

    public void HandleUpdate()
    {
        if(state== BattleState.PlayerAction)
        {
            HandleActionSelector();
        }
        else if(state== BattleState.PlayerMove)
        {
            HandleMoveSelector();
        }
        else if(state== BattleState.Question)
        {
            HandleAnswerSelector();
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
                PlayerMove();

            }
            else if(currentAction==1)
            {
                //Run
            }
        }
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
        dialogBox.UpdateAnswerSelection(currentAnswer,question.Questions);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            
            dialogBox.EnableAnswerSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(CorrectAnswer());
        }
    }
}
