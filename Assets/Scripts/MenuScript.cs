using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsCanvas = null;
    [SerializeField]
    private GameObject creditsCanvas = null;
    [SerializeField]
    private GameObject levelSelectCanvas = null;

    private Animator creditsAnim;


    public bool isCredits { get; set; }
    private void Start()
    {
        //Checks the last scene that was played, if it is either of the two ending scenes it plays the Credits animation upon finishing the level
        string LastScene = PlayerPrefs.GetString("LastScene", null);
        settingsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
        creditsAnim = creditsCanvas.GetComponentInChildren<Animator>();
        creditsAnim.SetBool("activeCredits", false);
        if(LastScene == "Ending1" || LastScene == "Ending2")
        {
            print("lastcene");
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            Credits();
        }
    }
    private void Update()
    {
        isCredits = creditsCanvas.activeSelf;
    }
    public void StartGame()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Tutorial1");
    }
    //Main menu buttons
    public void LevelSelection()
    {
        this.gameObject.SetActive(false);
        levelSelectCanvas.SetActive(true);
    }
    public void Settings()
    {
        settingsCanvas.SetActive(true);
        if (AudioManager.instance != null)
            AudioManager.instance.SetSliderValue();
        this.gameObject.SetActive(false);
    }
    public void Credits()
    {
        creditsCanvas.SetActive(true);
        creditsAnim.SetBool("activeCredits", true);
        this.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        this.gameObject.SetActive(true);
        settingsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
    }

    //LEVEL SELECTION MANAGEMENT
    #region
    public void Level1()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Spikes1");
    }
    public void Level2()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Spikes2");
    }
    public void Level3()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Spikes3");
    }
    public void Level4()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Walljump1");
    }
    public void Level5()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Walljump2");
    }
    public void Level6()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Walljump3");
    }
    public void Level7()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("JumpPad1");
    }
    public void Level8()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("JumpPad2");
    }
    public void Level9()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("JumpPad3");
    }
    public void Level10()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Ice1");
    }
    public void Level11()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Ice2");
    }
    public void Level12()
    {
        LevelTransition.transitionInstance.LoadNextLevelString("Ice3");
    }
    #endregion
}
