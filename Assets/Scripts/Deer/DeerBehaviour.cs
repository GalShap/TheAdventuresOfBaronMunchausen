using System.Collections;
using UnityEngine;

namespace Deer
{
    public class DeerBehaviour : MonoBehaviour, IHornResponsive 
    {
        [SerializeField] private SkinnedMeshRenderer deerMeshRenderer;
    
        [SerializeField] private float timeToIdleAnimation = 1f;

        [SerializeField] private float timeToWakeUpAnimation = 1f;

        [SerializeField] private GameObject sleepParticleSys;

        private bool _isInWakeUpAnimation = false;
    
        private float _elapsedIdleTime = 0;

        private float _elapsedWakeUpTime = 0;
    
        // the number of times the deer got hit by the horn.
        private int _timesHit = 0;

        private bool _hasReachedIdleAnimEnd = false;

        private bool _wokeUp = false;

        private bool _loadedDeerScene = false;

        private const int MaxBlendShapeValue = 100;

        private const int MidBlendShapeValue = 50;

        private const int MinBlendShapeValue = 0;

        private const int MaxHits = 4;
    
        private const int HornBlownOneTime = 1;
    
        private const int IdleBlendLayer = 0;

        // Update is called once per frame
        void Update()
        {
            if (_wokeUp && !_loadedDeerScene)
            {   
                _loadedDeerScene = true;
                EventManager.Instance.WakeDeerUp();
            } 
            
            if (!_isInWakeUpAnimation && !_wokeUp)
                SetIdleDeerAnimationValue();
        }

        private void SetIdleDeerAnimationValue()
        {
            float curA;
            float curB;
            float curBlendValue;
        
            if (!_hasReachedIdleAnimEnd)
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

            if (_elapsedIdleTime < timeToIdleAnimation)
            {
                _elapsedIdleTime += Time.deltaTime;
                curBlendValue = Mathf.Lerp(curA, curB, _elapsedIdleTime / timeToIdleAnimation);
            }

            else
            {
                curBlendValue = curB;
                _elapsedIdleTime = 0f;
                _hasReachedIdleAnimEnd = !_hasReachedIdleAnimEnd;
            }
		
            deerMeshRenderer.SetBlendShapeWeight(IdleBlendLayer, curBlendValue);
        }
    
        private IEnumerator PlayDeerAnimation(int animationBlendShapeIdx)
        {
            deerMeshRenderer.SetBlendShapeWeight(IdleBlendLayer, MidBlendShapeValue);
            _isInWakeUpAnimation = true;
            AudioManager.Instance.PlayDeerYawnClip(_timesHit);
            var blendValue = MinBlendShapeValue;
            _elapsedWakeUpTime = 0f;
        
            // from min to max (first part of animation)
            while (_elapsedWakeUpTime < timeToWakeUpAnimation)
            {
                _elapsedWakeUpTime += Time.deltaTime;
                blendValue = (int) Mathf.Lerp(MinBlendShapeValue, MaxBlendShapeValue, _elapsedWakeUpTime / timeToWakeUpAnimation);
                deerMeshRenderer.SetBlendShapeWeight(animationBlendShapeIdx, blendValue);
                yield return null;
            } 
        
            deerMeshRenderer.SetBlendShapeWeight(animationBlendShapeIdx, MaxBlendShapeValue);
            _elapsedWakeUpTime = 0f;

            if (_timesHit != MaxHits)
            {
                // from max to min (second part of animation)
                while (_elapsedWakeUpTime < timeToWakeUpAnimation)
                {
                    _elapsedWakeUpTime += Time.deltaTime;
                    blendValue = (int) Mathf.Lerp(MaxBlendShapeValue, MinBlendShapeValue,
                        _elapsedWakeUpTime / timeToWakeUpAnimation);
                    deerMeshRenderer.SetBlendShapeWeight(animationBlendShapeIdx, blendValue);
                    yield return null;
                }

                deerMeshRenderer.SetBlendShapeWeight(animationBlendShapeIdx, MinBlendShapeValue);
                _elapsedWakeUpTime = 0f;
                _isInWakeUpAnimation = false;
            
            }

            else
            {   
                // deer has woken up.
                sleepParticleSys.SetActive(false);
                _wokeUp = true;
            }
            
            deerMeshRenderer.SetBlendShapeWeight(IdleBlendLayer, MinBlendShapeValue);
            _hasReachedIdleAnimEnd = true;
        }

        public bool IsAlreadyHit()
        {
            throw new System.NotImplementedException();
        }

        public void HornUsedOnObject()
        {   
            
            
            if (_timesHit > MaxHits) return;
            
            // there are still animations to be played.
            if (_timesHit < MaxHits && !_isInWakeUpAnimation)
            {

                if (_timesHit == 0)
                    AudioManager.Instance.SetMusicVol(0.05f);
                _timesHit += HornBlownOneTime;
                Debug.Log("Horn used on deer, time: " + _timesHit);
                StartCoroutine(PlayDeerAnimation(_timesHit));
            }

            else if (_timesHit == MaxHits && !_wokeUp)
            {
                _wokeUp = true;
            }
        }   

    }
}
