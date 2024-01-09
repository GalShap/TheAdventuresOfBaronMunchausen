using System;
using UnityEngine;

public class CarrotIceBerg : MonoBehaviour, IHornResponsive
{
    
    [SerializeField] private ParticleSystem iceParticleSystem;

    [SerializeField] private GameObject iceObject;

    [SerializeField] private bool isHit = false; 
    
    private Collider _iceCollider;
    
    private void Awake()
    {
        _iceCollider = _iceCollider = GetComponent<BoxCollider>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {




    }

    public void BreakIce()
    {
        iceObject.SetActive(false);
        iceParticleSystem.Play();
        _iceCollider.enabled = false;
        AudioManager.Instance.PlayIceBreaksSound();
    }

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
