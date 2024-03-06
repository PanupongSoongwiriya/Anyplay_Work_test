using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class ThirdPersonMovement : NetworkBehaviour
{
    private ThirdPersonController TPController;
    private ThirdPersonAnimation TPAnimation;
    private Rigidbody rb;
    private CapsuleCollider cc;
    private Vector3 movementInput;

    [Header("Speed Settings")]
    [SerializeField] private float WalkSpeed = 2.5f;
    [SerializeField] private float CrouchSpeed = 1.5f;
    [SerializeField] private float CrawlSpeed = 1;
    private float CurrentSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float JumpHeight = 5;

    [Header("Grounded Settings")]
    [SerializeField] private float GroundedOffset = -0.14f;
    [SerializeField] private float GroundedRadius = 0.28f;
    [SerializeField] private LayerMask GroundLayers;
    private bool IsGrounded;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            enabled = false;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        TPController = GetComponent<ThirdPersonController>();
        TPAnimation = GetComponent<ThirdPersonAnimation>();
        CurrentSpeed = WalkSpeed;
    }
    private void Update()
    {
        GroundedCheck();
        Move();
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            rb.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
        }
    }

    //Adjust speed and animations according to the player's state.
    public void ChangeSpeed()
    {
        switch (TPController.MovementState)
        {
            case "Crouch":
                CurrentSpeed = CrouchSpeed;
                TPAnimation.MovementState = 1;
                break;
            case "Crawl":
                CurrentSpeed = CrawlSpeed;
                TPAnimation.MovementState = 2;
                break;
            default://walk
                CurrentSpeed = WalkSpeed;
                TPAnimation.MovementState = 0;
                break;
        }
    }

    //Adjust the height of Collider according to the player's status
    public void SetHitBox(float center, float height)
    {
        cc.center = new Vector3(0, center, 0);
        cc.height = height;
    }

    private void Move()
    {
        if (IsGrounded && TPController.Move != null)
        {
            //for set animation according to movement
            TPAnimation.MoveVector = TPController.Move.ReadValue<Vector2>();

            Vector3 horizontal = TPController.Move.ReadValue<Vector2>().x * GetCameraRight();
            Vector3 vertical = TPController.Move.ReadValue<Vector2>().y * GetCameraForward();

            movementInput += horizontal;
            movementInput += vertical;

            movementInput = movementInput.normalized;

            if (movementInput.magnitude >= .1f)
            {
                Vector3 direction = movementInput * CurrentSpeed;
                rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);
            }
            movementInput = Vector3.zero;
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    //This method to allow the player to move left and right according to the direction of the camera.
    private Vector3 GetCameraRight()
    {
        Vector3 right = TPController.ThirdPersonCamera.right;
        right.y = 0;
        return right;
    }

    //This method to allow the player to move forward and backward according to the direction of the camera.
    private Vector3 GetCameraForward()
    {
        Vector3 forward = TPController.ThirdPersonCamera.forward;
        forward.y = 0;
        return forward;
    }
}