using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
     
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set;}

    public Quest(QuestBase _base)
    {
        Base = _base;
    }


    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;
        yield return DialogManager.Instance.ShowDialog(Base.StartDialog);
        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public IEnumerator CompleteQuest(Transform player)
    {
        Status = QuestStatus.Completed;
        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialog);
        var inventory = Inventory.GetInventory();
        if(Base.RequiredItem!=null)
        {
            inventory.RemoveItem(Base.RequiredItem);
        }
        if(Base.RewardItem!=null)
        {
            inventory.AddItem(Base.RewardItem);
            var playerName = player.GetComponent<PlayerController>().Name;
            yield return DialogManager.Instance.ShowDialogText($"{playerName} recibio {Base.RewardItem.Name}");
        }
        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            if(!inventory.HasItem(Base.RequiredItem))
            {
                return false;
            }
        }
        return true;
    }
}
public enum QuestStatus { None, Started, Completed}