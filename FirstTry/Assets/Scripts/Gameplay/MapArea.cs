using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<MonsterBase> wildMonsters;

    public MonsterBase GetRandomWildMonster()
    {
        return wildMonsters[Random.Range(0,wildMonsters.Count)];
    }
}
