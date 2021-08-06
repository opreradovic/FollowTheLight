using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.SceneManagement;
using MilkShake;
using UnityEngine;

public class MothLogic : MonoBehaviour
{
    //[SerializeField]
    //private Animator animator = null;
    [SerializeField]
    private AIPath aiPath = null;
    [SerializeField]
    private AIDestinationSetter destinationSetter = null;
    [SerializeField]
    private GameObject[] lights = null;
    [SerializeField]
    private float lightRange = 0.0f;
    [SerializeField]
    private GameObject[] lightSwitches = null;
    List<GameObject> lightSwitchesOn = null;
    [SerializeField]
    private GameObject[] bugCatchers = null;
    [SerializeField]
    private float bugCatcherRange= 0.0f;

    private Vector3 mousePos = Vector3.zero;

    [SerializeField]
    private ParticleSystem particleExplosion = null;
    [SerializeField]
    private ParticleSystem heartParticle = null;

    [SerializeField]
    private ShakePreset mothDeathShake = null;
    [SerializeField]
    private Shaker cameraShaker;

    private bool canMothDie = true;

    // Start is called before the first frame update
    void Start()
    {
        //Creates a new list, sets the ai target to nothing
        lightSwitchesOn = new List<GameObject>();
        destinationSetter.target = null;
        InvokeRepeating(nameof(CheckLightDistance), 0, 1.0f);
        InvokeRepeating(nameof(CheckBugCatcherDistance), 0, 0.5f);
        cameraShaker = Camera.main.GetComponent<Shaker>();
    }

    // Update is called once per frame
    void Update()
    {
        //Moth follow mouse logic in the menus
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos); 
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            transform.position = Vector2.Lerp(transform.position, mousePos, 0.01f);
        }

        //Checks if a light swithc is on, if yes then it adds them to a list, if not removes them from the list
        foreach(GameObject ls in lightSwitches)
        {
            LightSwitch lsScript = ls.GetComponent<LightSwitch>();

            if(lsScript.isLightOn)
            {
                if(!lightSwitchesOn.Contains(ls))
                lightSwitchesOn.Add(ls);
            }
            else if(lsScript.isLightOn == false)
            {
                lightSwitchesOn.Remove(ls);
            }
        }

        //It doesn't allow the AI moth to move if 2 or more light swithces are on.
        if (lightSwitchesOn.Count >= 2)
            aiPath.canMove = false;
        else if(lightSwitchesOn.Count < 2)
            aiPath.canMove = true;
    }

    void CheckLightDistance()
    { 
        //Checks if a light is on, if yes and in ai moth's range it sets the light as a target
       foreach(GameObject l in lights)
        {
            if(l.activeSelf)
                if(Vector3.Distance(l.transform.position, this.transform.position) <= lightRange)
                    destinationSetter.target = l.transform;       
        } 
    }
    //Checks if the bug lamp is in range, if yes it's set as the moths target
    void CheckBugCatcherDistance()
    {
        foreach(GameObject b in bugCatchers)
        {          
            if (b.gameObject.GetComponent<BugCatcher>().isBugCatcherOn == true)
                if (Vector3.Distance(b.transform.position, this.transform.position) <= bugCatcherRange)
                    destinationSetter.target = b.transform;
        }
    }

    //Really really messy, works with string which I don't like. The game is small so it isn't that bad, on a bigger project make a separate script
    //follow the SOLID pattern, moth shouldn't be responsible for the level change logic
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Plays the correct last level, based on the times the moth has died
        if (collision.gameObject.CompareTag("finish")
            && SceneManager.GetActiveScene().name == "Ice3"
            && AudioManager.instance.MothDeathCount > 10)
                {
                    LevelTransition.transitionInstance.LoadNextLevelString("Ending2");
                    PlayerPrefs.SetString("LastScene", "Ending2");
                    PlayerPrefs.Save();
                    print(PlayerPrefs.GetString("LastScene"));
                }
        else if (collision.gameObject.CompareTag("finish")
            && SceneManager.GetActiveScene().name == "Ice3"
            && AudioManager.instance.MothDeathCount < 10)
                {
                    LevelTransition.transitionInstance.LoadNextLevelString("Ending1");
                    PlayerPrefs.SetString("LastScene", "Ending1");
                    PlayerPrefs.Save();
                    print(PlayerPrefs.GetString("LastScene"));
                }
        


        else if (collision.gameObject.CompareTag("finish"))
        {
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            LevelTransition.transitionInstance.LoadNextLevelIndex(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (collision.gameObject.CompareTag("bugcatcher") && collision.GetComponent<BugCatcher>().isBugCatcherOn && canMothDie)
        {
            canMothDie = false;
            StartCoroutine(ResetMothDeath());
            particleExplosion.Play();
            cameraShaker.Shake(mothDeathShake);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(RestartLevel());
            if (AudioManager.instance != null)
            {
                AudioManager.instance.Play("MothDeath");
                AudioManager.instance.MothDeathCount++;
            }
        }
        if(collision.gameObject.CompareTag("Player"))
            {
                heartParticle.Play();
            }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(0.5f);
        LevelTransition.transitionInstance.LoadNextLevelIndex(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
    //In some cases the moth could die twice!
    IEnumerator ResetMothDeath() {
        yield return new WaitForSeconds(0.25f);
        canMothDie = true;
    }
}
