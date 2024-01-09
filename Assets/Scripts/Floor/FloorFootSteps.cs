using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class FloorFootSteps : MonoBehaviour
{
   #region Inspector

   [SerializeField] private GameObject[] footprints;
   [SerializeField] private PlayerInteractions playerInteractions;
   [SerializeField] private ThirdPersonController playerController;

   #endregion

   #region Fields

   private int _curFootPrintsIndex;
   private int _footPrintsCount;

   #endregion

   #region MonoBehaviuor

   private void Start()
   {
      _footPrintsCount = footprints.Length;
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.gameObject.CompareTag("PlayerLeftFoot") || other.gameObject.CompareTag("PlayerRightFoot"))
      {
         if (_curFootPrintsIndex >= _footPrintsCount)
         {
            _curFootPrintsIndex = 0;
         }
         if (!footprints[_curFootPrintsIndex].activeSelf)
         {
            footprints[_curFootPrintsIndex].SetActive(true);
         }

         footprints[_curFootPrintsIndex].transform.position = other.transform.position;
         footprints[_curFootPrintsIndex].transform.rotation = playerInteractions.transform.rotation;
         //playerController.PlayAudioOnFootstep();
         
         _curFootPrintsIndex++;
      }
   }

   #endregion
}
