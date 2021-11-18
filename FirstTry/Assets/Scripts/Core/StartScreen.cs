using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class StartScreen : MonoBehaviour
{
    [SerializeField] GameObject instructions;
    [SerializeField] GameObject start;
    [SerializeField] GameObject optionSelector;
    List<Text> optionSelectorItems;
    int selectedItem = 0;
    bool zPressed = false;
    public event Action<int> onStartScreenSelected;

    private void Awake()
    {
        optionSelectorItems = optionSelector.GetComponentsInChildren<Text>().ToList();
    }
   
    public void HandleUpdate()
    {
        if (!zPressed)
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                instructions.SetActive(false);
                start.SetActive(false);
                optionSelector.SetActive(true);
                UpdateItemSelection();
                zPressed = true;
            }  
        }
        else
        {
            int prevSelection = selectedItem;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;

            
            selectedItem = Mathf.Clamp(selectedItem, 0, optionSelectorItems.Count-1);

            if (prevSelection != selectedItem)
                UpdateItemSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                CloseStartScreen();
                onStartScreenSelected?.Invoke(selectedItem);
                
            }
        }
        
        
    }

    void CloseStartScreen()
    {
        this.gameObject.SetActive(false);
    }
    void UpdateItemSelection()
    {
        for (int i = 0; i < optionSelectorItems.Count; i++)
        {
            if (i == selectedItem)

                optionSelectorItems[i].color = GlobalSettings.i.HighlightedColor;

            else
                optionSelectorItems[i].color = Color.white;


        }
    }
}
