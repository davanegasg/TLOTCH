using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Menu, Paused, Dialog, Cutscene }

public class GameController : MonoBehaviour
{
    
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;
    GameState previousState;
     

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();
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
          menuController.onBack += () =>
          {
              state= GameState.FreeRoam;
          };

          menuController.onMenuSelected += OnMenuSelected;

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
            monster.BattleLost();
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

            if(Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }     
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state== GameState.Dialog)
        {
           DialogManager.Instance.HandleUpdate() ;
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
    }
    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //SAVE
            SavingSystem.i.Save("saveSlot1");
        }
        else if (selectedItem == 1)
        {
            //LOAD
            SavingSystem.i.Load("saveSlot1");
        }
    state = GameState.FreeRoam;
    
    }
}
