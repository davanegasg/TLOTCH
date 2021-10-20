using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject Fov;

    //State

    bool battleLost = false;
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public void Interact(Transform initiator)
    {
        if (!battleLost)
        {
            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                GameController.Instance.StartSetBattle(this);
            }));
        }

    }
    public IEnumerator TriggerMonsterBattle(PlayerController player)
    {
        //Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);
        //Walk Towards the player
        var diff = player.transform.position - transform.position;
        var moveVec=diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        yield return character.Move(moveVec);

        //Show dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
         {
             GameController.Instance.StartSetBattle(this);
         }));
    }
    public void BattleLoss()
    {
        battleLost = true;
        this.gameObject.SetActive(false);
    }
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if(dir==FacingDirection.right)
        {
            angle = 90f;
        }
        else if (dir == FacingDirection.up)
        {
            angle = 180f;
        }
        else if (dir == FacingDirection.left)
        {
            angle = 270f;
        }
        Fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
