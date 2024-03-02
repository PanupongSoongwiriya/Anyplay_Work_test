using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private ThirdPersonAction action;
    private InputAction move;

    private CharacterController characterController;
    private Rigidbody rb;

    [SerializeField]
    private float movementForce = 1;
    [SerializeField]
    private float jumpForce = 5;
    [SerializeField]
    private float maxSpeed = 6;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private float turnSmoothTime = .1f;
    float turnSmoothVelocity;


    [SerializeField]
    private Camera playerCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        action = new ThirdPersonAction();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        action.Player.Jump.started += DoJump;
        move = action.Player.Move;
        action.Player.Enable();
    }

    private void OnDisable()
    {
        action.Player.Jump.started -= DoJump;
        action.Player.Disable();
    }

    private void Update()
    {
        Vector3 horizontal = move.ReadValue<Vector2>().x * GetCameraRight(playerCamera);
        Vector3 vertical = move.ReadValue<Vector2>().y * GetCameraForward(playerCamera);

        forceDirection += horizontal;
        forceDirection += vertical;

        forceDirection = forceDirection.normalized;

        if (forceDirection.magnitude >= .1f)
        {
            float targetAmgle = MathF.Atan2(forceDirection.x, forceDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAmgle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAmgle, 0) * Vector3.forward;

            characterController.Move(moveDir.normalized * movementForce * Time.deltaTime);
        }

        forceDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {
         /*forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;


        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0)
            rb.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        LookAt();*/
    }

    private void LookAt()
    {
        Vector3 direction = forceDirection;
        direction.y = 0;

        if (move.ReadValue<Vector2>().sqrMagnitude > .1f && direction.sqrMagnitude > .1f)
            rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraRight(Camera c)
    {
        Vector3 right = c.transform.right;
        right.y = 0;
        return right;
    }

    private Vector3 GetCameraForward(Camera c)
    {
        Vector3 forward = c.transform.forward;
        forward.y = 0;
        return forward;
    }

    private void DoJump(InputAction.CallbackContext context)
    {
        print("DoJump: " + characterController.isGrounded);
        if (characterController.isGrounded)
        {
            forceDirection += Vector3.up * jumpForce;
        }
    }

    private bool IsGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * .25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, .3f))
            return true;
        return false;
    }
}
