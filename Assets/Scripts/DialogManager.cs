using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textDisplay = null;
    [SerializeField]
    private string[] sentences = null;
    private int index;
    private float typingSpeed;
    private PlayableDirector director;
    [SerializeField]
    private Canvas dialogCanvas = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Type());
        typingSpeed = 0.05f;
        director = GameObject.FindGameObjectWithTag("director").GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the text in realtime, checks for cutscene skip input
        if (textDisplay.text == sentences[index])
        {
            Invoke(nameof(NextSentence), 1.0f);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            dialogCanvas.enabled = false;
            director.time = director.playableAsset.duration;
        }
    }

    //Types out the text that was set in the inspector, letter by letter
    IEnumerator Type()
    {
        if(sentences[index] == "pause")
        {
            yield return new WaitForSeconds(1.0f);
            sentences[index] = "";
        }

        foreach(char letter in sentences[index].ToCharArray())
        {
            if(sentences[index] != "pause")
            {
                textDisplay.text += letter;
                if (AudioManager.instance != null)
                    AudioManager.instance.Play("KeyPress");
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
    //Starts typing the next sentence in the array
    void NextSentence()
    {
        CancelInvoke("NextSentence");

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
            textDisplay.text = "";
    }
}
