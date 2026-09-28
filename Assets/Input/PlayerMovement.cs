using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private bool isGrounded;
    private Rigidbody2D rb;
    private float moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

   

    // updates independant of the players framerate
    void FixedUpdate()
    {
        // moves the player on the x axis depending on the moveSpeed
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }

    // Takes user input and updates the moveInput variable 
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
        Debug.Log($"moveInput: {moveInput}");
    }

    // when the user presses space the player jumps accordign to the jumpForce
    public void OnJump(InputValue value)
    {
        if(value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
