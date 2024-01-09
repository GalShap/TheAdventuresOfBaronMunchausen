using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HorseBottomHalfBehaviour : MonoBehaviour
{

    #region Inspector
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [FormerlySerializedAs("target")] [SerializeField] Transform playerTarget; // the target object to follow
    [SerializeField] float distanceToTarget; // the fixed distance to maintain from the target
    [SerializeField] float speedToTarget;
    [SerializeField] private float timeToWalk = 1f;
    [SerializeField] float rotationToTargetSpeed = 5f;
    [SerializeField] GameObject carrotDream;

    [SerializeField] private float timeToPlayHorseBark = 8f;

    [Header("Combination")] [SerializeField]
    private Transform frontHorseTarget;
    [SerializeField] float distanceToFrontHorse; // the fixed distance to maintain from the target
    [SerializeField] float speedToFrontHorse;
    [SerializeField] private FullHorseBehaviour _fullHorseBehaviour;
    #endregion



    #region Fields

    private bool _shouldStand;
    private bool _isStanding;
    private Rigidbody _rb;
    private float _timeToWalk;
    private bool _hasReachedAnimEnd;
    private float _elapsedWalkTime;
    private Transform _parentTransform;
    private float _speed;
    private Transform _target;
    private float _distance;

    private float _curTime = 0f;
    #endregion


    #region Constants
    private const int MaxBlendShapeValue = 100;
    private const int MinBlendShapeValue = 0;
    private const int WalkBlendLayer = 0;
    #endregion

    #region properties


    public bool ShouldStand
    {
        set => _shouldStand = value;
    }
    public Transform Target
    {
        set => _target = value;
    }

    public float SpeedToTarget
    {
        set => _speed = value;
    }
    
    public float DistanceToTarget
    {
        set => _distance = value;
    }


    #endregion


    #region MonoBehaviour
    private void Start()
    {
        _parentTransform = transform.parent.transform;
        // _rb = _parentTransform.GetComponent<Rigidbody>();
        _target = playerTarget;
        _speed = speedToTarget;
        _distance = distanceToTarget;

    }

    void Update()
    {

        _curTime += Time.deltaTime;
        if (_curTime >= timeToPlayHorseBark)
        {
            _curTime = 0f;
            if (Random.Range(0, 2) < 1f)
                AudioManager.Instance.PlayHorseSound1();
            
            else 
                AudioManager.Instance.PlayHorseSound2();
        }
        
        if (_shouldStand)
        {
            if (!_isStanding)
            {
                // _rb.isKinematic = true;
                _isStanding = true;
            }
            
            return;
        }

        if (_isStanding)
        {
            // _rb.isKinematic = false;
            _isStanding = false;
        }
        

        if (_target != null)
        {
            Vector3 targetPosition = _target.position + (_target.forward * -_distance);
            _parentTransform.position = Vector3.Lerp(_parentTransform.position, targetPosition, Time.deltaTime * _speed);
            Vector3 direction = (playerTarget.position - _parentTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _parentTransform.rotation = Quaternion.Slerp(_parentTransform.rotation, lookRotation, Time.deltaTime * rotationToTargetSpeed);
        }


    }

    private void FixedUpdate()
    {
        
        if (!_isStanding)
        {

            CalcBlendShapeValue(0);
        }
        else
        {
            
            CalcBlendShapeValue(1); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BottomHorseStopPoint"))
        {
            _shouldStand = true;
            carrotDream.SetActive(true);
        }

        else if (other.gameObject.CompareTag("Carrot"))
        {
            other.transform.parent.gameObject.SetActive(false);
            StartFollowingPlayer();
            carrotDream.SetActive(false);
            AudioManager.Instance.PlayHorseBitCarrot();
            StartCoroutine(PlayPlayerCallingHorse());

        }

        else if (other.gameObject.CompareTag("CombinationPoint"))
        {
            frontHorseTarget.parent.gameObject.SetActive(false);
            _fullHorseBehaviour.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    #endregion

    

    #region Methods

    IEnumerator PlayPlayerCallingHorse()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        AudioManager.Instance.PlayPlayerCallHorse();
    }
    private void CalcBlendShapeValue(int layer)
    {
        

        // _timeToWalk = _isPlayerSprinting ? timeToWalk : 2 * timeToWalk;
        _timeToWalk = timeToWalk;
        float curA;
        float curB;
        float curBlendValue;
        if (!_hasReachedAnimEnd)
        {   
            // force is increasing
            curA = MinBlendShapeValue;
            curB = MaxBlendShapeValue;
        }
        else
        {   
            // force is decreasing
            curA = MaxBlendShapeValue;
            curB = MinBlendShapeValue;
        }

        if (_elapsedWalkTime < _timeToWalk)
        {
            _elapsedWalkTime += Time.deltaTime;
            curBlendValue = Mathf.Lerp(curA, curB, _elapsedWalkTime / _timeToWalk);
            
        }

        else
        {
            curBlendValue = curB;
            _elapsedWalkTime = 0f;
            _hasReachedAnimEnd = !_hasReachedAnimEnd;
        }
		
        meshRenderer.SetBlendShapeWeight(layer, curBlendValue);
    }

    private void StartFollowingPlayer()
    {
        _target = playerTarget;
        _speed = speedToTarget;
        _distance = distanceToTarget;
    }

    public void CombineWithFrontHorse()
    {
        _target = frontHorseTarget;
        _distance = distanceToFrontHorse;
        _speed = speedToFrontHorse;
    }
    

    #endregion
}
