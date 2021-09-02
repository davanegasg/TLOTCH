using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle,Paused}

public class GameController : MonoBehaviour
{
    
    [SerializeField] PlayerController playerController;
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

    void StartBattle()
    {
        state = GameState.Battle;


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

        }
    }
    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }
}
