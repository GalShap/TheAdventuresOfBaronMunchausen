using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private float volume = 0.15f;
    
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private ThirdPersonController thirdPersonController;

    [SerializeField] private PlayerHornAbility playerHornAbility;

    [SerializeField] private GameObject virtualCamPreAnim;

    [SerializeField] private GameObject virtualCamPostAnim;

    [SerializeField] private FloorFootSteps footSteps;

    #region Private Fields

    private static readonly int GotUp = Animator.StringToHash("GotUp");

    #endregion

    public void OnGettingUpEnd()
    {   
        virtualCamPreAnim.SetActive(false);
        virtualCamPostAnim.SetActive(true);
        playerAnimator.SetBool(GotUp, true);
        AudioManager.Instance.SetMusicVol(volume);
        footSteps.enabled = true;
    }

    public void SwitchCamerasOnGettingUp()
    {
       
    }

    public void OnJumpStart()
    {
        thirdPersonController.CanJumpFromAnimation = true;
    }

    public void OnJumpEnd()
    {
        thirdPersonController.CanJumpFromAnimation = false;
    }

    public void OnShowHorn()
    {
        playerHornAbility.hornObject.SetActive(true);
    }
    
    
    public void OnDisableHorn()
    {
        playerHornAbility.hornObject.SetActive(false);
    }

    public void OnUseHornEnd()
    {   
        playerHornAbility.BlowTheHorn();
    }


}
