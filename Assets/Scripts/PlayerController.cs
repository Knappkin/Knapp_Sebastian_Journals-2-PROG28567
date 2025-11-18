using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Rigidbody
    Rigidbody2D rb;
    public float gravity;
    ////Horizontal Movement
    //[Header("Running Variables")]
    //[SerializeField] private float maxSpeed;
    //[SerializeField] private float acceltime;
    //[SerializeField] private float deceltime;
    [SerializeField] private float moveSpeed;

    //Jumping
    [Header("Jump Variables")]
    [SerializeField] private float apexTime;
    [SerializeField] private float apexHeight;
    private float initialJumpVelo;
    private bool useJump;
    private bool isJumping;
    private float jumpStartTime;
    private float currentJumpTime;

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
        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpVelo = 2 * apexHeight / apexTime;
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
      
        playerInput.x = Input.GetAxisRaw("Horizontal");
        MovementUpdate(playerInput);

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

        

    }

    private void FixedUpdate()
    {

        rb.linearVelocityY += gravity * Time.fixedDeltaTime;

        rb.linearVelocityX = moveSpeed * playerInput.x;
        if (useJump)
        {
            //useJump = false;


            Debug.Log(currentJumpTime);
            rb.linearVelocityY = gravity * currentJumpTime + initialJumpVelo;
        }
    }

    private void MovementUpdate(Vector2 playerInput)
    {

    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return true;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
