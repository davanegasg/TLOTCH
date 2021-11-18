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
    [SerializeField] GameObject bigDialogBox;
    [SerializeField] Text bigDialogText;
    Dialog dialog;
    

    public static DialogManager Instance { get; private set; }
    public bool IsShowing { get; private set; }

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    Action onDialogFinished;

    public IEnumerator ShowDialogText(string Text, bool waitForInput = true, bool autoClose = true)
    {
        IsShowing = true;
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        yield return TypeDialog(Text);
        if(waitForInput)
        {
           
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        }
        if(autoClose)
        {
            CloseDialog();
        }
    }
    public IEnumerator ShowBigDialogText(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        OnShowDialog?.Invoke();
        bigDialogBox.SetActive(true);
        foreach (var line in dialog.Lines)
        {
            yield return TypeBigDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
        CloseBigDialog();

    }
    public void CloseBigDialog()
    {
        bigDialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();

    }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();

    }
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        foreach( var line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
        CloseDialog();
        

    }   
    
    private void Awake()
    {
        Instance = this;
    }

    
    
    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";

        foreach (var letter in line.ToCharArray())
        { 
                dialogText.text += letter;
                yield return new WaitForSeconds(0.5f / lettersPerSecond);
        }
        
    }
    public IEnumerator TypeBigDialog(string line)
    {
        bigDialogText.text = "";

        foreach (var letter in line.ToCharArray())
        {
            bigDialogText.text += letter;
            yield return new WaitForSeconds(0.5f / lettersPerSecond);
        }

    }
}

