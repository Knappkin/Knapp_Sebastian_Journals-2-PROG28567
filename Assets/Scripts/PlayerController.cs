using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Rigidbody
    Rigidbody2D rb;
    public float gravity;
    private float normalGrav;
    [SerializeField] private Transform startPos;

    //Horizontal Movement
    [Header("Running Variables")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelTime;
    [SerializeField] private float decelTime;

    private float lastFacingDir;

    private float acceleration;
    private float deceleration;

    //Jumping
    [Header("Jump Variables")]
    [SerializeField] private float apexTime;
    [SerializeField] private float apexHeight;
    private float initialJumpVelo;
    private bool useJump;
    private bool isJumping;
    private bool canJump;
    private float jumpStartTime;
    private float currentJumpTime;
    [SerializeField] private float terminalVelo;
    [SerializeField] private float coyoteTime;


    //Parachute Variable
    [Header("Parachute Variables")]
    [SerializeField] private GameObject parachuteObj;
    private Rigidbody2D chuteRB;
    private bool useChute;
    private bool canChute;
    [SerializeField] private float maxAirSpeed;
    [SerializeField] private float airAccelTime;
    [SerializeField] private float airDecelTime;
    private float airFriction;
    private float airAccel;

    [SerializeField] private float chuteYVelo;
    [SerializeField] private float chuteTurnRate;


    [Header("Bounce Pad Variables")]
    //Bounce pad Variables
    private bool onBP;
    private bool firstBP;
    [SerializeField] private float bpMultiplier;
    private float bpAccel;
    [SerializeField] private float bpApexTime;
    [SerializeField] private float bpApexHeight;
    private float initialBounceVelo;
    private bool isBouncing;
    private float bounceGrav;
    private float bounceStartTime;
    private float bouceJumpTime;

    [Header("Wind Tunnel Variables")]
    private bool useWind;
    private float windSpeed;
    private float windAccel;
    [SerializeField] private float windAccelTime;
    [SerializeField] private float maxWindSpeed;

    private bool lastFrameGround;
    private bool currentlyOnGround;
    private bool canCoyote;
   
    [SerializeField] public LayerMask groundMask;

    [SerializeField] Vector2 playerInput = new Vector2();
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        normalGrav = -2 * apexHeight / (apexTime * apexTime);
        gravity = normalGrav;

        bounceGrav = -2 * bpApexHeight / (bpApexTime * bpApexTime);

        rb = GetComponent<Rigidbody2D>();
        chuteRB = parachuteObj.GetComponent<Rigidbody2D>();

        parachuteObj.SetActive(false);

        isJumping = false;
        useJump = false;
        canJump = false;

        lastFrameGround = false;
        currentlyOnGround = false;
        canCoyote = false;

        useWind = false;

        firstBP = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (onBP)
        {
           isBouncing = true;
        }
       
        acceleration = maxSpeed / accelTime;
        deceleration = maxSpeed / decelTime;
        airAccel = maxAirSpeed / airAccelTime;
        

        airFriction = maxAirSpeed / airDecelTime;
        windAccel = maxWindSpeed / windAccelTime;

        initialJumpVelo = 2 * apexHeight / apexTime;

        initialBounceVelo = 2 * bpApexHeight / bpApexTime;


        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
      
        playerInput.x = Input.GetAxisRaw("Horizontal");
      

        //if (input)
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            useJump = true;
            jumpStartTime = Time.deltaTime;
            currentJumpTime = jumpStartTime;
        }

        if (Input.GetKey(KeyCode.P) && canChute)
        {
            useChute = true;
            parachuteObj.SetActive(true);
        }
        else
        {
            useChute = false;
            parachuteObj.SetActive(false);
        }
        
            currentJumpTime += Time.deltaTime;
        


        IsWalking();

    
      
        CoyoteCheck();

  

    }

    private void FixedUpdate()
    {


        MovementUpdate(playerInput);

       if (useChute)
        {
            isJumping = false;
            rb.linearVelocityY = chuteYVelo;
           
        }
       

        if (!IsGrounded())
        {
            //Debug.Log("BWAHAHA");
            canJump = false;
            rb.linearVelocityY += gravity * Time.deltaTime;
            if (rb.linearVelocityY < 0)
            {
                canChute = true;
            }
        }

        else
        {
            //Debug.Log("BWEHEHE");
            isJumping = false;
            canJump = true;
            canChute = false;
            isBouncing = false;
            firstBP = true;
        }

        if (useJump && (canJump||canCoyote))
        {
            isJumping = true;
            canJump = false;
            useJump = false;
        }
       
        if (isJumping)
        {
            rb.linearVelocityY = gravity * currentJumpTime + initialJumpVelo;
        }

        if (isBouncing)
        {
            rb.linearVelocityY = bounceGrav * currentJumpTime + initialJumpVelo;
        }
        if (rb.linearVelocityY < terminalVelo)
        {
            rb.linearVelocityY = terminalVelo;
        }
       
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if(IsGrounded())
        {
            GroundMovement(playerInput);
        }
        else
        {
            airMovement(playerInput);
        }
        Debug.Log(rb.linearVelocityX);
    }

    private void GroundMovement(Vector2 playerInput)
    {
        if (playerInput.x != 0)
        {


            if (Mathf.Abs(rb.linearVelocityX) < maxSpeed)
            {
                rb.linearVelocityX += playerInput.x * acceleration * Time.deltaTime;
            }
            else
            {
                rb.linearVelocityX = maxSpeed * playerInput.x;
            }
        }

        else
        {
            if (rb.linearVelocityX < 0.8f && rb.linearVelocityX > -0.8f)
            {
                rb.linearVelocityX = 0;
            }
            else if (rb.linearVelocityX < 0)
            {
                rb.linearVelocityX += deceleration * Time.deltaTime;
            }
            else if (rb.linearVelocityX > 0)
            {
                rb.linearVelocityX -= deceleration * Time.deltaTime;
            }
        }
    }

    private void airMovement(Vector2 playerInput)
    {
        float afMulti = 1;
        if (useWind)
        {
            if (Mathf.Abs(rb.linearVelocityX) < maxWindSpeed)
            {
                rb.linearVelocityX += windSpeed * windAccel * Time.deltaTime;
            }
        }

        if (playerInput.x != 0)
        {

           
            if (Mathf.Abs(rb.linearVelocityX) < maxAirSpeed)
            {
                rb.linearVelocityX += playerInput.x * acceleration * Time.deltaTime;     
            }
            
        }
        if (!useWind && Mathf.Abs(rb.linearVelocityX) > maxSpeed)
        {
            afMulti = 3;
        }
        else
        {
            afMulti = 1;
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
