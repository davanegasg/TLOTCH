using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Teleports player to different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable 
{
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;
    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }
    public bool TriggerRepeatedly => false;
    Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    IEnumerator Teleport()
    {


        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);
        
        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

    }
    public enum DestinationIdentifier { A, B, C, D, E }
    public Transform SpawnPoint => spawnPoint;
}
