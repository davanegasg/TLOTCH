using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    
   
    private Vector2 input;
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {

        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            if (input.x != 0) input.y = 0;
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input,OnMoveOver));
            }
        }
        character.HandleUpdate();
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable!= null)
            {
                
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }

    }
    public Character Character => character;

}
