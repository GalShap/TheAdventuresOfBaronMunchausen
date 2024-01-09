using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    
    #region Inspector

    [Tooltip("Drag the player CharacterController component here!")]
    [SerializeField] private Rigidbody playerRigidbody;

    [Tooltip("Adjust movement speed")]
    [SerializeField] private float movementSpeed = 15f;
    
    #endregion

    #region Fields

    private bool _isDancing=true;

    #endregion
    
    #region  Properties
   
    public bool IsDancing
    {
        get => _isDancing;
    }

    #endregion
    
    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    
    #endregion
    
    #region Private Methods

    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal * movementSpeed, playerRigidbody.velocity.y, vertical * 
            movementSpeed);
        playerRigidbody.velocity = direction;
    }
    
    #endregion
    
}
