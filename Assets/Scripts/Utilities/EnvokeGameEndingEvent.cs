using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvokeGameEndingEvent : MonoBehaviour
{
    [SerializeField] private float timeToPause = 3f;
    
    public void OnGameEnd()
    {   
        
        GameManager.Instance.LoadEnding(timeToPause);
    }
}
