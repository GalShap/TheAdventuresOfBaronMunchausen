using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyBehaviour : MonoBehaviour
{
   #region Inspector

   [SerializeField] private Transform bunnyObject;
   [SerializeField] private BunnyWaitingPoint[] bunnyWaitingPoints;
   [SerializeField] private Animator animator;
   [Range(0, 10)] [SerializeField] private float timeToMoveToNextPoint;

 

   #endregion

   #region Fields

   private int _curWaitingPointIndex;
   private bool _canMoveToNextPoint;
   private bool _isCurrentlyMoving; 
   private AudioSource _bunnyAudioSource;
   private bool _isCurrentlyPlayingSounds;

   #endregion

   #region Properties

   public bool CanMoveToNextPoint
   {
      set => _canMoveToNextPoint = value;
   }

   #region AnimatorParams

   private static readonly int idle = Animator.StringToHash("Idle");
   private static readonly int run = Animator.StringToHash("Run");

   #endregion

   #endregion

   #region MonoBehaviour

   private void Start()
   {
      _bunnyAudioSource = gameObject.GetComponent<AudioSource>();
      _bunnyAudioSource.Play();
   }

   private void FixedUpdate()
   {
      if (_canMoveToNextPoint && !_isCurrentlyMoving && _curWaitingPointIndex < bunnyWaitingPoints.Length - 1)
      {  
         
         _isCurrentlyMoving = true;
         animator.SetTrigger(run);
         StartCoroutine(MoveObjectOverTime(bunnyWaitingPoints[_curWaitingPointIndex].transform.position, bunnyWaitingPoints[_curWaitingPointIndex + 1].transform.position));

         //if (_curWaitingPointIndex >= bunnyWaitingPoints.Length)
            //_bunnyAudioSource.enabled = false;
      }
      
   }

   #endregion

   #region Methods

  

   
   IEnumerator MoveObjectOverTime( Vector3 pointA, Vector3 pointB)
   {
      _bunnyAudioSource.Stop();
      float i = 0.0f;
      float rate = 1.0f / timeToMoveToNextPoint;
      while (i < 1.0f)
      {
         i += Time.deltaTime * rate;
         var lerp = Vector3.Lerp(pointA, pointB, i);
         bunnyObject.position = new Vector3(lerp.x, bunnyObject.position.y, lerp.z);
         bunnyObject.LookAt(pointB);
         yield return null;
      }
      bunnyObject.position = new Vector3(pointB.x, bunnyObject.position.y, pointB.z);

      _curWaitingPointIndex++;
      _isCurrentlyMoving = false;
      _canMoveToNextPoint = false;
      animator.SetTrigger(idle);
      if (_curWaitingPointIndex < bunnyWaitingPoints.Length - 1)
      {
         _bunnyAudioSource.Play();
      }
   }

   #endregion
}
