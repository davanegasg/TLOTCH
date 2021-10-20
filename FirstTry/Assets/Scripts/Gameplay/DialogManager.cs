using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    Dialog dialog;
    int currentLine = 0;
    bool isTyping;
    bool skip = false;

    public static DialogManager Instance { get; private set; }
    public bool IsShowing { get; private set; }

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    Action onDialogFinished;

    public IEnumerator ShowDialog(Dialog dialog,Action onFinished=null)
    {
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        this.dialog = dialog;
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        onDialogFinished = onFinished;
        StartCoroutine(TypeDialog(dialog.Lines[0]));

    }   
    public void HandleUpdate()
    {
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isTyping)
            {
                skip = false;   
                ++currentLine;
                if (currentLine < dialog.Lines.Count)
                {

                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else
                {
                    currentLine = 0;
                    IsShowing = false;
                    dialogBox.SetActive(false);
                    onDialogFinished?.Invoke();
                    OnCloseDialog?.Invoke();

                }
            }
            else
            {
                skip = true;
            }
        }
    }
    private void Awake()
    {
        Instance = this;
    }

    
    
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (var letter in line.ToCharArray())
        {
           
            if (skip)
            {
                
                dialogText.text = dialog.Lines[currentLine];
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                isTyping = false;

                yield break;
                

            }
            else
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(1f / lettersPerSecond);
               
            }
            
            
        }
        isTyping = false;
    }
}

