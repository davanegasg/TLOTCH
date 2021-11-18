using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState { FreeRoam, Battle, Menu, Paused, Dialog, Cutscene, Inventory, StartScreen }

public class GameController : MonoBehaviour,ISavable
{
    
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] StartScreen startScreen;
    GameState state;
    GameState previousState;
    bool isSetBattle;
    float gameTime = 0;


    public SceneDetails CurrentScene { get; private set; }
    public bool IsSetBattle { get { return isSetBattle; } }
    public SceneDetails PrevScene { get; private set; }
    public PlayerController PlayerController { get; }

    MenuController menuController;

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        StartGame();
        Instance = this;
        MonstersDB.Init();
        ConditionsDB.Init();
        ItemDB.Init();
        MoveDB.Init();
        QuestDB.Init();
        menuController = GetComponent<MenuController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
             previousState = state;
             state = GameState.Dialog;
         };
        DialogManager.Instance.OnCloseDialog += () =>
          {
              if(state==GameState.Dialog)
                state = previousState;
          };
          menuController.onBack += () =>
          {
              
              state= previousState;
          };
        startScreen.onStartScreenSelected += OnStartScreenSelected;

          menuController.onMenuSelected += OnMenuSelected;

    }

    public void StartGame()
    {
        state = GameState.StartScreen;
        
    }
    public void StartBattle()
    {
        isSetBattle = false;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        var myParty = playerController.GetComponent<MyParty>();
        var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildMonster();
        battleSystem.StartBattle(myParty,wildMonster,playerController);
    }
    MonsterController monster;
     public void StartSetBattle(MonsterController monster)
    {
        isSetBattle = true;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        this.monster = monster;
        var myParty = playerController.GetComponent<MyParty>();
        var wildMonster = monster.GetComponent<MyParty>();
        battleSystem.StartBattle(myParty, wildMonster.Myself[0],playerController);
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
        gameTime += Time.deltaTime;
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
           
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state ==GameState.Inventory)
        {
            Action onBack = () =>
             {
                 if (!DialogManager.Instance.IsShowing)
                 {
                     inventoryUI.gameObject.SetActive(false);
                     state = GameState.FreeRoam;
                 }
                 
             };
            inventoryUI.HandleUpdate(onBack);
        }
        else if (state == GameState.StartScreen)
        {
            
            startScreen.HandleUpdate();
        }
    }
    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnStartScreenSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            List<string> lines = new List<string>();
            lines.Add("Bienvenido a The legend of the coding hero, un divertido juego en el que embarcarás una aventura como el guerrero de Thanya. Para moverte utiliza las teclas W A S D, o las flechas del teclado. Para abrir el menu presiona enter y para cerrarlo x, para seleccionar e interactuar con items y otros seres del mundo de Thanya presiona z.                                                                          Presiona Z para continuar");
            lines.Add("El objetivo del juego es explorar el mundo y sus alrededores minetras combates monstruos usando tus conocimientos sobre programación. Libranos del mal de los monstruos, aventurero! Para empezar tu avetura deberías hablar con el mago Helmuth Dijkstraz.                                                         Presiona Z para cerrar el mensaje");
            Dialog dialog = new Dialog()
            {
                Lines = lines
            };
            StartCoroutine(DialogManager.Instance.ShowBigDialogText(dialog));
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 1)
        {
            //LOAD
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
    }
    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //Inventory
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Inventory;
        }
        else if (selectedItem == 1)
        {
            //SAVE
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 2)
        {
            //LOAD
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if(selectedItem==3)
        {
            //SeeTime
            
            StartCoroutine(DialogManager.Instance.ShowDialogText($"{(int)gameTime/3600} horas y {(int)(gameTime/60)%60} minutos"));
            state = GameState.Dialog;
        }
        
    
    
    }

    public object CaptureState()
    {
        return gameTime;
    }

    public void RestoreState(object state)
    {
        gameTime += (float)state;
    }
}
