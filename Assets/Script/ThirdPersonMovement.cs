using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class ThirdPersonMovement : NetworkBehaviour
{
    private ThirdPersonController TPController;

    private Rigidbody rb;
    private CapsuleCollider cc;

    [Header("Speed Settings")]
    [SerializeField] private float CurrentSpeed;
    [SerializeField] private float RunSpeed = 4;
    [SerializeField] private float WalkSpeed = 2;
    [SerializeField] private float CrouchSpeed = 1.25f;
    [SerializeField] private float CrawlSpeed = 0.75f;
    private Vector3 movementInput;

    [Header("Jump Settings")]
    [SerializeField] private float JumpHeight = 1.2f;

    [Header("Grounded Settings")]
    [SerializeField] private float GroundedOffset = -0.14f;
    [SerializeField] private float GroundedRadius = 0.28f;
    [SerializeField] private LayerMask GroundLayers;
    [SerializeField] private bool IsGrounded;

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

    public void ChangeSpeed()
    {
        switch (TPController.state)
        {
            case "Run":
                CurrentSpeed = RunSpeed;
                break;
            case "Crouch":
                CurrentSpeed = CrouchSpeed;
                break;
            case "Crawl":
                CurrentSpeed = CrawlSpeed;
                break;
            default:
                CurrentSpeed = WalkSpeed;
                break;
        }
    }

    public void setHitBox(float center, float height)
    {
        cc.center = new Vector3(0, center, 0);
        cc.height = height;
    }

    private void Move()
    {
        if (IsGrounded)
        {
            Vector3 horizontal = TPController.move.ReadValue<Vector2>().x * GetCameraRight();
            Vector3 vertical = TPController.move.ReadValue<Vector2>().y * GetCameraForward();

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

    private Vector3 GetCameraRight()
    {
        Vector3 right = TPController.thirdPersonCamera.transform.right;
        right.y = 0;
        return right;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = TPController.thirdPersonCamera.transform.forward;
        forward.y = 0;
        return forward;
    }
}