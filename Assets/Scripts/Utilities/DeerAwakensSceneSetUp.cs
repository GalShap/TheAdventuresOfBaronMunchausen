using Cinemachine;
using StarterAssets;
using UnityEngine;

namespace Utilities
{
    public class DeerAwakensSceneSetUp : MonoBehaviour
    {
        [SerializeField] private GameObject deer;
        [SerializeField] private CinemachineVirtualCamera sceneVmCamera;
        [SerializeField] private Transform newPlayerPos;

            // Start is called before the first frame update
        void Start()
        {
            SetNewPlayerLocation();
            AudioManager.Instance.SetMusicVol(0.15f);
        }

        void SetNewPlayerLocation()
        {   
            var playerController =  Player.Instance.gameObject.GetComponent<ThirdPersonController>();
            playerController.CinemachineCameraTarget = sceneVmCamera.gameObject;
            playerController.CanControlPlayer = false;
           
            var playerTransform = Player.Instance.transform;
            playerTransform.position = newPlayerPos.position;
            playerTransform.rotation = newPlayerPos.rotation;
            playerTransform.SetParent(deer.transform);
            sceneVmCamera.Follow = playerTransform;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
