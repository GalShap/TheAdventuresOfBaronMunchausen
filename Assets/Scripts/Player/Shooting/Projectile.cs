using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private float timeToDestroy = 3f;

    private float _elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (_elapsedTime < timeToDestroy)
            _elapsedTime += Time.deltaTime;

        else
        {
            Debug.Log("Projectile shot is destroyed");
            Destroy(this.gameObject);
        }
        
        
    }
}
