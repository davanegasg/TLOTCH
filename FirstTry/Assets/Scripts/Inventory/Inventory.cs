using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    public event Action OnUpdated;

    
    public List<ItemSlot> Slots => slots;
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public bool HasItem(ItemBase item)
    {
        return slots.Exists(slot=> slot.Item==item);
    }
    public void AddItem(ItemBase item, int count=1)
    {
        var itemSlot = slots.FirstOrDefault(slots => slots.Item == item);
        if(itemSlot!=null)
        {
            itemSlot.Count+=count;
        }
        else
        {
            slots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }
        OnUpdated?.Invoke();

    }
    public ItemBase UseItem(int itemIndex, Monster monster)
    {
        var item =slots[itemIndex].Item;
        bool itemUsed = item.Use(monster);
        if(itemUsed)
        {
            RemoveItem(item);
            return item;
        }
        else
        {
            
            return null;
        }
        
    }
    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slots => slots.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
        {
            slots.Remove(itemSlot);
        }
        OnUpdated?.Invoke();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(i => i.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;
        slots = saveData.items.Select(i=>new ItemSlot(i)).ToList();

        OnUpdated?.Invoke();
    }
}

[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField]int count;

    public ItemSlot()
    {

    }
    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name) ;
        count = saveData.count;
    }
    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count
        };
        return saveData;
    }

    public ItemBase Item { get => item; set => item = value; }
    public int Count { get => count; set => count = value; } 
}

[System.Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}
[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
}