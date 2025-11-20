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
    private float jumpStartTime;
    private float currentJumpTime;

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
      

        if (Input.GetKeyDown(KeyCode.Space))
        {
            useJump = true;
            isJumping = true;
            jumpStartTime = Time.deltaTime;
            currentJumpTime = jumpStartTime;
        }

        if (isJumping)
        {
            currentJumpTime += Time.deltaTime;
            //Debug.Log(rb.linearVelocityY);
        }

        IsWalking();

       
        Debug.Log(IsGrounded());
        

    }

    private void FixedUpdate()
    {

       // rb.linearVelocityY += gravity * Time.fixedDeltaTime;

        //rb.linearVelocityX = moveSpeed * playerInput.x;
        MovementUpdate(playerInput);


        if (useJump)
        {
            //useJump = false;


            //Debug.Log(currentJumpTime);
            rb.linearVelocityY = gravity * currentJumpTime + initialJumpVelo;
        }
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
            rb.linearVelocityX += playerInput.x * deceleration * Time.deltaTime;
        }
    }

    public bool IsWalking()
    {
        if (rb.linearVelocityX != 0)
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
}
