using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float rbMass = 15f;
    [SerializeField] private float jumpForce = 150f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = -0.05f;

    private Transform cameraHolder; // contain move direction info
    private Transform cameraTransform;
    private Vector3 moveDirection;

    private bool canJump = true;
    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rb;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private float playerHeight;
    private bool grounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.mass = rbMass;
        cameraHolder = GameObject.FindGameObjectWithTag("CameraHolder").transform;
        cameraTransform = GameObject.FindGameObjectWithTag("Camera").transform;

        playerHeight = GetComponent<CapsuleCollider>().height;
    }

    void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        MyInput();
        SpeedControl();
    }
    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        if (cameraHolder && cameraTransform)
        {
            moveDirection = cameraHolder.forward * verticalInput + cameraTransform.right * horizontalInput;
        }
        else
            Debug.Log("cameraHolder or cameraTransform not found");
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        canJump = true;
    }
}
