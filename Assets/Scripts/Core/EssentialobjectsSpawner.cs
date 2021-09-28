using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialobjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsprefab;

    private void Awake()
    {
        var existingObjects= FindObjectsOfType<EssentialObjects>();
        if(existingObjects.Length == 0)
        {

            //If there's a grid spawn at it's center\

            var spawnPos = new Vector3(0, 0, 0);
            var grid = FindObjectOfType<Grid>();
            if(grid!=null)
            {
                spawnPos = grid.transform.position;
            }
            Instantiate(essentialObjectsprefab,spawnPos,Quaternion.identity);
        }
    }


}
