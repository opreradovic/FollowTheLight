using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//Level transition singleton, plays the level transition animation on scenechange
public class LevelTransition : MonoBehaviour
{
    public static LevelTransition transitionInstance;

    [SerializeField]
    private Animator transition = null;
    private float transitionTime = 1.0f;
    private void OnEnable()
    {
        print("OnEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        if (transitionInstance == null)
            transitionInstance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transition.SetBool("shouldTransition", false);
    }

    public void LoadNextLevelString(string levelName)
    {
        StartCoroutine(LoadLevelString(levelName));
    }

    IEnumerator LoadLevelString(string levelName = "")
    {
        transition.SetBool("shouldTransition", true);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelName);
    }
    public void LoadNextLevelIndex(int buildIndex)
    {
        StartCoroutine(LoadLevelIndex(buildIndex));
    }
    IEnumerator LoadLevelIndex(int buildIndex)
    {
        transition.SetBool("shouldTransition", true);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(buildIndex);
    }
}
