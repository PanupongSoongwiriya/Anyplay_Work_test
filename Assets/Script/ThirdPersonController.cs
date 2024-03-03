using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    public static ThirdPersonController Instance;

    private ThirdPersonAction action;
    [HideInInspector] public InputAction move;
    public string state = "Stand";

    public Camera thirdPersonCamera;
    private ThirdPersonMovement TPMovement;

    private void Awake()
    {
        Instance = this;
        action = new ThirdPersonAction();
        TPMovement = GetComponent<ThirdPersonMovement>();
    }
    private void Update()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        action.Player.Jump.started += DoJump;
        action.Player.Crouch.started += DoCrouch;
        action.Player.Crawl.started += DoCrawl;
        move = action.Player.Move;
        action.Player.Enable();
    }

    private void OnDisable()
    {
        action.Player.Jump.started -= DoJump;
        action.Player.Crouch.started -= DoCrouch;
        action.Player.Crawl.started -= DoCrawl;
        action.Player.Disable();
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
        ThirdPersonCamera.Instance.setCameraPositionY(y);
    }
}