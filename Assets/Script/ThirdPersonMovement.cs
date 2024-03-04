using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class ThirdPersonMovement : NetworkBehaviour
{
    private ThirdPersonController TPController;

    private CharacterController characterController;

    [Header("Speed Settings")]
    [SerializeField] private float CurrentSpeed;
    [SerializeField] private float RunSpeed = 4;
    [SerializeField] private float WalkSpeed = 2;
    [SerializeField] private float CrouchSpeed = 1.25f;
    [SerializeField] private float CrawlSpeed = 0.75f;
    private Vector3 targetDirection;

    [Header("Jump & Gravity Settings")]
    [SerializeField] private float Gravity = -15;
    [SerializeField] private float JumpHeight = 1.2f;
    private float verticalVelocity;

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
        characterController = GetComponent<CharacterController>();
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
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2 * Gravity);
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
        characterController.center = new Vector3(0, center, 0);
        characterController.height = height;
    }

    private void Move()
    {
        Vector3 horizontal = TPController.move.ReadValue<Vector2>().x * GetCameraRight();
        Vector3 vertical = TPController.move.ReadValue<Vector2>().y * GetCameraForward();

        targetDirection += horizontal;
        targetDirection += vertical;

        targetDirection = targetDirection.normalized;

        if (targetDirection.magnitude >= .1f)
        {
            characterController.Move(targetDirection * CurrentSpeed * Time.deltaTime);
        }
        targetDirection = Vector3.zero;

        verticalVelocity += Gravity * Time.deltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, Gravity / 1.5f);

        characterController.Move(new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
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