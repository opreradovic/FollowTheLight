using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SignLogic : MonoBehaviour
{
    private Animator anim;
    [SerializeField]
    private TextMeshProUGUI signText;
    [SerializeField]
    private Canvas canvas = null;
    private float dilateTimer = 0.0f;
    private void Start()
    {
        anim = this.GetComponent<Animator>();
        canvas.enabled = false;
        print("image disabled");
    }
    private void Update()
    {
        dilateTimer += Time.deltaTime;
        signText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Mathf.PingPong(dilateTimer, 0.2f));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            canvas.enabled = true;
            anim.SetBool("playerInTrigger", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            anim.SetBool("playerInTrigger", false);
        }
    }

}
