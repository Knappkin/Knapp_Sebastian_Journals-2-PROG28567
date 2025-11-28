using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Rigidbody
    Rigidbody2D rb;
    public float gravity;

    //Horizontal Movement
    [Header("Running Variables")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelTime;
    [SerializeField] private float decelTime;
    [SerializeField] private float moveSpeed;

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
        rb = GetComponent<Rigidbody2D>();

        isJumping = false;
        useJump = false;
        canJump = false;
        lastFrameGround = false;
        currentlyOnGround = false;
        canCoyote = false;


       
    }

    // Update is called once per frame
    void Update()
    {
        acceleration = maxSpeed / accelTime;
        deceleration = maxSpeed / decelTime;
        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpVelo = 2 * apexHeight / apexTime;
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
      
        playerInput.x = Input.GetAxisRaw("Horizontal");
      

        
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            useJump = true;
            jumpStartTime = Time.deltaTime;
            currentJumpTime = jumpStartTime;
        }

        if (isJumping)
        {
            currentJumpTime += Time.deltaTime;
            //Debug.Log(rb.linearVelocityY);
        }


        IsWalking();

        //Debug.Log(rb.linearVelocityX);
        
        CoyoteCheck();
        Debug.Log(canCoyote);

    }

    private void FixedUpdate()
    {

      

        //rb.linearVelocityX = moveSpeed * playerInput.x;
        MovementUpdate(playerInput);

       

        if (!IsGrounded())
        {
            //Debug.Log("BWAHAHA");
            canJump = false;
            rb.linearVelocityY += gravity * Time.deltaTime;
        }
        else
        {
            //Debug.Log("BWEHEHE");
            isJumping = false;
            canJump = true;
            rb.linearVelocityY = 0;
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

        if (rb.linearVelocityY < terminalVelo)
        {
            rb.linearVelocityY = terminalVelo;
        }

        //Debug.Log(isJumping);
        
        //Debug.Log(rb.linearVelocityY);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x != 0)
        {
            
       
        if (Mathf.Abs(rb.linearVelocityX) < maxSpeed)
        {
            rb.linearVelocityX += playerInput.x * acceleration * Time.deltaTime;
            //Debug.Log(rb.linearVelocityX);
        }
            else
            {
                rb.linearVelocityX = maxSpeed * playerInput.x;
            }
        }

        else {
            if (rb.linearVelocityX < 0.4f &&  rb.linearVelocityX > -0.4f)
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
        if (Physics2D.BoxCast(rb.position, Vector2.one, 0f, Vector2.down, .2f, groundMask))
        {
            return true;
        }

        else
        {
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
}
