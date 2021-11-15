using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;


public class PlayerController : MonoBehaviour, ISavable
{

    [SerializeField] string name;
    private Vector2 input;
    private Character character;
    public event Action<Collider2D> OnEnterTrainersView;

    public string Name { get { return name; } }
    
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
            StartCoroutine(Interact());
        }
    }

    IEnumerator Interact()
    {
        var faceDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + faceDir;
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }

    }
    IPlayerTriggerable currentlyInTrigger;
    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        IPlayerTriggerable triggerable=null;

        foreach (var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable!= null)
            {
                if(triggerable== currentlyInTrigger&& !triggerable.TriggerRepeatedly)
                    break;
                
                currentlyInTrigger = triggerable;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
        if (colliders.Count() == 0||triggerable!=currentlyInTrigger)
            currentlyInTrigger = null;
        
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

        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            monster = GetComponent<MyParty>().Myself.Select(p => p.GetSaveData()).ToList()
        };
        
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        //Restore position
        var pos = saveData.position;
        transform.position= new Vector3(pos[0],pos[1]);
        //Restore Party
        GetComponent<MyParty>().Myself  = saveData.monster.Select(s=> new Monster(s)).ToList();
    }

    public Character Character => character;

    
}
[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<MonsterSaveData> monster;

}
