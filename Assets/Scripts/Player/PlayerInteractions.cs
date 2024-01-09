using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private float slipTime = 3f;

    [Header("BottomHorse")] 
    public HorseBottomHalfBehaviour _bottomHorse;
    [SerializeField] private float speedToCarrot;
    [SerializeField] private float distancefromCarrot = 0;
    

    #endregion

    #region Fields
    private bool _isSprinting;
    
    // animation ids
    private readonly int _slip = Animator.StringToHash("Slip");
    private readonly int _drown = Animator.StringToHash("Drown");

    #endregion
    
    #region  Properties
   
    // public bool IsSprinting
    // {
    //     get => _isSprinting;
    //     set => _isSprinting = value;
    // }

    #endregion

    #region MonoBehaviour
    
    
    private void OnTriggerEnter(Collider other)
    {

        if (GameManager.Instance.curGameState == (int) GameManager.GameState.Game)
        {
            if (Flock.AllUnits.Count == EventManager.Instance.NumOfDucks)
            {
                if (other.gameObject.CompareTag("ActivateDuckPositions"))
                {
                    EventManager.Instance.StartMovingAllDucks();
                }
            }
        }
        else if (GameManager.Instance.curGameState == (int)GameManager.GameState.GotToChruch)
        {
            if (other.gameObject.CompareTag("HorseCombineTrigger"))
            {
                print("Combine");
                _bottomHorse.CombineWithFrontHorse();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance.curGameState == (int)GameManager.GameState.GotToChruch)
        {
            if (other.gameObject.CompareTag("CarrotBlock"))
            {
                if (other.transform.Find("IceBlock").GetComponent<IceBerg>().DidBreak)
                {
                    _bottomHorse.ShouldStand = false;
                    _bottomHorse.Target = other.transform.Find("HorsePoint").transform;
                    _bottomHorse.SpeedToTarget = speedToCarrot;
                    _bottomHorse.DistanceToTarget = distancefromCarrot;
                }
            }
        }

    }

    private IEnumerator SetPlayerPositionAfterSlip(Vector3 newPos, float time)
    {
        var a = gameObject.transform.position;
        var b = newPos;
        float elapsedTime = 0;
      
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(a, b, elapsedTime / time);
            yield return null;
        }

        gameObject.transform.position = newPos;
    }

    private IEnumerator WaitForDrownAnimationEnd(Vector3 newPos,int layer = 0)
    {
        AnimatorStateInfo animState = playerAnimator.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.normalizedTime % 1;
        yield return new WaitForSecondsRealtime(currentTime);
        gameObject.transform.position = newPos;
    }

    #endregion
    
    /// <summary>
    /// should be called when player is trying to enter the lake and hasn't shot the ducks yet. 
    /// </summary>
    /// <param name="other">
    /// the lake collider.
    /// </param>
    public void PlayerSlipOnLake(Vector3 newPos)
    {
        playerAnimator.SetTrigger(_slip);
        print(newPos);
        StartCoroutine(SetPlayerPositionAfterSlip(newPos ,slipTime));
        
    }

    public void PlayerDrownInLake(Vector3 newPos)
    {   
        playerAnimator.SetTrigger(_drown);
        StartCoroutine(WaitForDrownAnimationEnd(newPos));
    }
    
    
}
