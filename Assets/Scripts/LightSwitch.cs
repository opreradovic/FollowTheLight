using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField]
    private Sprite lightOnSprite = null;
    [SerializeField]
    private Sprite lightOffSprite = null;
    [SerializeField]
    private GameObject lightPosition = null;
    [SerializeField]
    private GameObject realLight = null;

    private SpriteRenderer spriteRenderer = null;

    public bool isLightOn { get; set; }


    public bool inTrigger = false;
    private void Start()
    {
        lightPosition.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            inTrigger = false;
        }
    }
    private void Update()
    {
        //If the player is near the light swithc and presses C it is turned on//off, and his position is NOT saved
        if(inTrigger && Input.GetKeyDown(KeyCode.C))
        {
            LightSwitchLogic();
        }
        if (isLightOn)
        {
            spriteRenderer.sprite = lightOnSprite;

        }
        else if (!isLightOn)
        {
            spriteRenderer.sprite = lightOffSprite;

        }
    }

    //Based on the lights condition it is either turned on or off
    void LightSwitchLogic()
    {
        if(AudioManager.instance != null)
        AudioManager.instance.Play("LightSwitch");

        if (!isLightOn)
        {
            lightPosition.SetActive(true);
            realLight.SetActive(true);
            AudioManager.instance.Play("LightBuzzing");
        }
        if (isLightOn)
        {
            lightPosition.SetActive(false);
            realLight.SetActive(false);
            AudioManager.instance.StopPlaying("LightBuzzing");
        }

        isLightOn = !isLightOn;
    }

}
