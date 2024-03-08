using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Connection;
using FishNet.Object;

//This class will control work in various parts of the thirdperson system
public class ThirdPersonController : NetworkBehaviour
{
    //InputAction
    private ThirdPersonAction action;
    private InputAction move;

    private ThirdPersonMovement TPMovement;
    private ThirdPersonCamera TPCamera;
    private ThirdPersonAnimation TPAnimation;
    private string movementState = "Stand";

    [SerializeField] private Transform thirdPersonCameraHolder;
    [SerializeField] private GameObject uIController;
    [SerializeField] private float forceMagnitude = 0.5f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            action = new ThirdPersonAction();
            TPMovement = GetComponent<ThirdPersonMovement>();
            TPCamera = GetComponent<ThirdPersonCamera>();
            TPAnimation = GetComponent<ThirdPersonAnimation>();

            //InputAction setting when connected
            action.Player.Jump.started += DoJump;
            action.Player.Crouch.started += DoCrouch;
            action.Player.Crawl.started += DoCrawl;
            move = action.Player.Move;
            action.Player.Enable();
        }
        else
        {
            enabled = false;
            UIController.SetActive(false);
            thirdPersonCameraHolder.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (base.IsOwner)
        {
            //InputAction setting on enable
            action.Player.Jump.started += DoJump;
            action.Player.Crouch.started += DoCrouch;
            action.Player.Crawl.started += DoCrawl;
            move = action.Player.Move;
            action.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (base.IsOwner)
        {
            //InputAction setting on disable
            action.Player.Jump.started -= DoJump;
            action.Player.Crouch.started -= DoCrouch;
            action.Player.Crawl.started -= DoCrawl;
            action.Player.Disable();
        }
    }

    private void Update()
    {
        if (move != null)
        {
            //for set animation according to movement
            TPAnimation.MoveVector = Move.ReadValue<Vector2>();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Add force to another object if that object has Rigidbody.
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null)
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        }
    }


    private void DoJump(InputAction.CallbackContext context)
    {
        TPMovement.Jump();
    }

    private void DoCrawl(InputAction.CallbackContext context)
    {
        SetPlayerState("Crawl");
    }

    private void DoCrouch(InputAction.CallbackContext context)
    {
        SetPlayerState("Crouch");
    }

    //This method will adjust various settings in the player to suit that state.
    private void SetPlayerState(string s)
    {
        float center = 0.9f;
        float height = 1.8f;
        float y = 1.8f;
        float maxX = 60;
        float minX = -45;
        Vector2 poleZ = new Vector2(-0.18f, 0.21f);
        Vector2 cameraZ = new Vector2(0, 0.36f);

        //If the player presses the same button, it will return to its stand state.
        if (movementState == s) { movementState = "Stand"; }
        else { movementState = s; }

        switch (MovementState)
        {
            case "Crouch":
                TPAnimation.MovementState = 1;
                center /= 2;
                height /= 2;
                y -= 0.6f;
                maxX = 40;
                minX = -25;
                poleZ = Vector2.zero;
                cameraZ = new Vector2(0.36f, 0);
                break;
            case "Crawl":
                TPAnimation.MovementState = 2;
                center /= 10;
                height /= 10;
                y -= 1.2f;
                maxX = 5;
                minX = -15;
                poleZ = Vector2.zero;
                cameraZ = new Vector2(1, 0.7f);
                break;
            default://walk
                TPAnimation.MovementState = 0;
                break;
        }

        TPMovement.SetHitBox(center, height);
        TPCamera.SettingCamera(y, maxX, minX, poleZ, cameraZ);
    }

    //Encapsulation
    public InputAction Move { get => move; set => move = value; }
    public string MovementState { get => movementState; set => movementState = value; }
    public Transform ThirdPersonCameraHolder { get => thirdPersonCameraHolder; set => thirdPersonCameraHolder = value; }
    public GameObject UIController { get => uIController; set => uIController = value; }
}