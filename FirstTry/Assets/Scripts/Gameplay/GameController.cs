using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle,Paused,Dialog,Cutscene}

public class GameController : MonoBehaviour
{
    
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;
    GameState previousState; 

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;
        playerController.OnEnterTrainersView += (Collider2D monsterCollider) =>
         {
             var monster =monsterCollider.GetComponentInParent<MonsterController>();
             if (monster != null)
             {
                 
                 state = GameState.Cutscene;
                 StartCoroutine(monster.TriggerMonsterBattle(playerController));
             }
         };
        DialogManager.Instance.OnShowDialog += () =>
         {
             state = GameState.Dialog;
         };
        DialogManager.Instance.OnCloseDialog += () =>
          {
              if(state==GameState.Dialog)
                state = GameState.FreeRoam;
          };
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        var myParty = playerController.GetComponent<MyParty>();
        var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildMonster();
        battleSystem.StartBattle(myParty,wildMonster);
    }
    MonsterController monster;
     public void StartSetBattle(MonsterController monster)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        this.monster = monster;
        var myParty = playerController.GetComponent<MyParty>();
        var wildMonster = monster.GetComponent<MyParty>();
        battleSystem.StartBattle(myParty, wildMonster.getMyself());
    }

    public void EndBattle(bool won)
    {
        if (monster != null&&won==true)
        {
            monster.BattleLoss();
            monster = null;
        }
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    public void PauseGame(bool pause)
    {
        if(pause)
        {
            previousState = state;
            state = GameState.Paused;
        }
        else
        {
            state = previousState;
        }
    }

   
    private void Update()
    {
        
        if (state== GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state== GameState.Dialog)
        {
           DialogManager.Instance.HandleUpdate() ;
        }
    }
    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }
}
