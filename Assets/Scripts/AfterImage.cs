using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField]private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;

    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerSR;

    private Color color;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        try { playerSR = player.GetComponent<SpriteRenderer>(); }
        catch (NullReferenceException) { }

        

        alpha = alphaSet;
        try {
            spriteRenderer.sprite = playerSR.sprite;
            transform.position = player.transform.position;
            transform.rotation = player.transform.rotation;
             } 
        catch (NullReferenceException) { }
        
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(0.5f, 0.5f, 1f, alpha);
        spriteRenderer.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
