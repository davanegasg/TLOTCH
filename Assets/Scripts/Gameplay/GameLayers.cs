using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask longGrassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    
    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask LongGrassLayer
    {
        get => longGrassLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }    

    public LayerMask FovLayer
    {
        get => fovLayer;
    }
    public LayerMask PortalLayer
    {
        get => portalLayer;
    }

    public LayerMask TriggerableLayers
    {
        get => longGrassLayer|fovLayer|portalLayer;
    }
    
}
