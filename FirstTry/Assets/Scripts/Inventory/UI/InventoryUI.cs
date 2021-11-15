using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    [SerializeField] PlayerController playerController;
    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    int selectedItem=0;
    Action onItemUsed;
    RectTransform itemListRect;
    const int itemsInViewPort = 8;
    
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
       
        //Clear all items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);
        slotUIList = new List<ItemSlotUI>();
        foreach(var itemSlot in inventory.Slots)
        {
            var slotUObj = Instantiate(itemSlotUI, itemList.transform);
            slotUObj.SetData(itemSlot);
            slotUIList.Add(slotUObj);
        }

        UpdateItemSelection();
    }
    
    public void HandleUpdate(Action onBack,Action onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();
        if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Usar item
            StartCoroutine(UseItem());
        }

    }
    IEnumerator UseItem()
    {
        var me = playerController.GetComponent<MyParty>().Myself[0];

        var usedItem = inventory.UseItem(selectedItem, me);
        if(usedItem!=null)
        {
            yield return DialogManager.Instance.ShowDialogText($"Usaste {usedItem.Name}");
            onItemUsed?.Invoke();
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"El item no se uso, no tendria efecto");
        }
    }
    void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)

                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;

            else
                slotUIList[i].NameText.color = Color.black;


        }

        
        

        Debug.Log($"{selectedItem}");
        if (inventory.Slots.Count == 0)
        {
            itemIcon.GetComponent<Image>().enabled = false;
            itemDescription.text = "El inventario esta vacio";
        }
        else
        {
            itemIcon.GetComponent<Image>().enabled = true;
            var slot = inventory.Slots[selectedItem].Item;
            itemIcon.sprite = slot.Icon;
            itemDescription.text = slot.Description;
            HandleScrolling();
        }
        

        
    }
    void HandleScrolling()
    {
        var scrollPos = Mathf.Clamp(selectedItem-itemsInViewPort/2,0,selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x,scrollPos);
        bool showUpArrow = selectedItem > itemsInViewPort / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem+ itemsInViewPort / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
