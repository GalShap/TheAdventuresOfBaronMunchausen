using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Video;


public class UiManager : SingletonPersistent<UiManager>
{
    [Header("menu canvases")]
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] public GameObject resumeButton;

    [SerializeField] private GameObject mainMenuCanvas;

    [SerializeField] private GameObject animationCanvas;

    [SerializeField] private GameObject endingCanvas;

    [SerializeField] private VideoPlayer startingSeqPlayer;

    [SerializeField] public GameObject firstFrame;


    [SerializeField] private Image endingBackgroundImage;

    [SerializeField] private Image endingImage;

    [Header("Fade in and out")] 
    [SerializeField] private GameObject fadeImageObj;
    
    [SerializeField] private float timeToFade = 2;

    private Image _fadeImage;

    private bool _inFade = false;
    
    private const float MaxAlpha = 1f;

    private const float MinAlpha = 0f;

    private const float BlackRGB = 0f;

    private void Start()
    {
        _fadeImage = fadeImageObj.GetComponent<Image>();
       
    }
    
    #region ScreenEffects
    public IEnumerator FadeOut()
    {
        float a = 1f;
        float b = 0f;
        float curTime = 0f;
        float alpha = 0;
        
        while (curTime < timeToFade)
        {
            curTime += Time.deltaTime;
            alpha = Mathf.Lerp(a, b, curTime / timeToFade);
            var newColor = new Color(_fadeImage.color.r,_fadeImage.color.g ,
                _fadeImage.color.b, alpha);
            _fadeImage.color = newColor;
            yield return null;
        }
        
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.r, _fadeImage.color.r, b);
        _inFade = false;
        fadeImageObj.SetActive(false);
    }
    public IEnumerator FadeScreen(bool fadeInOrOut)
    {   
        
        // true is fade out , false is fade in 
        fadeImageObj.SetActive(true);
        float a = fadeInOrOut ? MinAlpha : MaxAlpha;
        float b = fadeInOrOut ? MaxAlpha : MinAlpha;
        float curTime = 0f;
        float alpha = 0;

        while (curTime < timeToFade)
        {
            curTime += Time.deltaTime;
            alpha = Mathf.Lerp(a, b, curTime / timeToFade);
            var newColor = new Color(BlackRGB, BlackRGB, BlackRGB, alpha);
            _fadeImage.color = newColor;
            yield return null;
        }

        _fadeImage.color = new Color(BlackRGB, BlackRGB, BlackRGB, b);
    }
    
    #endregion

    #region PauseMenu
    public void SetPauseMenu(bool active)
    {
        pauseMenu.SetActive(active);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }
    
    public void Restart()
    {
        GameManager.Instance.RestartGame();
    }

    public void MainMenu()
    {
        Debug.Log("got here");
        GameManager.Instance.BackToMainMenu();
    }

    public void Quit()
    {
        GameManager.Instance.QuitGame();
    }
    
    #endregion

    #region StartScene
    
    public void StartAnimationSequence()
    {   
        startingSeqPlayer.Prepare();
        firstFrame.SetActive(false);
        startingSeqPlayer.Play();
    } 
    public bool HasVideoEnded()
    {
        return !startingSeqPlayer.isPlaying;
    }
    public void LoadStartScene()
    {
        StartCoroutine(LoadScene("StartScene",  (int) GameManager.GameState.OpeningAnimation,
            mainMenuCanvas, animationCanvas));
    }
    
    #endregion

    public IEnumerator LoadScene(string sceneName, int newState, [CanBeNull] GameObject uiToDisable, 
        [CanBeNull] GameObject uiToEnable)
    {
        fadeImageObj.SetActive(true);
        _inFade = true;
        float curTime = 0f;
        float alpha = 0;

        while (curTime < timeToFade)
        {
            curTime += Time.deltaTime;
            alpha = Mathf.Lerp(MinAlpha, MaxAlpha, curTime / timeToFade);
            var newColor = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, alpha);
            _fadeImage.color = newColor;
            yield return null;
        }
        if (uiToEnable != null) uiToEnable.SetActive(true);
        if (uiToDisable != null) uiToDisable.SetActive(false);
    
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, MaxAlpha);


        var prevState = GameManager.Instance.curGameState;
        GameManager.Instance.curGameState = newState;
        GameManager.Instance.CheckAndApplyHardReset(prevState, newState);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        
        if (newState == (int) GameManager.GameState.MainMenu)
            Destroy(UiManager.Instance.gameObject);
        
        yield return StartCoroutine(FadeOut());
    }


    public IEnumerator LoadEndingCanvas(float pauseTime)
    {   
        endingCanvas.SetActive(true);
        _inFade = true;
        float curTime = 0f;
        float alpha = 0;

        while (curTime < timeToFade)
        {
            curTime += Time.deltaTime;
            alpha = Mathf.Lerp(MinAlpha, MaxAlpha, curTime / timeToFade);
            var newColor = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, alpha);
            endingBackgroundImage.color = newColor;
            endingImage.color = newColor;
            yield return null;
        }
        
        endingImage.color = Color.white;
        endingBackgroundImage.color = Color.white;

        yield return new WaitForSecondsRealtime(pauseTime);

        _inFade = false;

        yield return StartCoroutine(LoadScene("MainMenu", (int) GameManager.GameState.MainMenu, null, null));


    }
    
   public bool IsInFade()
   {
       return _inFade;
   }

   #region getters

    public GameObject GetPauseMenuCanvas()
    {
        return pauseMenu;
    }

    public GameObject GetStartAnimationCanvas()
    {
        return animationCanvas;
    }

    public GameObject GetEndingCanvas()
    {
        return endingCanvas;
    }

    public GameObject GetMainMenuCanvas()
    {
        return mainMenuCanvas;
    }

    public float GetVideoLength()
    {
        return (float) startingSeqPlayer.length;
    }

    #endregion

}
