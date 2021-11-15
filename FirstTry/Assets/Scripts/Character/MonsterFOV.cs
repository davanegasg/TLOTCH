using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
    }
    public bool TriggerRepeatedly => false;
}
