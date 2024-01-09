using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class IceBerg : MonoBehaviour, IHornResponsive
{

    [SerializeField] private ParticleSystem iceParticleSystem;

    [SerializeField] private GameObject iceObject;

    [SerializeField] private bool isHit = false;

    private bool _didBreak;

    public bool DidBreak
    {
        get => _didBreak;
    }
    private Collider _iceCollider;

    void Awake()
    {
        _iceCollider = GetComponent<BoxCollider>();
    }
    
    void BreakIce()
    {
        iceObject.SetActive(false);
        iceParticleSystem.Play();
        _iceCollider.enabled = false;
        AudioManager.Instance.PlayIceBreaksSound();
        _didBreak = true;
    }
    // Start is called before the first frame update
    public bool IsAlreadyHit()
    {
        return isHit;
    }

    public void HornUsedOnObject()
    {
        if (!IsAlreadyHit())
        {
            BreakIce();
            isHit = true;
        }
    }
}
