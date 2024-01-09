using UnityEngine;
using UnityEngine.Events;

public class GameCollider : MonoBehaviour
{
  
    [SerializeField] private Collider triggerCollider;

    [SerializeField] private UnityEvent whatToDo;

    private void Awake()
    {
        triggerCollider.enabled = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("player reached here");
            TurnColliderToSolid();
        }

    }

    private void TurnColliderToSolid()
    {
        triggerCollider.isTrigger = false;
    }
}

    
