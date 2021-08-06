using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using MilkShake;
using UnityEngine;

public class Player : MonoBehaviour
{
    //DECLARATIONS
    #region
    public Vector2 velocity;
    [SerializeField]
    private float speed = 0;
    private Rigidbody2D rb;
    private bool facingRight = true;

    //Movement variables
    private bool isGrounded;
    [SerializeField]
    private Transform groundCheck = null;
    [SerializeField]
    private float groundCheckRadius = 0;
    [SerializeField]
    private float frontCheckRadius = 0;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float jumpForce = 0;
    private bool isPlayerOnIce = false;
    [SerializeField]
    private float movementForceX = 0;
    private bool hasPlayerLaunched= false;

    //HangTime, jump buffer
    private float hangTime = 0.15f;
    private float hangCounter;
    private float jumpBufferLength = 0.15f;
    private float jumpBufferCount;
    
    private float input = 0.0f;

    //Wallsliding variables
    private bool isTouchingFront;
    [SerializeField]
    private Transform frontCheck = null;
    private bool wallSliding;
    [SerializeField]
    private float wallSlidingSpeed = 0;

    //Walljumping variables
    private bool wallJumping;
    [SerializeField]
    private float xWallForce = 0;
    [SerializeField]
    private float yWallForce = 0;
    [SerializeField]
    private float wallJumpTime = 0;

    private Vector3 respawnPosition = Vector3.zero;

    private Animator anim;

    private PlayableDirector director;

    //Particles
    [SerializeField]
    private ParticleSystem footsteps = null;
    private ParticleSystem.EmissionModule footEmission;

    [SerializeField]
    private ParticleSystem impactEffect = null;
    private bool wasOnGround;

    //Camera shake
    [SerializeField]
    private ShakePreset playerDeathShake = null;
    [SerializeField]
    private ShakePreset playerRespawnShake = null;
    [SerializeField]
    private ShakePreset playerLaunchShake = null;
    [SerializeField]
    private Shaker cameraShaker = null;

    [SerializeField]
    private Canvas pauseMenuCanvas = null;

    //AfterImages Vriables
    private float lastAfterImageXPos;
    [SerializeField]
    private float distanceBetweenImages = 0.5f;
    private GameObject pool;

    private bool isDead = false;

    private IEnumerator canSmallJumpCoroutine;

