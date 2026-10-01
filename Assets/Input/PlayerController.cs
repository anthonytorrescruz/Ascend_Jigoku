using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Functionality")]
    [SerializeField] private float moveSpeed = 20f;
    
    private PlayerActions controller;
    private Vector2 moveInput;
    private Vector2 targetVelocity;
    private Vector2 velocityRef;

    [SerializeField] private float smoothTime = 0.01f;
    [SerializeField]private bool facingRight = true;

    [Header("Jump Functionality")]

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGrounded;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int maxJumpCount = 2;
    private int jumpCount;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Space(10)]
    [SerializeField] private Rigidbody2D rb;
    void Awake()
    {
        // initializes the players movement actions
        controller = new PlayerActions();
    }

    void OnEnable()
    {
        // enables the keybindings for inputs built into unity
        controller.Enable();
        controller.PlayerInput.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        controller.Disable();
        controller.PlayerInput.Jump.performed -= OnJump;
    }

    void Update()
    {
        // updates the value depending on if the player moves left or right
        //example left is -1 and right is 1
        moveInput = controller.PlayerInput.Move.ReadValue<Vector2>();
        // decides if the sprite should flip
        HandleFlip();

        // checks to see if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if(!wasGrounded && isGrounded)
        {
            jumpCount = 0;
        }

        wasGrounded = isGrounded;
    }

    private void HandleFlip()
    {
        // checks if the player is moving
        if(Mathf.Abs(rb.linearVelocityX) > 0.1f)
        {
            bool moveRight = rb.linearVelocityX > 0;
            // if player is moving one direction but facing the wrong direction
            if(moveRight != facingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        // changes value to show if the player is facing left or right
        facingRight = !facingRight;

        // changes the scale to -1 which just flips the sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void FixedUpdate()
    {
        // the targetVelocity is what the speed we want to get to is
        targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);
        // slowly builds up to the targetVelocity making the movement feel smoother
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocityRef, smoothTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // if the sprite is grounded than apply the jumpforce to the y position.
        if(jumpCount < maxJumpCount)
        {
            // immediatly changes the isGrounded variable to false so you can't jump again before the OverlapCircle
            isGrounded = false;
            jumpCount++;
            rb.linearVelocityY = 0;
            rb.linearVelocityY = jumpForce;
        }
    }
}
