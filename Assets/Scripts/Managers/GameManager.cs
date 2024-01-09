using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class GameManager : SingletonPersistent<GameManager>
{
    
    [Header("Debugging")]
    [Tooltip("for debugging only, use this if you want to put ducks next to baron")]
    [SerializeField] private bool allDucksToBaron = false;
    
    public bool IsDeerMoving = false;

    public bool GoToStart = false;

    public enum GameState
    {
        MainMenu, OpeningAnimation , Game, PauseMenu, AfterDeerWakeUp, GotToChruch, Ending
    }
    public int curGameState { get; set; } = (int) GameState.Game;

    #region constants
        
    private const int AllDucksShot = 0;

    #endregion
    
    private void Start()
    {
        if (allDucksToBaron) EventManager.Instance.SetAllDucksOnBaron();
        if (GoToStart) EventManager.Instance.MovePlayerToDefaultPos();
    }

    private void Update()
    {
        
    }

    public void PauseGame()
    {   
        // can only pause when in game
        if (curGameState == (int) GameState.MainMenu || curGameState == (int) GameState.PauseMenu) return;
        
        curGameState = (int) GameState.PauseMenu;
        
        Time.timeScale = 0f;
        
        UiManager.Instance.SetPauseMenu(true);
    }

    public void ResumeGame()
    {
     
        UiManager.Instance.SetPauseMenu(false);
        
        Time.timeScale = 1f; 
        
        curGameState = (int) GameState.Game; 
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        var pauseMenu = UiManager.Instance.GetPauseMenuCanvas();
        var openingScene = UiManager.Instance.GetStartAnimationCanvas();
        UiManager.Instance.firstFrame.SetActive(true);
       
        StartCoroutine(UiManager.Instance.LoadScene("Game", (int) GameState.Game,
            pauseMenu, null));

    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        var pauseMenu = UiManager.Instance.GetPauseMenuCanvas();
        var mainMenu = UiManager.Instance.GetMainMenuCanvas();
        UiManager.Instance.firstFrame.SetActive(true);
       
        StartCoroutine(UiManager.Instance.LoadScene("MainMenu", (int) GameState.MainMenu,
            pauseMenu, mainMenu));
    }
    

    public void StartGame()
    {   
        if (Player.Instance != null) Destroy(Player.Instance);
        var pauseMenu = UiManager.Instance.GetPauseMenuCanvas();
        var openingScene = UiManager.Instance.GetStartAnimationCanvas();
        StartCoroutine(UiManager.Instance.LoadScene("Game",(int) GameState.Game,
            openingScene, null));
    }


    public void LoadPostDeerWakeScene()
    {   
        
        StartCoroutine(UiManager.Instance.LoadScene("AfterDeerWakeUp",(int) GameState.AfterDeerWakeUp,
            null, null));
    }

    public void LoadChurchScene()
    {   
        
        StartCoroutine(UiManager.Instance.LoadScene("ChurchRoofScene",(int) GameState.GotToChruch,
            null, null));
    }
    
 

    public void LoadEnding(float pauseTime)
    {
        curGameState = (int) GameState.Ending;
        StartCoroutine(UiManager.Instance.LoadEndingCanvas(pauseTime));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool InGame()
    {
        if (curGameState == (int) GameState.Game || curGameState == (int) GameState.AfterDeerWakeUp
                                               || curGameState == (int) GameState.GotToChruch)
            return true;
        
        return false;
    }
    
    // this means something more then just reloading the scene needs to happen
    public void CheckAndApplyHardReset(int prevState, int newState)
    {
        // theres no need to disable anything, since we don't reset a scene. its just the normal scene transition.
        if (prevState != (int) GameState.PauseMenu && prevState != (int) GameState.Ending) return;
       
        var fromPauseToGame = newState == (int) GameState.Game;
       
        var ToMenu = newState == (int) GameState.MainMenu;

        if (fromPauseToGame)
        {
            Destroy(Player.Instance.gameObject);
            Destroy(EventManager.Instance.gameObject);
           
        }
       
        else if (ToMenu)
        {
            Debug.Log("destroying player");
            Destroy(Player.Instance.gameObject);
            Destroy(EventManager.Instance.gameObject);
            Destroy(AudioManager.Instance.gameObject);
            Destroy(Camera.main.GetComponent<PostProcessLayer>());
            Destroy(Camera.main);
        }

    }

}
