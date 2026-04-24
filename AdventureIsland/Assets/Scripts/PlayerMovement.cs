using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private bool canThrow = false;

    [Header("Environment and Camera")]
    public Transform levelEnvironment;
    private Camera mainCamera;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float playerWidthOffset = 0.5f; 
    private float scrollThresholdX; 

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float fallMultiplier = 2.5f; 
    public float lowJumpMultiplier = 2f;

    [Header("Throwing System")]
    public GameObject projectilePrefab;
    public Transform throwPoint;
    public int poolSize = 10;

    private Queue<GameObject> projectilePool;


    private Rigidbody rb;
    private float moveInput;
    private bool isGrounded;
    private bool isJumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // Record the player's initial X position. 
        // We will keep the player pushing from this point when moving right.
        scrollThresholdX = transform.position.x;

        //Object Pooling Setup
        projectilePool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Enqueue(obj);
        }
    }

    void Update()
    {
        // Check ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Ensure we calculate the boundary FIRST before applying new horizontal velocity
        float leftBoundaryX = GetLeftCameraBoundary();

        // --- MOVEMENT LOGIC ---
        if (moveInput > 0) // Moving Right
        {
            if (transform.position.x < scrollThresholdX)
            {
                velocity.x = moveInput * moveSpeed;
            }
            else
            {
                velocity.x = 0;

                Vector3 pos = transform.position;
                pos.x = scrollThresholdX;
                transform.position = pos;

                if (levelEnvironment != null)
                {
                    levelEnvironment.Translate(Vector3.left * (moveInput * moveSpeed * Time.fixedDeltaTime), Space.World);
                }
            }
        }
        else if (moveInput < 0) // Moving Left
        {
            // If we are significantly inside the boundary, move normally
            if (transform.position.x > leftBoundaryX)
            {
                velocity.x = moveInput * moveSpeed;
            }
            else
            {
                // Push the player right back exactly to the boundary and stop them.
                velocity.x = 0;
                ClampPlayerToCameraBoundary(leftBoundaryX);
            }
        }
        else // Idle
        {
            velocity.x = 0;
            
            // Only clamp when idle to prevent drifting into the wall
            if (transform.position.x < leftBoundaryX)
            {
                ClampPlayerToCameraBoundary(leftBoundaryX);
            }
        }

        // --- BETTER JUMP CURVE LOGIC ---
        if (velocity.y < 0) // Falling
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0 && !isJumpPressed) // Short hop (button released early)
        {
            velocity.y += Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        // Tell the physics engine what to do this frame
        rb.linearVelocity = velocity;
    }

    private float GetLeftCameraBoundary()
    {
        if (mainCamera == null) return -999f;
        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera)).x + playerWidthOffset;
    }

    private void ClampPlayerToCameraBoundary(float leftBoundaryX)
    {
        if (mainCamera == null) return;
        
        Vector3 clampedPos = transform.position;
        clampedPos.x = leftBoundaryX;
        
        // This instantly snaps the Transform to the correct location 
        // without waiting for the Rigidbody to resolve it next frame.
        transform.position = clampedPos;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input.x;
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            isJumpPressed = true;
            if(isGrounded)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        else if(context.canceled)
        {
             isJumpPressed = false;
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && canThrow)
        {
            if (projectilePrefab == null || throwPoint == null)
            {
                Debug.LogError("Projectile prefab or throw point is not assigned!");
                return;
            }
            GameObject objToSpawn = projectilePool.Dequeue();

            projectilePool.Enqueue(objToSpawn);

            Projectile projectileScript = objToSpawn.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Fire(throwPoint.position, Vector3.right);
            }
        }
        
        else if(context.performed && !canThrow)
        {
            Debug.LogError("need to collect egg first!!!");
        }


    }


    public void EnableThrowing()
    {
        canThrow = true;
        Debug.Log("Player collected the Axe and can now throw!");
    }

}