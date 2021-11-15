using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestList : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();
    public event Action OnUpdated;
    public static QuestList GetQuestList()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestList>();
    }
    public bool IsStarted(string questName)
    {
        var questStatus =quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStatus.Started||questStatus==QuestStatus.Completed;
    }

    public bool IsCompleted(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return  questStatus == QuestStatus.Completed;
    }
    public void AddQuest(Quest questToAdd)
    {
        if(!quests.Contains(questToAdd))
            quests.Add(questToAdd);
        OnUpdated?.Invoke();
    }
}
