using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum BattleState { Start, ActionSelection, MoveSelection,Bag, RunningTurn, Busy, Question,BattleOver,MoveToForget}
public enum BattleAction { Move, UseItem, Run,Defend,WrongAnswer}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] QuestionS questions;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;
    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentAnswer;
    MyParty me;
    Monster wildMonster;
    bool defense;
    bool usedDefense;
    int escapeAttempts;
    bool isSetBattle;
    MoveBase moveToLearn;


    public void StartBattle(MyParty me, Monster wildMonster,PlayerController player)
    {
       
        this.me = me;
        this.wildMonster = wildMonster;
        StartCoroutine(SetupBattle(player));
    }



    public IEnumerator SetupBattle(PlayerController player)
    {
        isSetBattle = GameController.Instance.IsSetBattle;
        usedDefense = false;   
        playerUnit.Setup(me.Myself[0],player);
        enemyUnit.Setup(wildMonster,player);
        questions.Setup();
        dialogBox.SetMoveNames(playerUnit.Monster.Moves);
        dialogBox.SetAnswers(questions.Questions.Answers);
        yield return StartCoroutine(dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Description}"));
        escapeAttempts = 0;
        ActionSelection();
    }
    
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        me.Myself[0].OnBattleOver();
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }

    void ActionSelection()
    {

        StartCoroutine(DefendMove());
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Escoge una accion"));
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

    IEnumerator ChooseMoveToForeget(Monster monster, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"¿Cual vas a olvidar?");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(monster.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleState.MoveToForget;
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        
        if (playerAction== BattleAction.Move)
        {
            
            playerUnit.Monster.CurrentMove = playerUnit.Monster.Moves[currentMove];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            int playerMovePriority = playerUnit.Monster.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Base.Priority;
            //Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;

            else if(enemyMovePriority==playerMovePriority)
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            //First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;
            //Second turn
            if(secondUnit.Monster.HP>0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            
        }
        else
        {
            if (playerAction == BattleAction.Defend)
            {
                StartCoroutine(DefendMove());
                yield return new WaitForSeconds(2f);

            }
            else if (playerAction == BattleAction.UseItem)
            {
                
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                dialogBox.EnableActionSelector(false);
                yield return TryToEscape();
            }
            var enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return RunAfterTurn(playerUnit);
            if (state == BattleState.BattleOver) yield break;
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            defense = false;
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        
        if(state!=BattleState.BattleOver)
            ActionSelection();
    }
    IEnumerator CorrectAnswer()
    {
        state = BattleState.Busy;
        var answer = questions.Questions.Answers[currentAnswer];
        yield return dialogBox.TypeDialog($"Respondiste {answer.Base.Name}");

        if (answer.Base == questions.Questions.Base.CorrectAnswer.Base)
        {
            yield return dialogBox.TypeDialog($"Respuesta correcta!");
            questions.AddCount();
            questions.updateQuestion();
            if (!defense)
            {

               
                StartCoroutine(RunTurns(BattleAction.Move));
            }
            else
            {
                StartCoroutine(RunTurns(BattleAction.Defend));

            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"Respuesta incorrecta!");
            for(int i =0; i<questions.Questions.Base.Feedback.Count;i++)
            {
                yield return dialogBox.TypeDialog($"{questions.Questions.Base.Feedback[i]} ");
                yield return new WaitForSeconds(0.5f);
            }
            yield return dialogBox.TypeDialog($"Presiona z para continuar");
            yield return StartCoroutine(WaitForKeyDown(KeyCode.Z));
            questions.AddCount();
            questions.updateQuestion();
            StartCoroutine(RunTurns(BattleAction.WrongAnswer));
        }
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Monster.OnBeforeMove();

        if(!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Monster);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Monster);
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} uso {move.Base.Name}");
        if (CheckIfMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
        {


            sourceUnit.PlayAttackAnimation();

            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Monster, targetUnit.Monster,move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.SecondaryEffects !=null&& move.Base.SecondaryEffects.Count>0&& targetUnit.Monster.HP>0)
            {
                foreach(var secondary in move.Base.SecondaryEffects)
                {
                    var random = UnityEngine.Random.Range(1, 101);
                    if (random <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Monster, targetUnit.Monster,secondary.Target);
                }
            }
            if (targetUnit.Monster.HP <= 0)
            {

                yield return HandleFainted(targetUnit);

            }
           
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} fallo el ataque");
        }
        
        
    }
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver)
            yield break;
        //Statuses like burn or psn will hurt the monster after the turn
        sourceUnit.Monster.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        //Monster might die after said damage
        if (sourceUnit.Monster.HP <= 0)
        {

            yield return HandleFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);

        }
    }
    IEnumerator RunMoveEffects(MoveEffects effects, Monster source, Monster target,MoveTarget moveTarget)
    {
        
        //Stat boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts, false);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts, false);
            }
        }
        //Status condition
        if(effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        //Volatile status condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    bool CheckIfMoveHits(Move move, Monster source, Monster target)
    {
        if(move.Base.AlwaysHit)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];
        var boostValues = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3 };
        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }
    IEnumerator ShowStatusChanges(Monster monster)
    {
        while(monster.StatusChanges.Count>0)
        {
            var message = monster.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1)
            yield return dialogBox.TypeDialog("Fue un golpe critico!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("Fue muy efectivo!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("No fue muy efectivo...");
            
    }

    IEnumerator HandleFainted(BattleUnit faintedUnit)
    {
        if (faintedUnit.Monster.Base.name == "Myself")
        {
            yield return dialogBox.TypeDialog($"Te has desmayado");
        }
        else
        {
            yield return dialogBox.TypeDialog($"{faintedUnit.Monster.Base.name } se desmayo");
        }
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);
        if (!faintedUnit.IsPlayerUnit)
        {
            //EXP GAIN
            int expYield=faintedUnit.Monster.Base.ExpYield;
            int enemyLevel = faintedUnit.Monster.Level;
            float setBattleBonus = (isSetBattle)? 1.5f : 1f;
            int expGain = Mathf.FloorToInt((expYield * enemyLevel * setBattleBonus) / 7);

            playerUnit.Monster.Exp += expGain;
            yield return dialogBox.TypeDialog($"ganaste {expGain} exp");
            yield return playerUnit.Hud.SetExpSmooth();
            
            
            //CHECK Level up
            while (playerUnit.Monster.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"Subiste de nivel!");

                //Try to learn new move
                var newMove = playerUnit.Monster.GetLearnableMoveAtCurrentLevel();
                if(newMove!=null)
                {
                    if(playerUnit.Monster.Moves.Count<MonsterBase.MaxNumOfMoves)
                    {
                        playerUnit.Monster.LearnMove(newMove);
                        
                        yield return dialogBox.TypeDialog($"En tu batalla ganaste conocimientos ");
                        yield return dialogBox.TypeDialog($"Has aprendido {newMove.Base.name} ");
                        dialogBox.SetMoveNames(playerUnit.Monster.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"En tu batalla ganaste conocimientos");
                        yield return dialogBox.TypeDialog($"Pero solo puedes dominar {MonsterBase.MaxNumOfMoves} movimientos");
                        
                        yield return ChooseMoveToForeget(playerUnit.Monster, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);

                    }
                }
                yield return playerUnit.Hud.SetExpSmooth(true);

            }
            yield return new WaitForSeconds(1f);
        }
        
        CheckForBattleover(faintedUnit);
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
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelector();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelector();
        }
        else if (state == BattleState.Question)
        {
            HandleAnswerSelector();
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == MonsterBase.MaxNumOfMoves)
                {
                    //Don't Learn new move
                    StartCoroutine(dialogBox.TypeDialog($"Decidiste no dominar {moveToLearn.Name}"));
                }
                else
                {
                    //Forget selected move and learn new move
                    var selectedMove = playerUnit.Monster.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($"Decidiste olvidar {selectedMove.Name} y dominar {moveToLearn.Name}"));
                    playerUnit.Monster.Moves[moveIndex] = new Move(moveToLearn);
                }
                moveToLearn = null;
                state = BattleState.RunningTurn;

            };
            moveSelectionUI.HandleMoveSelector(onMoveSelected);
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                if (!DialogManager.Instance.IsShowing)
                {
                    inventoryUI.gameObject.SetActive(false);
                    state = BattleState.ActionSelection;
                }
            };
            Action onItemUsed = () =>
            {
                state = BattleState.Busy;
                inventoryUI.gameObject.SetActive(false);
                StartCoroutine(RunTurns(BattleAction.UseItem));
                
            };
            inventoryUI.HandleUpdate(onBack,onItemUsed);
        }
        
    }
    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);

    }
    void HandleActionSelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentAction;
        
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
           --currentAction;

        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentAction += 2;

        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);
        dialogBox.UpdateActionSelection(currentAction);
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction==0)
            {
                //Fight
                defense = false;
                usedDefense = false;
                MoveSelection();

            }
            else if(currentAction==1)
            {
                //Bag
                OpenBag();
                
            }
            else if (currentAction == 2)
            {
                //Defend
                defense = true;
                usedDefense = true;
                QuestionMove();

            }
            else if (currentAction == 3)
            {
                //Run
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }
   IEnumerator DefendMove()
    {
        
        if (defense)
        {
            bool canRunMove = playerUnit.Monster.OnBeforeMove();

            if (!canRunMove)
            {

                yield return ShowStatusChanges(playerUnit.Monster);
                yield return playerUnit.Hud.WaitForHPUpdate();
                yield break;

            }
            yield return dialogBox.TypeDialog($"Te has defendido!");
            List<StatBoost> Effect = new List<StatBoost>();
            Effect.Add(new StatBoost(Stat.Defense, 6));
            Effect.Add(new StatBoost(Stat.SpDefense, 6));
            playerUnit.Monster.ApplyBoosts(Effect,true);
        }
        else
        {
            if(usedDefense)
            {
                List<StatBoost> Effect = new List<StatBoost>();
                Effect.Add(new StatBoost(Stat.Defense, -6));
                Effect.Add(new StatBoost(Stat.SpDefense, -6));
                playerUnit.Monster.ApplyBoosts(Effect,true);
            }
        }
        
    }


    void HandleMoveSelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentMove;

        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentMove;

        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentMove += 2;

        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Monster.Moves.Count - 1);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.Moves[currentMove]);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Monster.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableAnswerSelector(true);
            QuestionMove();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandleAnswerSelector()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ++currentAnswer;

        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            --currentAnswer;

        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            currentAnswer += 2;

        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            currentAnswer -= 2;

        currentAnswer = Mathf.Clamp(currentAnswer, 0, 3);
       
        dialogBox.UpdateAnswerSelection(currentAnswer,questions.Questions);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            
            dialogBox.EnableAnswerSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(CorrectAnswer());
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            defense = false;
            usedDefense = false;
            dialogBox.EnableAnswerSelector(false);
            dialogBox.EnableDialogText(true);
            MoveSelection();
        }
    }

   
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
       

        if(isSetBattle)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} no te dejara escapar");
            state = BattleState.RunningTurn;
            yield break;
        }
        ++escapeAttempts;
        int playerSpeed = playerUnit.Monster.Speed;
        int enemySpeed = enemyUnit.Monster.Speed;
        if (enemySpeed<=playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Escapaste a salvo!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if(UnityEngine.Random.Range(0,256)<f)
            {
                yield return dialogBox.TypeDialog($"Escapaste a salvo!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"No puedes escapar !");
                state = BattleState.RunningTurn;
            }
        }
    }
}
