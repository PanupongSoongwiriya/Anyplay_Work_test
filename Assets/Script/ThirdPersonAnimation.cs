using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

//This class it connects and configures various parameters in Animator.
public class ThirdPersonAnimation : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    private Vector2 moveVector;
    private int movementState;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        moveVector *= 2;//multiply 2 for smooth transition animation in blend tree
        //set movement animation
        animator.SetFloat("X", moveVector.x, 0.2f, Time.deltaTime);
        animator.SetFloat("Y", moveVector.y, 0.2f, Time.deltaTime);
    }

    //Encapsulation

    public int MovementState
    {
        get { return movementState; }
        set {
            movementState = value;
            //Set state action stand(0), crouch(1), crawl(2)
            animator.SetInteger("movementState", movementState);
        }
    }
    public Vector2 MoveVector { get => moveVector; set => moveVector = value; }
}
