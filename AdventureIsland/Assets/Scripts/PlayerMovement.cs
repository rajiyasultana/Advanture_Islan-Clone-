using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private bool canThrow = false;
    
    [Header("References")]
    public Transform cameraTergate; 

    [Header("Movement")]
    public float moveSpeed = 5f;

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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

    }


    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // --- MOVEMENT LOGIC ---
        velocity.x = moveInput * moveSpeed;

        // --- BETTER JUMP CURVE LOGIC ---
        if (velocity.y < 0) // Falling
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0 && !isJumpPressed) // Short hop
        {
            velocity.y += Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = velocity;
    }

    private void LateUpdate()
    {
        // Update Camera Target (Only push it to the right, never backwards)
        if (cameraTergate != null)
        {
            if (transform.position.x > cameraTergate.position.x)
            {
                // Lock Y and Z to the player, but only advance X forward
                cameraTergate.position = new Vector3(transform.position.x, cameraTergate.position.y, transform.position.z);
            }
            else
            {
                // Player is moving left, but we keep the target Y and Z aligned with the player 
                // so jumping doesn't break the camera, but X stays locked!
                cameraTergate.position = new Vector3(cameraTergate.position.x, cameraTergate.position.y, transform.position.z);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
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
            if (projectilePrefab == null || throwPoint == null) return;
            
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