    private bool canLaunch = true;
    private bool canSmallJump = true;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuCanvas.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        SaveCheckpoint();
        anim = GetComponent<Animator>();
        footEmission = footsteps.emission;
        try{director = GameObject.FindGameObjectWithTag("director").GetComponent<PlayableDirector>();}
        catch(NullReferenceException ex){Debug.Log("director not found" + ex);}
        pool = GameObject.Find("AfterImagePool");
    }

    // Update is called once per frame
    void Update()
    {
        //Without this I keep getting a lot of error messages during cutscenes because the player is disabled 
        if (rb == null)
            return;
        //If a cutscene is playing there is no need for an update loop
        if (director != null && director.state == PlayState.Playing)
            return;
        //Gets Player Input 
        if(isDead == false)
            input = Input.GetAxisRaw("Horizontal");
        else { input = 0; }

        //Limits the player X positive/negative velocity to 3/-3, needed for ice movement,
        //otherwise the player can get too fast when moving on ice because of AddForce (quick and dirty, but it works)
            if (rb.velocity.x >= 3.5f && rb.velocity.x > 0 && !hasPlayerLaunched)
                rb.velocity = new Vector2(3.5f, rb.velocity.y);
            else if (rb.velocity.x <= -3.5 && rb.velocity.x < 0 && !hasPlayerLaunched)
                rb.velocity = new Vector2(-3.5f, rb.velocity.y);

        //GroundedCheck
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        //Hangtime(coyote time)
        if(isGrounded)
        {
            hangCounter = hangTime;
            anim.SetBool("isJumping", false);
        }
        else
            hangCounter -= Time.deltaTime;

        if (wallSliding && Input.GetKeyDown(KeyCode.X))
        {
            WallJump();
            velocity = rb.velocity;
        }

        //Jumpbuffer
        if (Input.GetKeyDown(KeyCode.X))
            jumpBufferCount = jumpBufferLength;
        else
            jumpBufferCount -= Time.deltaTime;

        if (!hasPlayerLaunched && jumpBufferCount >= 0 && hangCounter > 0.0f)
            Jump();


        if(canSmallJump && !wallJumping && rb.velocity.y > 0 && Input.GetKeyUp(KeyCode.X))
            SmallJump();

        //Movement when not on ice
        if (!wallJumping && !isPlayerOnIce)
        {
            rb.velocity = new Vector2(input * speed, rb.velocity.y);
        }
        
        //FrontTouchingCheck
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, frontCheckRadius, whatIsGround);

        //WallslidingCheck
        WallSliding();

        ShowParticleEffects();
        
        //Pause the game!
        if(Input.GetKeyDown(KeyCode.Escape))
            PauseGame();


        if (input != 0 && (input > 0) != facingRight && !wallJumping) Flip();

        //Starts or stops the player running animation based on input
        if (input != 0)
            anim.SetBool("isRunning", true);
        else if (input == 0)
            anim.SetBool("isRunning", false);

        CheckForAfterImage();
    }
    private void FixedUpdate()
    {
        //Movement when player is on ice, movement with addforce makes it really unresponsive and "slidey"
        if (!wallJumping && isPlayerOnIce)
            rb.AddForce(new Vector2(movementForceX * input, 0), ForceMode2D.Force);
    }

    void Jump()
    {
        anim.SetBool("isJumping", true);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpBufferCount = 0;
        isGrounded = false;
        hangCounter = 0.0f;
        isPlayerOnIce = false;
    }

    //Small Jump, if the player releases the jump key he is going to stop going upwards
    void SmallJump()
    {
        velocity = rb.velocity;
        anim.SetBool("isJumping", true);
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .25f);
        isGrounded = false;
        jumpBufferCount = 0;
        hangCounter = 0.0f;
    }
    //Plays the appropriate particle effect, based on type of movement
    void ShowParticleEffects()
    {
        //Show footstep effect
        if (input != 0 && isGrounded)
            footEmission.rateOverTime = 50.0f;
        else
            footEmission.rateOverTime = 0.0f;
        //Show impact effect
        if (!wasOnGround && isGrounded)
        {
            impactEffect.gameObject.SetActive(true);
            impactEffect.Stop();
            impactEffect.Play();
        }
        wasOnGround = isGrounded;
    }
    
    void WallSliding()
    {
        if (!isGrounded && isTouchingFront && input != 0)
        {
            wallSliding = true;
            //print("wallsliding");
            anim.SetBool("isSliding", true);
        }
        else { wallSliding = false; anim.SetBool("isSliding", false); }

        //Wallsliding, if wall is icy player slides faster
        if (wallSliding == true)
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        else if (wallSliding == true && isPlayerOnIce)
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed * 1.25f, float.MaxValue));
    }
    //Pushes the player of the wall and flips him in the opposite direction of the wall
    void WallJump()
    {
        print("walljump");
        wallJumping = true;
        Invoke(nameof(SetWallJumpingToFalse), wallJumpTime);
        if(facingRight)
        rb.velocity = new Vector2(-xWallForce, yWallForce);
        else if(!facingRight)
            rb.velocity = new Vector2(xWallForce, yWallForce);
        Flip();

    }
    //If the player is moving, it plays the ghosting effect
    void CheckForAfterImage()
    {
        if(rb.velocity.x != 0 || rb.velocity.y != 0)
        {
            AfterImagePool.Instance.GetFromPool();
            lastAfterImageXPos = transform.position.x;
            if (Mathf.Abs(transform.position.x - lastAfterImageXPos) > distanceBetweenImages)
            {
                AfterImagePool.Instance.GetFromPool();
                lastAfterImageXPos = transform.position.x;
            }
        }
    }
    //Flips the main Character
    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }
    void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "spike" && isDead == false)
            PlayerDie();
        
        if (collision.gameObject.CompareTag("ice"))
            isPlayerOnIce = true; 
        else if(collision.gameObject.tag != "ice")
            isPlayerOnIce = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "jumpvertical")
        {
            Animator launchAnim = collision.gameObject.GetComponent<Animator>();
            hasPlayerLaunched = true;
            PlayerLaunch(launchAnim, Vector2.up);
        }

        if (collision.gameObject.tag == "finish" && SceneManager.GetActiveScene().name == "Tutorial1")
            LevelTransition.transitionInstance.LoadNextLevelString("Tutorial2");
    }

    //Player kill function
    void PlayerDie()
    {
        isDead = true;
        if(pool != null)
        pool.SetActive(false);

        anim.SetBool("isDead", true);
        cameraShaker.Shake(playerDeathShake);
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Death");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //Respawn
        Invoke(nameof(PlayerRespawn), 1.0f);
    }
    //saves the current location of the player so he can be respawned
    public void SaveCheckpoint()
    {
        respawnPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }
    //respawns the player, plays the respawn animations, shakes the camera and freezes player movement
    void PlayerRespawn()
    {
        isDead = false;
        if(AudioManager.instance != null)
            AudioManager.instance.Play("PlayerRespawn");
        this.transform.position = respawnPosition;
        
        anim.SetBool("isDead", false);
        anim.SetBool("isRespawning", true);
        cameraShaker.Shake(playerRespawnShake);
    }
    //Launches the player when he touches te jumppad and plays the jumppad animation(should be in a completely separate script)
    void PlayerLaunch(Animator launchAnim, Vector2 direction)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Play("JumpPad");
        launchAnim.SetBool("isLaunching", true);

        StopCoroutine(nameof(HandleCanSmallJumpAfterLaunchCo));
        StartCoroutine(nameof(HandleCanSmallJumpAfterLaunchCo));

        StartCoroutine(nameof(HandleLaunchCo));

        rb.velocity = Vector2.zero;
        rb.AddForce(direction * jumpForce * 75, ForceMode2D.Impulse);
        print("launch " + direction);

        cameraShaker.Shake(playerLaunchShake);

        impactEffect.gameObject.SetActive(true);
        impactEffect.transform.localScale *= 2;
        impactEffect.Stop();
        impactEffect.Play();
        StartCoroutine(SetLaunchAnimationToFalse(launchAnim));
    }


    //The two Coroutines are used to fix a bug which happened when the player was doing the smalljump before getting launched.
    //the small jump and the jumppad were trying to change the velocity at the same time, causing weird behaviour.
    private IEnumerator HandleCanSmallJumpAfterLaunchCo()
    {
        canSmallJump = false;
        yield return new WaitForSeconds(1f);
        canSmallJump = true;
    }
    private IEnumerator HandleLaunchCo()
    {
        canLaunch = false;
        yield return new WaitForSeconds(0.1f);
        canLaunch = true;
    }
    //Sets the jumppad launch animation to false after x amount of seconds
    IEnumerator  SetLaunchAnimationToFalse(Animator launchAnim)
    {
        yield return new WaitForSeconds(0.35f);
        launchAnim.SetBool("isLaunching", false);
        hasPlayerLaunched = false;
        impactEffect.transform.localScale /= 2;
    }
    //Allows transitions from respawn animation and allows the player to move again
    void SetRespawnAnimationToFalse()
    {
        anim.SetBool("isRespawning", false);
        if(pool != null)
        pool.SetActive(true);

        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    //Pauses the game and brings the pausemenu up
    void PauseGame()
    {
        pauseMenuCanvas.enabled = !pauseMenuCanvas.enabled;
        if (pauseMenuCanvas.enabled)
            Time.timeScale = 0.0f;
        else if (!pauseMenuCanvas.enabled)
            Time.timeScale = 1.0f;
    }
}
