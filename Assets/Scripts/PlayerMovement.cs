using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float jumpForce = 2f; // Jump height

    private Rigidbody rb; // Rigidbody for physics-based movement
    public bool isGrounded; // Check if player is grounded
    private bool isMoving; // Check if player is moving

    private Vector3 lastPosition = new Vector3(0, 0, 0);

    // Ground check variables
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rotation due to physics
    }

    void Update()
    {
        Groundcheck();
        movement();
        Jump();
    }

    public void Groundcheck()
    {
        // Ground check using Physics.OverlapSphere
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);
        isGrounded = colliders.Length > 0;
    }

    public void movement()
    {
        // Get input for movement
        float moveX = Input.GetAxis("Horizontal"); // Left/Right movement
        float moveZ = Input.GetAxis("Vertical");   // Forward/Backward movement

        // Calculate movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        // Apply horizontal movement
        Vector3 rbVelocity = new Vector3(move.x, rb.velocity.y, move.z);
        rb.velocity = rbVelocity;

        // Check if the player is moving
        if (lastPosition != transform.position && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = transform.position;
    }

    public void Jump()
    {
        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce , ForceMode.Impulse );
        }

       
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the ground check sphere in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}