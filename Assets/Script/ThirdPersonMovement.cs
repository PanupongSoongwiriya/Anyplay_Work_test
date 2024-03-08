using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class ThirdPersonMovement : NetworkBehaviour
{
    private ThirdPersonController TPController;
    private CharacterController characterController;
    private Vector3 movementInput;

    [Header("Speed Settings")]
    [SerializeField] private float speed = 5;
    private float currentSpeed;

    [Header("Jump & Gravity Settings")]
    [SerializeField] private float gravity = -15;
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
        currentSpeed = speed;
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
            verticalVelocity = Mathf.Sqrt(JumpHeight * -2 * gravity);
        }
    }

    //Adjust the height of Collider according to the player's status
    public void SetHitBox(float center, float height)
    {
        characterController.center = new Vector3(0, center, 0);
        characterController.height = height;
    }

    private void Move()
    {
        if (TPController.Move != null)
        {
            Vector3 horizontal = TPController.Move.ReadValue<Vector2>().x * GetCameraRight();
            Vector3 vertical = TPController.Move.ReadValue<Vector2>().y * GetCameraForward();

            movementInput += horizontal;
            movementInput += vertical;

            movementInput.Normalize();

            if (movementInput.magnitude >= .1f)
            {
                Vector3 direction = movementInput * currentSpeed;
                characterController.Move(direction * Time.deltaTime);
            }
            movementInput = Vector3.zero;

            verticalVelocity += gravity * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, gravity / 1.5f);

            characterController.Move(new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
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
        Vector3 right = TPController.ThirdPersonCameraHolder.right;
        right.y = 0;
        return right;
    }

    //This method to allow the player to move forward and backward according to the direction of the camera.
    private Vector3 GetCameraForward()
    {
        Vector3 forward = TPController.ThirdPersonCameraHolder.forward;
        forward.y = 0;
        return forward;
    }
}