using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPatterns;
    int currentPatten = 0;
    Character character;
    NPCState state;
    float idleTimer = 0f;
    private void Awake()
    {
        character = GetComponent<Character>();
    }
    public void Interact()
    {
        if(state== NPCState.Idle)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        }    
        
    }

    private void Update()
    {
        if (DialogManager.Instance.IsShowing) return;
        if(state==NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer>timeBetweenPatterns)
            {
                idleTimer = 0f;
                if(movementPattern.Count>0)
                {
                    StartCoroutine(Walk());
                }
                

            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;
        yield return character.Move(movementPattern[currentPatten]);
        currentPatten = (currentPatten + 1)%movementPattern.Count;
        state = NPCState.Idle;
    }

    public enum NPCState { Idle, Walking}
}