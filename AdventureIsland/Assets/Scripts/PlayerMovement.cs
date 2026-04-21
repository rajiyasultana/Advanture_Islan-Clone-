using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private int hitCounter = 0;
    private bool canThrow = false;

    [Header("Environment and Camera")]
    public Transform levelEnvironment;
    private Camera mainCamera;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float playerWidthOffset = 0.5f; // Offset to prevent player from going through walls
    private float scrollPosX; //the x position from where player should start scrolling the environment

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private float moveInput;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        scrollPosX = transform.position.x; // Initialize scroll position to player's starting x position
    }

    void Update()
    {
        // Check ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        if (moveInput > 0)
        {

            if (transform.position.x < scrollPosX)
            {
                velocity.x = moveInput * moveSpeed;
            }
            else
            {
                velocity.x = 0;
                // Player has reached the scroll line. Stop player from moving right, move environment instead.
                velocity.x = 0;

                // Snap player strictly to the threshold to prevent drifting
                Vector3 pos = transform.position;
                pos.x = scrollPosX;
                transform.position = pos;

                // Move the entire level to the left (-x)
                if (levelEnvironment != null)
                {
                    levelEnvironment.Translate(Vector3.left * (moveInput * moveSpeed * Time.fixedDeltaTime), Space.World);
                }
            }

            if(levelEnvironment != null)
            {
                levelEnvironment.Translate(Vector3.left * (moveInput * moveSpeed * Time.fixedDeltaTime), Space.World);

                //float rightBoundary = levelEnvironment.position.x + (levelEnvironment.localScale.x / 2) - playerWidthOffset;
                //if (transform.position.x < rightBoundary)
                //{
                //    velocity.x = moveInput * moveSpeed;
                //}
            }

        }

        else if (moveInput < 0)
        {
            velocity.x = moveInput * moveSpeed;
        }

        else
        {
            velocity.x = 0;
        }

        rb.linearVelocity = velocity;

        // Ensure player doesn't walk off the left side of the camera view
        ClampPlayerToCameraBoundary();

        //velocity.x = moveInput * moveSpeed;
        //rb.linearVelocity = velocity;
    }

    private void ClampPlayerToCameraBoundary()
    {
        if (mainCamera == null) return;

        // Calculate the absolute distance between the camera's Z and player's Z 
        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        // Find the left boundary x coordinate in world space
        float leftBoundaryX = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera)).x;

        // If the player tries to go past the boundary, enforce position
        if (transform.position.x < leftBoundaryX + playerWidthOffset)
        {
            Vector3 clampedPos = transform.position;
            clampedPos.x = leftBoundaryX + playerWidthOffset;
            transform.position = clampedPos;

            // Optional: Zero out horizontal velocity so they don't slide artificially against the wall edge
            if (rb.linearVelocity.x < 0)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            }
        }

    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input.x;
    }

    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Implement throw logic here
            Debug.Log("Throw action performed");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Egg"))
        {
            hitCounter++;
            if(hitCounter >= 2)
            {
                canThrow = true;
                Debug.Log("Player can now throw!");
                Destroy(collision.gameObject); // Destroy the egg after hitting it twice
            }
        }
    }
}
