using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // player has collected the horn.
        if (other.CompareTag("Player"))
        {   
            other.gameObject.GetComponent<PlayerHornAbility>().canUseHorn = true;
            AudioManager.Instance.PlayHornPickUpSound();
            GetComponent<AudioSource>().Stop();
            gameObject.SetActive(false);
        }
}
}
