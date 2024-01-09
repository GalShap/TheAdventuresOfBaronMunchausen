using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyWaitingPoint : MonoBehaviour
{

    #region Fields

    private BunnyBehaviour _bunny;

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        _bunny = transform.parent.GetComponent<BunnyBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _bunny.CanMoveToNextPoint = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _bunny.CanMoveToNextPoint = true;
        }
    }

    #endregion
}
