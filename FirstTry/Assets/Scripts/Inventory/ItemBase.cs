using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string pluralName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public string Name => name;
    public string PluralName => pluralName;
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Monster monster)
    {
        return false;
    }
}