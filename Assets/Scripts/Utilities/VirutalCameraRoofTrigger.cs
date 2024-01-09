using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirutalCameraRoofTrigger : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera cameraBeforeTrigger;
    
    [SerializeField] private CinemachineVirtualCamera cameraAfterTrigger;

    [SerializeField] private bool isFromFirstToDolly;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isFromFirstToDolly)
            {   
                Debug.Log("from follow to doly");
                cameraAfterTrigger.gameObject.SetActive(true);
                cameraBeforeTrigger.Priority -= 1;
                cameraBeforeTrigger.LookAt = other.gameObject.transform;
            }

            // from dolly to first
            else
            {
            Debug.Log("from doly to tower");
            
                cameraBeforeTrigger.gameObject.SetActive(false);
                cameraAfterTrigger.Priority += 2;
            }

           
           


        }
    }

}
