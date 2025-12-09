using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Player rigidbody
    Rigidbody2D rb;
    public float gravity;
    private float normalGrav;

    //Horizontal Movement
    //Variables for controller the speed up, slow down, and max speed of the player's horizontal movement
    [Header("Running Variables")] [SerializeField] private float maxSpeed;
    [SerializeField] private float accelTime;
    [SerializeField] private float decelTime;
    //stores the last facing direction as either 0 or 1 (left or right) //used for turning the sprite direction
    private float lastFacingDir;
    //floats for the acceleration and deceleration of the horizontal movement //Calculated in update so values could be changed at runtime
    private float acceleration;
    private float deceleration;

    //Jumping
    //Variables for jump mechanic, using the apex height and time methods shown in class //Includes variables for terminal velocity and coyote time buffers
    [Header("Jump Variables")] [SerializeField] private float apexTime;
    [SerializeField] private float apexHeight;
    private float initialJumpVelo;
    //Three booleans for jumping //useJump set in update on key press > then the jump happens in fixed update when useJump is true
    private bool useJump; //Gets the initial key press, starts the jump
    //Bool to tell if player is currently in a jump
    private bool isJumping;
    //Bool to control when the player can use jump
    private bool canJump;
    private float jumpStartTime;
    private float currentJumpTime;
    [SerializeField] private float terminalVelo;
    [SerializeField] private float coyoteTime;
    private bool lastFrameGround; //Checking if the previous frame was on the ground
    private bool currentlyOnGround; //Checking if currently on ground. If no but last frame was, start coyote check
    private bool canCoyote; //boolean for whether player can still jump after leaving ground


    //Parachute Variables
    [Header("Parachute Variables")]
    [SerializeField] private GameObject parachuteObj;
    //Booleans for controlling when the parachute is activated, when it can be used
    private bool useChute;
    private bool canChute;
    //Variables for horizontal movement in the air
    [SerializeField] private float maxAirSpeed;
    [SerializeField] private float airAccelTime;
    [SerializeField] private float airDecelTime;
    //Deceleration equivalent, added against current acceleration every fixedupdate frame to slow the player down
    private float airFriction;
    [SerializeField] private float chuteYVelo;

    //Variables for Bounce Pad Mechanic
    [Header("Bounce Pad Variables")]
    [SerializeField] private float bpApexTime;
    [SerializeField] private float bpApexHeight;
    private bool onBP; // Bool to detect whether or not player is standing on a bounce pad
    private bool firstBP; //Basically canBP, is set to false on first bp frame
    private float initialBounceVelo; // Same implementation as jumping, just different (more extreme) values to go higher
    private bool isBouncing;
    private float bounceGrav;
    private float bounceStartTime;
    private float currentBounceTime;

    [Header("Wind Tunnel Variables")]
    [SerializeField] private float windAccelTime; //Horizontal movement variables for wind Tunnels
    private bool useWind; //True if the player is overlapping a wind tunnel //when true adds wind accel to player's acceleration
    private float windSpeed;
    private float windAccel;
    [SerializeField] private float maxWindSpeed; //Max speed is higher when being pushed by wind

   
   
    [SerializeField] public LayerMask groundMask; //Reference to ground mask //Used for both ground and bouncepad layers

    [SerializeField] Vector2 playerInput = new Vector2();
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        normalGrav = -2 * apexHeight / (apexTime * apexTime); //Setting the default gravity for jumping - based on apex height and time
        gravity = normalGrav;

        bounceGrav = -2 * bpApexHeight / (bpApexTime * bpApexTime); //Separate gravity used for when launching off bounce pad

        rb = GetComponent<Rigidbody2D>();

        parachuteObj.SetActive(false); //Start with the parachute off

        isJumping = false; //Will be set to true as soon as player touches ground 
        useJump = false;
        canJump = false; //Will be set to true when player touches ground

        lastFrameGround = false; //Will be set tSo true when player touches ground
        currentlyOnGround = false; //Will be set to true when player touches ground
        canCoyote = false;

        useWind = false;

        firstBP = true; //starts true since player hasnt touched a bounce pad yet
    }

    // Update is called once per frame
    void Update()
    {

        if (onBP && firstBP) //Detects if player is on bounce pad to trigger the bounce launch
        {
           firstBP = false; //Makes it so it the if check will only pass for the first frame the player is on the pad
            isBouncing = true; //then tells fixed update that the player should bounce
            bounceStartTime = Time.deltaTime; //Starting the timer for bouce
            currentBounceTime = bounceStartTime; //current time also set, added to each frame
        }
       
        acceleration = maxSpeed / accelTime; //Calculating accel and decel of player's horizontal movement. In update so the values can be changed at runtime
        deceleration = maxSpeed / decelTime;
        

        airFriction = maxAirSpeed / airDecelTime; //Calculating the friction to add while in the air
        windAccel = maxWindSpeed / windAccelTime; //Calculating the acceleration for when the plaer is in a wind tunnel

        initialJumpVelo = 2 * apexHeight / apexTime; //Jump velo for jump calculation

        initialBounceVelo = 2 * bpApexHeight / bpApexTime; //Bounce velo for bounce calc


        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
      
        playerInput.x = Input.GetAxisRaw("Horizontal");
      

        //if (input)
        if (Input.GetKeyDown(KeyCode.Space) && canJump) //Triggers the jump
        {
            useJump = true;
            jumpStartTime = Time.deltaTime; //getting the start time of the jump
            currentJumpTime = jumpStartTime; //also set current jump time, adds delta time each frame at end of update
        }

        if (Input.GetKey(KeyCode.P) && canChute) //if the player can parachute and *holds* the button. The player can start pressing before the parachute can actually activate, and it will turn on as soon as it is possible
        {
            useChute = true;
            parachuteObj.SetActive(true);
        }
        else //Hiding the parachute if key is let go or can not parachute for other reason (such as lands on the ground)
        {
            useChute = false;
            parachuteObj.SetActive(false);
        }
        
            currentJumpTime += Time.deltaTime;
            currentBounceTime += Time.deltaTime;


        IsWalking();//Calling the check for x movement
      
        CoyoteCheck();//Calling the coyote check

    }

    private void FixedUpdate()
    {
        //PARACHUTE FOR PRESIDENT!!!

        MovementUpdate(playerInput); //Calls the movement function //which then calls either ground or air movement

       if (useChute) //setting the necessary booleans when player is parachuting
        {
            isJumping = false; //isn't jumping (won't apply the y linear velo calcs of jumping)
            isBouncing = false; //isn't bouncing (same here)
            rb.linearVelocityY = chuteYVelo; //Setting the y velo the the chute fall speed set in the inspector
           
        }
       

        if (!IsGrounded()) //Checks if the player is in the ground - calls the isGrounded function in the check
        {
            //Debug.Log("BWAHAHA");  //I don't have the heart to erase this line of code
            canJump = false; //Making it so the player can't jump when in the air (either mid-jump or falling)
           
            if (rb.linearVelocityY < 0) //Makes it so the player can only use the parachute when falling downwards
            {
                canChute = true;
            }
        }

        else //AKA is on the ground
        {
            //Debug.Log("BWEHEHE");
            isJumping = false;
            canJump = true;
            canChute = false;
            isBouncing = false;
            firstBP = true;
        }

        if (useJump && (canJump||canCoyote)) //Gets the use jump from update //If jumping is possible, do it
        {
            isJumping = true;
            canJump = false;
            useJump = false;
        }
       
        if (isJumping)
        {
            rb.linearVelocityY = gravity * currentJumpTime + initialJumpVelo; //Gravity to apply if jumping
        }

        if (isBouncing)
        {
            rb.linearVelocityY = bounceGrav * currentBounceTime + initialBounceVelo; //Gravity to apply if bouncing
        }

        rb.linearVelocityY += gravity * Time.deltaTime; //Adds base gravity per fixed update frame

        if (rb.linearVelocityY < terminalVelo) //Capping vertical fall speed (aka terminal velo)
        {
            rb.linearVelocityY = terminalVelo;
        }
     

    }

    private void MovementUpdate(Vector2 playerInput) //Called every fixed update frame //handles horizontal movement
    {
        if(IsGrounded()) //If grounded > use ground movement
        {
            GroundMovement(playerInput);
        }
        else
        {
            airMovement(playerInput); // >Otherwise use air movement
        }
    }

    private void GroundMovement(Vector2 playerInput)
    {
        if (playerInput.x != 0) //When a movement key is held
        {


            if (Mathf.Abs(rb.linearVelocityX) < maxSpeed) //If not at max speed, add acceleration
            {
                rb.linearVelocityX += playerInput.x * acceleration * Time.deltaTime; //multiplying by player input(will be either 1 or -1)
            }
            else
            {
                rb.linearVelocityX = maxSpeed * playerInput.x; //Capping horizontal speed to max walking speed //Use absolute value to check for that speed in either direction
            }
        }

        else //If no key is held
        {
            if (rb.linearVelocityX < 0.8f && rb.linearVelocityX > -0.8f) //Setting the speed to 0 if it falls within a small range, prevents permanently decelerating/overcorrecting
            {
                rb.linearVelocityX = 0;
            }
            else if (rb.linearVelocityX < 0) //if moving left, add decel
            {
                rb.linearVelocityX += deceleration * Time.deltaTime;
            }
            else if (rb.linearVelocityX > 0) //if moving right, subtract decel
            {
                rb.linearVelocityX -= deceleration * Time.deltaTime;
            }
        }
    }

    private void airMovement(Vector2 playerInput) //Handles horizontal movement when player is not on ground
    {
        float afMulti = 1; //A multiplier ti slow the player down faster if they are above normal max speed after leaving a wind tunnel

        if (useWind)//Movement when currently in wind tunnel
        {
            if (Mathf.Abs(rb.linearVelocityX) < maxWindSpeed) //similar check - is horizontal movement greater than max speed of wind tunnel
            {
                rb.linearVelocityX += windSpeed * windAccel * Time.deltaTime; //In which case add the wind speed times accel (speed includes direction)
            }
        }

        if (playerInput.x != 0) 
        {

           
            if (Mathf.Abs(rb.linearVelocityX) < maxAirSpeed) //max air speed ended up being same as grounded max speed, but good to have the option
            {
                rb.linearVelocityX += playerInput.x * acceleration * Time.deltaTime; 
            }
            
        }
        if (!useWind && Mathf.Abs(rb.linearVelocityX) > maxSpeed) // checks for if speed is above normal limit and player has left the wind tunnel
        {
            afMulti = 3; //stronger multiplier to make it slow downfaster when above normal max
        }
        else
        {
            afMulti = 1; // 
        }
        if (rb.linearVelocityX < 0)
        {
            rb.linearVelocityX += airFriction * Time.deltaTime * afMulti;
        }
        else if (rb.linearVelocityX > 0)
        {
            rb.linearVelocityX -= airFriction * Time.deltaTime * afMulti;
        }

    }
    public bool IsWalking()
    {
        if (playerInput.x != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
       
    }
    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, Vector2.one, 0f, Vector2.down, .2f, groundMask);
        if (hit)
        {      
            if (hit.collider.gameObject.CompareTag("Bouncy"))
            {
                onBP = true;
            }
            else
            {
                onBP = false;
            }
            return true;
        }

        else
        {
            onBP = false;
            return false;    
        }
    }

    public FacingDirection GetFacingDirection()
    {

        if (playerInput.x < 0)
        {
            lastFacingDir = 0;
        }
        if (playerInput.x > 0) 
        {
            
            lastFacingDir = 1;
        }

        if(lastFacingDir == 0)
        {
            return FacingDirection.left;
        }
        else
        {
            return FacingDirection.right;
        }
    }

    private void CoyoteCheck()
    {
        lastFrameGround = currentlyOnGround;

        currentlyOnGround = IsGrounded();

        if (!currentlyOnGround && lastFrameGround && !isJumping)
        {
            StartCoroutine(CoyoteTimeBuffer());
        }
    }
    private IEnumerator CoyoteTimeBuffer()
    {
        float t = Time.deltaTime;
        float startT = Time.deltaTime;
         canCoyote = true;
        while (t - startT < coyoteTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        canCoyote = false;
   
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (useChute)
        {
            Debug.Log("WOOOSH");
            useWind = true;
            windSpeed = collision.GetComponent<WindTunnel>().windPower;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        useWind = false;
        windSpeed = 0;
    }
}
