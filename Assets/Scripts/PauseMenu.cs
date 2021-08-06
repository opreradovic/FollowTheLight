using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        Time.timeScale = 1.0f;
        this.gameObject.GetComponent<Canvas>().enabled = false;
    }
    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
    }
    public void Quit()
    {
        print("quitgame");
        Application.Quit();
    }
}
