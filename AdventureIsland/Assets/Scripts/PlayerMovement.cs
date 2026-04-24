using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private int hitCounter = 0;
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

        // --- MOVEMENT LOGIC ---
        if (moveInput > 0)
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
        else if (moveInput < 0)
        {
            velocity.x = moveInput * moveSpeed;
        }
        else
        {
            velocity.x = 0;
        }

        // --- BETTER JUMP CURVE LOGIC ---
        if (velocity.y < 0)
        {
            // If falling, apply extra gravity to make the fall snappier
            velocity.y += Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0 && !isJumpPressed)
        {
            // If moving upwards but jump button is released, apply extra gravity so the jump is shorter
            velocity.y += Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = velocity;

        // Ensure player doesn't walk off the left side of the camera view
        ClampPlayerToCameraBoundary();
    }

    private void ClampPlayerToCameraBoundary()
    {
        if (mainCamera == null) return;

        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        float leftBoundaryX = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera)).x;

        // If the player tries to go past the boundary, enforce position
        if (transform.position.x < leftBoundaryX + playerWidthOffset)
        {
            Vector3 clampedPos = transform.position;
            clampedPos.x = leftBoundaryX + playerWidthOffset;
            transform.position = clampedPos;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Egg"))
        {
            hitCounter++;
            if (hitCounter >= 2)
            {
                canThrow = true;
                Debug.Log("Player can now throw!");
                Destroy(collision.gameObject); // Destroy the egg after hitting it twice
            }
            //Destroy(collision.gameObject); // Destroy the egg after hitting it once

        }
    }
}