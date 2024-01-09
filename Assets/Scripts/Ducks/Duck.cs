using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random=UnityEngine.Random;


/**
 * This class represents the ducks in game. 
 */
[Serializable]
public class Duck : MonoBehaviour, IHornResponsive
{

	#region Inspector

	[SerializeField] private bool gotHit;

	[SerializeField] public List<Transform> duckPositions;
	
	[SerializeField] private float smoothDamp;
	
	[SerializeField] private LayerMask obstacleMask;
	
	[Range(0, 5)]
	[SerializeField] private  float minTimeToChangeDirection;
	
	[Range(0, 5)]
	[SerializeField] private  float maxTimeToChangeDirection;
	
	
	[Header("Ice")]
	[SerializeField] private ParticleSystem iceParticleSystem;

    [SerializeField] private GameObject ice;
	
	[Header("Duck Animation")]
	
	[SerializeField] private SkinnedMeshRenderer duckMeshRenderer;

	[SerializeField] private float timeToSquish = 0.5f;

	[SerializeField] private float timeToWalk = 1f;
	
	#endregion

	#region private Fields

	private bool _isPlayerSprinting;

	private Camera _mainCamera;

	private float _timeToChangeDirection;
	
	private float _timeToChangeDirectionCounter;
	
	private Vector3 _direction;

	private Renderer _objectRender;

	private bool _alreadyShot = false;

	private List<Duck> _neighbours = new List<Duck>();
	
	private Vector3 _currentVelocity;
	
	private float _speed;

	private float _timeToWalk;

	private float _animationStartTime;
	
	private float _animationStartCounter;

	private bool _canStartAnimation;

	private bool _changedSpeed;

	private float _elapsedWalkTime = 0f;
	
	// has the duck reached the walking animation end?
	private bool _hasReachedAnimEnd = false;

	private bool _isInLake = false;
	
	//Todo - delete
    private bool _gotShotAlready;
    #endregion

	#region Properties

	public Transform MyTransform { get; set; }

	#endregion

	#region Constants
	
	private const int MaxBlendShapeValue = 100;

	private const int MinBlendShapeValue = 0;

	private const int DuckStandIdleShapeValue = 50;

	private const int WalkBlendLayer = 0;

	private const int SquishBlendLayer = 1;
	

	#endregion

	#region Mono Behaviour

	private void Awake()
	{
		_mainCamera = Camera.main;
		_objectRender = gameObject.GetComponent<Renderer>();
		MyTransform = transform;
	}

	private void Start()
	{
		_timeToWalk = timeToWalk;
		_speed = Random.Range(Flock.minSpeed, Flock.maxSpeed);
		_direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
		_timeToChangeDirection = Random.Range(minTimeToChangeDirection, maxTimeToChangeDirection);
		transform.LookAt(_direction);

		_animationStartTime = Random.Range(1f, 5f);

	}

	private void Update()
	{

		
			if (!gotHit)
			{
				_timeToChangeDirectionCounter += Time.deltaTime;
				
				if (_timeToChangeDirectionCounter >= _timeToChangeDirection)
				{
					_timeToChangeDirection = Random.Range(minTimeToChangeDirection, maxTimeToChangeDirection);
					_direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
					transform.rotation = Quaternion.LookRotation (_direction);
					_timeToChangeDirectionCounter = 0;
				}
				
				// Normalize the direction to ensure the object moves at a consistent speed
				_direction.Normalize();
				
				// Check if there are any obstacles in the chosen direction
				RaycastHit hit;
				if (Physics.Raycast(transform.position, _direction, out hit, 2, obstacleMask))
				{
					// If there is an obstacle, move in a random direction perpendicular to the obstacle
					_direction = Quaternion.Euler(0, 90, 0) * _direction;
					transform.rotation = Quaternion.LookRotation (_direction);
				}
				
				// Move the object in the chosen direction
				transform.position +=  _speed  * Time.deltaTime * _direction;
			}

	}

	private void FixedUpdate()
	{
		if (!_canStartAnimation)
		{
			_animationStartCounter += Time.deltaTime;	
		}
		if (_animationStartCounter >= _animationStartTime)
		{
			_canStartAnimation = true;
		}
		if (!_isInLake && _canStartAnimation)
		{
			CalcWalkBlendShapeValue();
		}
	}

	#endregion
    
   
   #region Public Methods

   public void DontMove()
   {
	   gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
   }
   /// <summary>
   /// A coroutine used to move the duck object from current location to a new one is a smooth way.
   /// </summary>
   /// <param name="timeToMove">
   ///  how much time it takes object to move from positons
   /// </param>
   public IEnumerator MoveDuckCoroutine(float timeToMove)
   {
	   
	   foreach (var Curtransform in duckPositions)
       {
           Vector3 curPosition = this.transform.position;
           var curRotation = transform.rotation;
           Vector3 newPos;
           Quaternion newRot;
           float elapsedTime = 0f;
                 
           while (elapsedTime < timeToMove)
           {
               elapsedTime += Time.deltaTime;
               newPos = Vector3.Lerp(curPosition, Curtransform.position, elapsedTime / timeToMove);
               newRot = Quaternion.Lerp(curRotation, Curtransform.rotation, elapsedTime / timeToMove );
               this.transform.position = newPos;
               this.transform.rotation = newRot;
               yield return null;
           }

           
           MyTransform.position = Curtransform.position;
           MyTransform.rotation = Curtransform.rotation;
           
           
           _isInLake = true;
           duckMeshRenderer.SetBlendShapeWeight(WalkBlendLayer, DuckStandIdleShapeValue);
           GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
       }
   }
   

