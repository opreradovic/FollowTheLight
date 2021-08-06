using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField]
    private MenuScript menuScript = null;

    private Animator anim;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("activeCredits", false);
            menuScript.MainMenu();
        }
    }

    public void EndCredits()
    {
        anim.SetBool("activeCredits", false);
        menuScript.MainMenu();
    }
}
