using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHorseBehaviour : MonoBehaviour
{

    
    [SerializeField] private float timeToWalk;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [SerializeField] private Animator outroDollyCamAnimator;

    [SerializeField] private int amountToPlayAnimation = 4;

    private bool _hasReachedAnimEnd;
    private float _elapsedWalkTime;
    private  int _walkBlendLayer = 1;

    private int _timesAnimationPlayed = 0;
    
    private const int MaxBlendShapeValue = 100;
    private const int MinBlendShapeValue = 0;


    private void Update()
    {
        CalcWalkBlendShapeValue();

        if (_timesAnimationPlayed >= amountToPlayAnimation)
        {
            outroDollyCamAnimator.enabled = true;
        }
        
    }

    private void CalcWalkBlendShapeValue()
    {

        float curA;
        float curB;
        float curBlendValue;
        if (!_hasReachedAnimEnd)
        {   
            // force is increasing
            curA = MinBlendShapeValue;
            curB = MaxBlendShapeValue;
        }
        else
        {   
            // force is decreasing
            curA = MaxBlendShapeValue;
            curB = MinBlendShapeValue;
        }

        if (_elapsedWalkTime < timeToWalk)
        {
            _elapsedWalkTime += Time.deltaTime;
            curBlendValue = Mathf.Lerp(curA, curB, _elapsedWalkTime / timeToWalk);
            
        }

        else
        {
            curBlendValue = curB;
            _elapsedWalkTime = 0f;
            _hasReachedAnimEnd = !_hasReachedAnimEnd;
            
            if (_walkBlendLayer == 1f)
            {
                _walkBlendLayer=0;
            }

            _timesAnimationPlayed++;
        }
		
        meshRenderer.SetBlendShapeWeight(_walkBlendLayer, curBlendValue);
    }
}
