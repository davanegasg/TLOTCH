using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPatterns;
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;
    Quest activeQuest;
    ItemGiver itemGiver;
    int currentPatten = 0;
    Character character;
    NPCState state;
    float idleTimer = 0f;
    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
    }

    
    public IEnumerator Interact(Transform initiator)
    {
        if(state== NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);
            if(questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest(initiator);
                questToComplete = null;
            }
            if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;
            }
            else if (activeQuest!= null)
            {
                if(activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null; 
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialog);
                }
            }
            else if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
                
            }
            idleTimer = 0;
            state = NPCState.Idle;

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

    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();
        if(questToStart!=null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();
        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if(saveData!=null)
        {

            activeQuest = (saveData.activeQuest != null)? new Quest(saveData.activeQuest): null;
            questToStart = (saveData.questToStart != null) ? new Quest(saveData.questToStart).Base : null;
            questToComplete = (saveData.questToComplete != null) ? new Quest(saveData.questToComplete).Base : null;
        }
    }

    
}
[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}
public enum NPCState { Idle, Walking, Dialog }
