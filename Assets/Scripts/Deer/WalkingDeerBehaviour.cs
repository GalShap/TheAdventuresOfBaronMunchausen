using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkingDeerBehaviour : MonoBehaviour, IHornResponsive
{
    [SerializeField] private List<Transform> deerLocations;

    [SerializeField] private float timeToWalk = 3f;

    [SerializeField] private CinemachineImpulseSource impulseSource;
    
    private int _curLocationIdx = 0;

    private bool _isMoving = false;

    private bool _finishedMoving = false;
    
    private Gamepad _playerGamePad;

    private void Awake()
    {
        _playerGamePad = Gamepad.current;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }
    
    private void AimWalkingDirection()
    {
        Vector2 aim = _playerGamePad.rightStick.ReadValue();
        aim.x = Mathf.Clamp(aim.x, -90, 90);
        aim.y = Mathf.Clamp(aim.y, -90, 90);
        Vector3 direction = new Vector3(aim.x, 0, aim.y);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private IEnumerator MoveToNextLocation(Transform nextLocation, float timeToMove, bool finalPos)
    {
        GameManager.Instance.IsDeerMoving = true;
        var deerTransform = transform.position;
        var shakeTimer = 0f;

        // position points for lerping
        var startingPos = deerTransform;
        var endingPos = nextLocation.position;
        var curPos = deerTransform;
        
        float elapsedTime = 0f;
        
        AudioManager.Instance.PlayDeerFootSteps();

        while (elapsedTime < timeToMove)
        {
            // generate bump impulse every second
            shakeTimer += Time.deltaTime;
            if (shakeTimer > 1f)
            {   
                impulseSource.GenerateImpulse();
                shakeTimer = 0f;
            }
            
            // lerp position and rotation
            elapsedTime += Time.deltaTime;
            curPos = Vector3.Lerp(startingPos, endingPos, elapsedTime / timeToMove);
            transform.position = curPos;
            yield return null;
        }


        transform.position = nextLocation.position;
        AudioManager.Instance.StopDeerFootSteps(); 
        GameManager.Instance.IsDeerMoving = false;

        if (finalPos)
        {   
            Player.Instance.gameObject.transform.parent = null;
            //DontDestroyOnLoad(Player.Instance.gameObject);
            GameManager.Instance.LoadChurchScene();
        }

    }


    public bool IsAlreadyHit()
    {
        throw new System.NotImplementedException();
    }

    public void HornUsedOnObject()
    {
        if (_curLocationIdx >= deerLocations.Count) return;
        
        if (!_finishedMoving)
        {
            if (_isMoving)
            {
                
                return;
            }

            bool _final = _curLocationIdx == deerLocations.Count - 1;
            StartCoroutine(MoveToNextLocation(deerLocations[_curLocationIdx], timeToWalk, _final));
            _curLocationIdx++;



        }

    }
}
