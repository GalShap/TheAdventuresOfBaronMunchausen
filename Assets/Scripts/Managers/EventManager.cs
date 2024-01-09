using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Core;
using StarterAssets;
using UnityEngine;

public class EventManager : SingletonPersistent<EventManager>
{

    #region Inspector
   
    [SerializeField] public ThirdPersonController playerController;

    [Header("Ducks Management")]
    [Tooltip("Drag all ducks in game to here")]
    [SerializeField] private List<Duck> ducksInGame;
    [SerializeField] private float duckMoveSpeed = 5f;
    [SerializeField] private float pauseTime = 0.5f;
    [SerializeField] private LakeBeahviour lakeBehaviour;

    [SerializeField] private Transform defaultPlayerPos;

    #endregion
    
    #region Public Fields
    public int NumOfDucks {get; set;}
    public bool IsPlayerSprinting
    { 
        get => playerController.IsSprinting;
    }
    
    public bool CanEnterLake { get; set; } = false;
    
    #endregion
    
    #region constants
        
    private const int AllDucksShot = 0;

    #endregion
    
    
    
    #region MonoBehaviour Funcs
    
    // Start is called before the first frame update
    void Start()
    {
        NumOfDucks = ducksInGame.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    #endregion
    
    #region Ducks Moving

    public void EnableDuckSounds()
    {
        foreach (Duck duck in ducksInGame)
        {
            duck.gameObject.GetComponent<AudioSource>().Play();
        }
    }
    
    public void StartMovingAllDucks()
    {
        StartCoroutine(MoveAllDucks());
    }
    
    /// <summary>
    /// a coroutine that iterates through all the ducks in game and moves them to their new position.
    /// </summary>
    private IEnumerator MoveAllDucks()
    {
        Flock.CanMoveDucks = false;
        foreach (var variablDuck in ducksInGame)
        {
            StartCoroutine(variablDuck.MoveDuckCoroutine(duckMoveSpeed));
            yield return new WaitForSeconds(pauseTime);
        }
        
        Debug.Log("All ducks moved!");
        
        // only once all ducks moved -> replace the colliders in lake. 
        
        lakeBehaviour.DisablePreDucksColliders();
        lakeBehaviour.EnablePostDuckColliders(GetListOfPositions());
        DisableAllDuckColliders();
        
        CanEnterLake = true;
        
        Debug.Log("changed all colliders! can pass lake!");
    }

    private List<Vector3> GetListOfPositions()
    {
        List<Vector3> ducksLastPositions = new List<Vector3>();
        foreach (Duck duck in ducksInGame){
            print("duck pos is: " + duck.GetDuckPosInLake());
            ducksLastPositions.Add(duck.GetDuckPosInLake());
        }
        
        return ducksLastPositions;
    }

    /// <summary>
    /// returns true if all ducks in game got shot. false otherwise. 
    /// </summary>
    /// <returns></returns>
    public bool ShouldMoveDucks()
    {
        return NumOfDucks == AllDucksShot;
    }

    public void DisableAllDuckColliders()
    {
        for (int i = 0; i < ducksInGame.Count; i++)
        {
            ducksInGame[i].SetDuckColliderActive(false);
        }
    }
    #endregion

    #region DeerWakeUp

    public void WakeDeerUp()
    { 
       GameManager.Instance.LoadPostDeerWakeScene();
    }
    
    #endregion
    
    #region debugging

    public void SetAllDucksOnBaron()
    {
        foreach (var duck in ducksInGame)
        {
            duck.gameObject.transform.position = playerController.gameObject.transform.position;
        }
    }

    public void MovePlayerToDefaultPos()
    {
        playerController.transform.position = defaultPlayerPos.position;
        playerController.transform.rotation = defaultPlayerPos.rotation;
    }
    
    
    #endregion
   
    
}
