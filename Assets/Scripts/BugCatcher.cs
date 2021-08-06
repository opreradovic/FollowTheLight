using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class BugCatcher : MonoBehaviour
{
    private Light2D bugLight;
    public bool isBugCatcherOn { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        bugLight = GetComponent<Light2D>();
        InvokeRepeating(nameof(BugCatcherSwitch), 0, 5f);
    }

    // Turns the bug lamp on and off every 5 seconds.
    void Update()
    {
        if (isBugCatcherOn)
        {
            bugLight.color = new Color(255, 0, 0);
            bugLight.intensity = 0.005f;
            AudioManager.instance.Play("BugLightBuzzing");
        }
        else if (!isBugCatcherOn)
        {
            bugLight.color = new Color(0, 255, 0);
            bugLight.intensity = 0.005f;
            AudioManager.instance.StopPlaying("BugLightBuzzing");
        }
    }
    void BugCatcherSwitch()
    {
        isBugCatcherOn = !isBugCatcherOn;
    }
}
