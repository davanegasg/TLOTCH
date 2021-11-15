using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneDetails : MonoBehaviour
{

    public bool IsLoaded { get; private set; }
    [SerializeField] List<SceneDetails> connectedScenes;
    List<SavableEntity> savableEntities;
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
            var prevScene = GameController.Instance.PrevScene;
            if (GameController.Instance.PrevScene !=null)
            {
                var previouslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;
                foreach( var scene in previouslyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene)&&scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
                if(!connectedScenes.Contains(prevScene))
                    prevScene.UnloadScene();
            }
        }
    }

    public void UnloadScene()
    {
        if(IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
            
        }
        
    }
    public void LoadScene()
    {
        if(!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
              {
                  savableEntities = GetSavableEntitiesInScene();
                  SavingSystem.i.RestoreEntityStates(savableEntities);
              };
            
        }
        
    }    

    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
        return savableEntities;
    }
}
