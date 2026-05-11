using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private bool canThrow = false;
    public Animator animator;
    
    [Header("References")]
    public Transform cameraTergate; 
    public float cameraOffsetX = 3.5f; 

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float leftBoundaryOffset = 1.0f;

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

    [Header("Skateboard Settings")]
    public float skateboardSpeedMultiplier = 1.6f;
    public bool HasSkateboard { get; private set; } = false;

    private Queue<GameObject> projectilePool;
    public Rigidbody rb;
    private float moveInput;
    private bool isGrounded;
    private bool isJumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

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

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        float currentSpeed = HasSkateboard ? (moveSpeed * skateboardSpeedMultiplier) : moveSpeed;
        float targetVelocityX = moveInput * currentSpeed;

        if (Camera.main != null)
        {
            float distanceToCamera = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            float leftBoundaryX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera)).x + leftBoundaryOffset;

            if (transform.position.x <= leftBoundaryX && targetVelocityX < 0)
            {
                targetVelocityX = 0f;
                rb.position = new Vector3(leftBoundaryX, rb.position.y, rb.position.z);
            }
            
            else if (transform.position.x < leftBoundaryX && targetVelocityX == 0)
            {
                rb.position = new Vector3(leftBoundaryX, rb.position.y, rb.position.z);
            }
        }

        velocity.x = targetVelocityX;

        if (velocity.y < 0) // Falling
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0 && !isJumpPressed)
        {
            velocity.y += Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = velocity;
    }

    private void LateUpdate()
    {
        if (cameraTergate != null)
        {
            float desiredX = transform.position.x + cameraOffsetX;

            if (desiredX > cameraTergate.position.x)
            {
                cameraTergate.position = new Vector3(desiredX, cameraTergate.position.y, transform.position.z);
            }
            else
            {
                // X stays locked!
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
            //animator.SetBool("IsGrounded", true);
            isJumpPressed = true;
            animator.SetBool("IsJumping", true);
            if (isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                
            }
                

        }
        else if(context.canceled)
        {
             isJumpPressed = false;
            animator.SetBool("IsJumping", false);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && canThrow)
        {
            if (projectilePrefab == null || throwPoint == null) return;

            animator.SetTrigger("IsThrowing");
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

    public void EnableSkateboard()
    {
        HasSkateboard = true;
        Debug.Log("Skateboard collected! Speed increased.");
    }

    public void LoseSkateboard()
    {
        HasSkateboard = false;
        Debug.Log("Lost Skateboard!");
    }
}