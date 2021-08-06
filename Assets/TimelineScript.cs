using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimelineScript : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector director;
    private void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
            SceneManager.LoadScene("Menu");
    }

    private void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }

}
