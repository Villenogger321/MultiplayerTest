using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    Vector2 movementInput = new Vector2();
    Vector3 moveDirection;
    float xRotation;
    Rigidbody rb;
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            this.enabled = false;

        gameObject.tag = "Player";
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    void OnJump()
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    void OnLook(InputValue _value)
    {
        
    }
    void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
