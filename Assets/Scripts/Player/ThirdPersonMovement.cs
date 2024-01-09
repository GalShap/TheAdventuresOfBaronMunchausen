using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    #region Inspector

    [Tooltip("Drag the player CharacterController component here!")]
    [SerializeField] private CharacterController controller;
    
    [Tooltip("Adjust movement speed")]
    [SerializeField] private float movementSpeed = 6f;
    
    [Tooltip("Adjust how smooth player rotation is")]
    [SerializeField] private float turnSmoothTime = 0.1f;
    
    #endregion
    
    private float turnSmoothVelocity;
    
    
    #region Constants

    private const float IsMoving = 0.1f;

    #endregion
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    
    
    void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= IsMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                 turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle,0);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * (movementSpeed * Time.deltaTime));
        }
    }

}
