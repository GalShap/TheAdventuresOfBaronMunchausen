using System;
using StarterAssets;
using UnityEngine;

public class LakePlatform : MonoBehaviour
{

    [SerializeField] private ThirdPersonController _thirdPersonController;
    
    [Tooltip("if player should jump on this platform set this to true. if player should drown when hitting " +
             "this platform set it to false")]
    [SerializeField] private bool isPlatform = true;

    [SerializeField] private bool bridgeLake = false;
    
    [Tooltip("Where will the player reset when falling into the lake?")]
    [SerializeField] private Transform playerPositionReset;
    
    [Tooltip("The duck in which this platform is attached to")]
    [SerializeField] private Duck duck;

    private const string PlatformMsg = "Landed on Platform!";

    private const string SinkMsg = "Landed inside lake!";

    private void OnTriggerEnter(Collider other)
    {
       
        // player has collided with platform so change it's radius
        if (other.gameObject.CompareTag("Player"))
        {
            
            
            var msg = isPlatform ? PlatformMsg : SinkMsg;
            _thirdPersonController.SetGroundedRadius(isPlatform);
            
            // landed on platform so the appropriate to that platform should be squished. 
            if (isPlatform)
            {   
                Debug.Log("Player landed on duck!");
                duck.ToggleSquish(true);
            }

            // this means player has fallen into the lake! 
            if (!isPlatform)
            {   
                var playerInteractions = other.gameObject.GetComponent<PlayerInteractions>();
               
                // player should drown.
                if (EventManager.Instance.CanEnterLake || bridgeLake)
                {   
                    Debug.Log("Player is drowning");
                    AudioManager.Instance.PlayDropIntoLakeSound();
                    playerInteractions.PlayerDrownInLake(playerPositionReset.position);
                }
                
                
                // Player should slip.
                else
                {
                    Debug.Log("Player can't enter lake yet");
                    playerInteractions.PlayerSlipOnLake(playerPositionReset.position);
                }
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isPlatform)
            {
                Debug.Log("Player jumped from duck!");
                duck.ToggleSquish(false);
            }
        }
    }
}
