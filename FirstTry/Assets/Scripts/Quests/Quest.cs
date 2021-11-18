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
    public Quest(QuestSaveData saveData)
    {
        Base = QuestDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }
    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            name = Base.Name,
            status = Status
        };
        return saveData;
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
[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QuestStatus status;
}
public enum QuestStatus { None, Started, Completed}