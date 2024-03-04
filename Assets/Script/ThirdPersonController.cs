using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Connection;
using FishNet.Object;

public class ThirdPersonController : NetworkBehaviour
{
    private ThirdPersonAction action;
    [HideInInspector]
    public InputAction move;
    public string state = "Stand";

    public Camera thirdPersonCamera;
    private ThirdPersonMovement TPMovement;
    private ThirdPersonCamera TPCamera;
    public GameObject UIController;

    public override void OnStartClient()
    {
        base.OnStartClient();
        print(name);
        if (base.IsOwner)
        {
            print("IsOwner");
            action = new ThirdPersonAction();
            TPMovement = GetComponent<ThirdPersonMovement>();
            TPCamera = GetComponent<ThirdPersonCamera>();

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

    private void Awake()
    {
        /*Instance = this;
        action = new ThirdPersonAction();
        TPMovement = GetComponent<ThirdPersonMovement>();*/
    }
    private void Update()
    {
    }
    private void OnEnable()
    {
        if (base.IsOwner)
        {
            print("OnEnable");
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
            print("OnDisable");
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
        print("DoCrawl");
        SetState("Crawl", 0.18f, 0.18f, -0.9f);
    }

    private void DoCrouch(InputAction.CallbackContext context)
    {
        print("DoCrouch");
        SetState("Crouch", 0.45f, 0.9f, -0.5f);
    }
    private void SetState(string s, float center, float height, float y)
    {
        if (state == s)
        {
            state = "Stand";
            center = 0.9f;
            height = 1.8f;
            y = 0;
        }
        else { state = s; }

        TPMovement.ChangeSpeed();
        TPMovement.setHitBox(center, height);
        TPCamera.setCameraPositionY(y);
    }
}