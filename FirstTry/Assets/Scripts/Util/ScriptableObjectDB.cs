using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{

    static Dictionary<string, T> objects;

    public static void Init()
    {
        objects = new Dictionary<string, T>();
        var objectArray = Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError($"Two {obj.name} with the same name");
                continue;
            }
            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError($"name doesn't exist {name}");
        }
        return objects[name];
    }
}

