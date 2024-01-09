using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    private bool _animationStarted = false;

    private float _videoLen;

    private float _time = 0f;

    private void Start()
    {
        _videoLen = UiManager.Instance.GetVideoLength();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UiManager.Instance.IsInFade())
        {

            
            // the fade has ended so start the video 
            if (!_animationStarted && UiManager.Instance.GetStartAnimationCanvas().activeSelf)
            {      
                
                 UiManager.Instance.StartAnimationSequence();
                _animationStarted = true;
            }
            
              // video has ended so need to change scene.
            else if (_animationStarted)
            {
                if (Gamepad.current != null)
                {
                    if (Gamepad.current.startButton.isPressed || Input.GetKeyDown(KeyCode.Escape))
                    {
                        _time = 0f;
                        GameManager.Instance.StartGame();
                    }
                }
                
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameManager.Instance.StartGame();       
                }

                _time += Time.deltaTime;
                if (_time > _videoLen)
                {
                    
                    _time = 0f;
                    GameManager.Instance.StartGame();
                }
            }
        }
    }

    void OnSkip()
    {
        GameManager.Instance.RestartGame();
    }
}
