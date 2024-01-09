using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Flocking {
    public class PlayerPeepController : MonoBehaviour {
        [SerializeField] PeepController peep;
        [SerializeField] InputActionReference moveAction;


        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Start()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }

        protected void Reset() {
            if (peep == null) {
                peep = GetComponent<PeepController>();
            }
        }

        protected void OnEnable() {
            moveAction.action.Enable();
            moveAction.action.started += OnMove;
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        }

        protected void OnDisable() {
            moveAction.action.started -= OnMove;
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
        }

        private void OnMove(InputAction.CallbackContext context) {
            
            var value = context.action.ReadValue<Vector2>();
            peep.DesiredVelocity = value;
        }

        public void Init()
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            peep.Velocity = Vector2.zero;
            peep.DesiredVelocity = Vector2.zero;
         
        }
    }
}
