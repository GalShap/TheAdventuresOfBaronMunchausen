using System;
using Cinemachine;
using StarterAssets;
using UnityEngine;

namespace Utilities
{
    public class ChurchRoofSceneSetUp : MonoBehaviour
    {

        [SerializeField] private Animator playerAnimator;

        [SerializeField] private Animator introAnimator;

        private bool _isSceneLoaded;


        // Start is called before the first frame update
        void Start()
        {   
            GameManager.Instance.curGameState = (int) GameManager.GameState.GotToChruch;
            playerAnimator.SetBool("GotUp", true);
            
           
        }

      

        // Update is called once per frame
        void Update()
        {
            if (!UiManager.Instance.IsInFade() && !_isSceneLoaded)
            {
                introAnimator.enabled = true;
            }
        }
    }
}
