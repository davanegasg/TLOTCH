using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerController : MonoBehaviour, ISavable
{
    
   
    private Vector2 input;
    private Character character;
    public event Action<Collider2D> OnEnterTrainersView;

    
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
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        var faceDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + faceDir;
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }

    }
    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
 

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable!= null)
            {
                character.Animator.IsMoving=false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
        CheckIfInTrainersView();
    }

    private void CheckIfInTrainersView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if (collider!=null)
        {
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public object CaptureState()
    {
        float[] position= new float[] {transform.position.x, transform.position.y };
        return position;
    }

    public void RestoreState(object state)
    {
        var position = (float[])state;
        transform.position= new Vector3(position[0],position[1]);
    }

    public Character Character => character;

    
}
