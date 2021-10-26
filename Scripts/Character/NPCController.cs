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

    
    public void Interact(Transform initiator)
    {
        if(state== NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog,()=> { idleTimer = 0f;  state = NPCState.Idle; }));
        }    
        
    }

    private void Update()
    {
       
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
        var oldPos = transform.position;
        yield return character.Move(movementPattern[currentPatten]);
        if(oldPos!= transform.position)    
            currentPatten = (currentPatten + 1)%movementPattern.Count;
        state = NPCState.Idle;
    }

    public enum NPCState { Idle, Walking,Dialog}
}
