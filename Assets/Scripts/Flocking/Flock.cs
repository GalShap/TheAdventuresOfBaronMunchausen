using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    [Header("Player Set Up")] 
    [SerializeField] private PlayerInteractions player;
    [SerializeField] private int playerWeight;

    [Header("Spawn Setup")]
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private  float _minSpeed;
    public static float minSpeed { get { return _instance._minSpeed; } }
    [Range(0, 10)]
    [SerializeField] private  float _maxSpeed;
    public static float maxSpeed { get { return _instance._maxSpeed; } }
    
    [Header("Weights Setup")]
    [Range(0, 10)]
    [SerializeField] private  float _cohesionWeight;
    public static float CohesionWeight { get { return _instance._cohesionWeight; } }
    [Range(0, 10)]
    [SerializeField] private  float _aligementWeight;
    public static float AligementWeight { get { return _instance._aligementWeight; } }

    
    private static Flock _instance;
    private bool _canMoveDucks = true;
    private List<Duck> _allUnits;
    public static List<Duck> AllUnits
    {
        get => _instance._allUnits;
    }
    
    public static Transform PlayerTransform
    {
        get => _instance.player.transform;
    }
    
    public static PlayerInteractions PlayerInteractions
    {
        get => _instance.player;
    }
    
    
    public static int PlayerWeight
    {
        get => _instance.playerWeight;
    }
    
    public static bool CanMoveDucks
    {
        get => _instance._canMoveDucks;
        set => _instance._canMoveDucks = value;
    }


    private void Awake()
    {

        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            _allUnits = new List<Duck>();
        }
    }

    private void Update()
    {
        if (_canMoveDucks)
        {
            for (int i = 0; i < _allUnits.Count; i++)
            {
                _allUnits[i].MoveUnit();
            } 
        }
        
        
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         _canMoveDucks = true;
    //     }
    // }

    // public static void StopDucks()
    // {
    //     for (int i = 0; i < _instance._allUnits.Count; i++)
    //     {
    //         _instance._allUnits[i].DontMove();
    //     }  
    // }

    public static void AddDuck(Duck duck)
    {
        _instance._allUnits.Add(duck);
    }
    
}
