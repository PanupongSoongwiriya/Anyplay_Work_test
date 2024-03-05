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
    private string movementState = "Stand";

    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private GameObject uIController;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            action = new ThirdPersonAction();
            TPMovement = GetComponent<ThirdPersonMovement>();
            TPCamera = GetComponent<ThirdPersonCamera>();

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
            thirdPersonCamera.gameObject.SetActive(false);
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

    private void DoJump(InputAction.CallbackContext context)
    {
        TPMovement.Jump();
    }

    private void DoCrawl(InputAction.CallbackContext context)
    {
        SetPlayerState("Crawl", 0.18f, 0.18f, -1.2f);
    }

    private void DoCrouch(InputAction.CallbackContext context)
    {
        SetPlayerState("Crouch", 0.45f, 0.9f, -0.6f);
    }

    //This method will adjust various settings in the player to suit that state.
    private void SetPlayerState(string s, float center, float height, float y)
    {
        //If the player presses the same button, it will return to its stand state.
        if (movementState == s)
        {
            movementState = "Stand";
            center = 0.9f;
            height = 1.8f;
            y = 0;
        }
        else { movementState = s; }

        TPMovement.ChangeSpeed();
        TPMovement.SetHitBox(center, height);
        TPCamera.SetCameraPositionY(y);
    }

    //Encapsulation
    public InputAction Move { get => move; set => move = value; }
    public string MovementState { get => movementState; set => movementState = value; }
    public Camera ThirdPersonCamera { get => thirdPersonCamera; set => thirdPersonCamera = value; }
    public GameObject UIController { get => uIController; set => uIController = value; }
}