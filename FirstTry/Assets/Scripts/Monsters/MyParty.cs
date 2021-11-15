using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParty : MonoBehaviour
{
    [SerializeField] List<Monster> myself;

    public List<Monster> Myself
    {
        get
        {
            return myself;
        }
        set
        {
            myself = value;
        }
    }
    private void Start()
    {
        foreach(var monster in myself)
            monster.Init();
    }

}