   public void MoveUnit()
   {
	   FindNeighbours();
	   CalculateSpeed();

	   if (EventManager.Instance.IsPlayerSprinting && !_isPlayerSprinting)
	   {
		   _speed *= 3f;
		   _isPlayerSprinting = true;
	   }
	   else if (!EventManager.Instance.IsPlayerSprinting && _isPlayerSprinting)
	   {
		   _speed /= 3f;
		   _isPlayerSprinting = false;
	   }

	   var cohesionVector = CalculateCohesionVector() * Flock.CohesionWeight;
	   var alignmentVector = CalculateAligementVector() * Flock.AligementWeight;
	   var moveVector = cohesionVector + alignmentVector;
	   moveVector = Vector3.SmoothDamp(MyTransform.forward, moveVector, ref _currentVelocity, smoothDamp);
	   moveVector = moveVector.normalized * _speed;
	   if (moveVector == Vector3.zero)
		   moveVector = transform.forward;
	   moveVector = new Vector3(moveVector.x, 0, moveVector.z);
	   MyTransform.forward = moveVector;
	   MyTransform.position += moveVector * Time.deltaTime;
   }

   
   #endregion

   #region Private Methods


   private void FindNeighbours()
	{
		_neighbours.Clear();
		var allUnits = Flock.AllUnits;
		for (int i = 0; i < allUnits.Count; i++)
		{
			var currentUnit = allUnits[i];
			if (currentUnit != this)
			{
				_neighbours.Add(currentUnit);
			}
		}
	}

	private void CalculateSpeed()
	{
		if (_neighbours.Count == 0)
			return;
		_speed = 0;
		for (int i = 0; i < _neighbours.Count; i++)
		{
			_speed += _neighbours[i]._speed;
		}

		_speed /= _neighbours.Count;
		_speed = Mathf.Clamp(_speed, Flock.minSpeed, Flock.maxSpeed);
	}

	private Vector3 CalculateCohesionVector()
	{

		var playerPosition = Flock.PlayerTransform.position;
		var cohesionVector = Flock.PlayerWeight * new Vector3(playerPosition.x, 0, playerPosition.z);

		for (int i = 0; i < _neighbours.Count; i++)
		{
			cohesionVector += _neighbours[i].MyTransform.position;
		}
		
		cohesionVector /= _neighbours.Count + Flock.PlayerWeight;
		cohesionVector -= MyTransform.position;
		cohesionVector = cohesionVector.normalized;
		return new Vector3(cohesionVector.x, 0, cohesionVector.z);
	}
	
	
	private Vector3 CalculateAligementVector()
	{
		var aligementVector = Flock.PlayerWeight * Flock.PlayerTransform.forward + MyTransform.forward;
		for (int i = 0; i < _neighbours.Count; i++)
		{
			aligementVector += _neighbours[i].MyTransform.forward;
		}

		aligementVector /= Flock.PlayerWeight + _neighbours.Count + 1;
		aligementVector = aligementVector.normalized;
		return aligementVector;
	}

	private void CalcWalkBlendShapeValue()
	{

		_timeToWalk = _isPlayerSprinting ? timeToWalk : 2 * timeToWalk;
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
		
		duckMeshRenderer.SetBlendShapeWeight(WalkBlendLayer, curBlendValue);
	}

	private void BreakIce()
	{
		ice.SetActive(false);
		iceParticleSystem.Play();
		AudioManager.Instance.PlayIceBreaksSound();
	}

	private IEnumerator SquishDuck(bool shouldSquish)
	{	
		if (shouldSquish)
			AudioManager.Instance.PlayDuckSquishSound();
		
		var a = shouldSquish ? MinBlendShapeValue : MaxBlendShapeValue;
		var b = shouldSquish ? MaxBlendShapeValue : MinBlendShapeValue;
		var elapsedTime = 0f;
		float value;
		while (elapsedTime < timeToSquish)
		{
			elapsedTime += Time.deltaTime;
			value = Mathf.Lerp(a, b, elapsedTime / timeToSquish);
			duckMeshRenderer.SetBlendShapeWeight(SquishBlendLayer, value);
			yield return null;
		}
		duckMeshRenderer.SetBlendShapeWeight(SquishBlendLayer, b);
	}

   #endregion
   
   #region Public Methods

   public Vector3 GetDuckPosInLake()
   {
	   return duckPositions[^1].position;
   }

   public void SetDuckColliderActive(bool isActive)
   {
	   gameObject.GetComponent<Collider>().enabled = isActive;
   }

   public void ToggleSquish(bool shouldSquish)
   {
	   //duckAnimator.SetBool(_animIdSquish, true);
	   StartCoroutine(SquishDuck(shouldSquish));
   }
   #endregion

   public bool IsAlreadyHit()
   {
	   return gotHit;
   }

   public void HornUsedOnObject()
   {
	   if (!_alreadyShot && !gotHit)
	   {	
		   //Debug.Log("duck caught in sphere");
		   _alreadyShot = true;
		   gotHit = true;
		   BreakIce();

		   GetComponent<AudioSource>().Stop();
		   Flock.AddDuck(this);
		   Flock.CanMoveDucks = true;
	   }
   }
   

   
}
