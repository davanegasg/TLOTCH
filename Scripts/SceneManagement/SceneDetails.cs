using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{

    public bool IsLoaded { get; private set; }
    [SerializeField] List<SceneDetails> connectedScenes;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);
                
            
            foreach (var scene in connectedScenes)
            {
                scene.LoadScene(); 
            }
            
            if(GameController.Instance.PrevScene !=null)
            {
                var previouslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;
                foreach( var scene in previouslyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene)&&scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
            }
        }
    }

    public void UnloadScene()
    {
        if(IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
        
    }
    public void LoadScene()
    {
        if(!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
        
    }    
}
