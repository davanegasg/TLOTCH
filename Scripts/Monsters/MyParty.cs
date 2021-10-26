using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParty : MonoBehaviour
{
    [SerializeField] Monster Myself;

    private void Start()
    {
        Myself.Init();
    }

    public Monster getMyself()
    {
        return Myself;
    }
}
