using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersDB
{
    static Dictionary<string, MonsterBase> monsters;

    public static void Init()
    {
        monsters = new Dictionary<string, MonsterBase>();
        var monsterArray = Resources.LoadAll<MonsterBase>("");
        foreach(var monster in monsterArray)
        {
            if(monsters.ContainsKey(monster.Name))
            {
                Debug.LogError($"Two monsters with the same name");
                continue;
            }
            monsters[monster.Name] = monster;
        }
    }

    public static MonsterBase GetMonsterByName(string name)
    {
        if(!monsters.ContainsKey(name))
        {
            Debug.LogError($"name doesn't exist {name}");
        }
        return monsters[name];
    }
}
