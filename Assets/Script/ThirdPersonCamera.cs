using UnityEngine;

//This class for control camera 
public class ThirdPersonCamera : MonoBehaviour
{
    private ThirdPersonController TPController;

    [SerializeField] private Transform cameraPole;

    [Header("Settings")]
    [SerializeField] private float cameraSensitivity = 5;
    [SerializeField] private LayerMask cameraObstacleLayers;
    private Vector3 offsetCamera;
    private float maxCameraDistance;

    // For LookAround method
    private Vector2 oldPos;
    private float cameraPitch;

    //For SmoothUpDown method
    private Vector3 targetPos, currentVelocity;
    private bool newTarget;

    private void Start()
    {
        TPController = GetComponent<ThirdPersonController>();

        offsetCamera = TPController.ThirdPersonCamera.localPosition;

        // Set max camera distance to the distance the camera is from the player in the editor
        maxCameraDistance = TPController.ThirdPersonCamera.localPosition.z;

        // Get the initial angle for the camera pole
        cameraPitch = cameraPole.localRotation.eulerAngles.x;
    }

    private void Update()
    {
        SmoothUpDown();
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    //This method will move the camera and character(yaw) in the direction the player swipes on the screen.
    public void LookAround(Vector2 newPos)
    {
        Vector3 differencePos = (newPos - oldPos).normalized;
        cameraPitch = Mathf.Clamp(cameraPitch - differencePos.y * (cameraSensitivity / 2), -45f, 35f);
        cameraPole.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        oldPos = newPos;

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, differencePos.x * cameraSensitivity);
    }

    //This method will move the camera up or down according to the player's height at that time.
    public void SetCameraPositionY(float y)
    {
        Vector3 newPos = cameraPole.transform.localPosition;
        newPos.y = y;

        //For SmoothUpDown method active
        targetPos = newPos;
        newTarget = true;
    }

    //This method will makes moving up and down the camera smoother.
    private void SmoothUpDown()
    {
        if (newTarget)
        {
            cameraPole.transform.localPosition = Vector3.SmoothDamp(cameraPole.transform.localPosition, targetPos, ref currentVelocity, 0.1f);
            //Lock x axis and z axis
            cameraPole.transform.localPosition = new Vector3(0, cameraPole.transform.localPosition.y, 0);

            float dist = Vector3.Distance(cameraPole.transform.localPosition, targetPos);
            if (Mathf.Abs(dist) < 0.01f)
            {
                newTarget = false;
            }
        }
    }

    //This method will move the camera in or out as appropriate.
    private void MoveCamera()
    {
        Transform TPCameraTrans = TPController.ThirdPersonCamera;
        Vector3 startVector = cameraPole.position;
        startVector.y += 1.2f;
        Vector3 rayDir = TPCameraTrans.position - startVector;

        Debug.DrawRay(startVector, rayDir, Color.red);

        // Check if the camera would be colliding with any obstacle
        if (Physics.Raycast(startVector, rayDir, out RaycastHit hit, Mathf.Abs(maxCameraDistance), cameraObstacleLayers))
        {
            // Move the camera to the impact point
            Vector3 newCameraPos = hit.point;

            TPCameraTrans.position = Vector3.SmoothDamp(TPCameraTrans.position, newCameraPos, ref currentVelocity, 0.1f);


            //Prevent the camera from sink in the ground
            Vector3 newTarget = TPCameraTrans.localPosition;
            newTarget.y = Mathf.Max(newTarget.y, 1);
            TPCameraTrans.localPosition = Vector3.SmoothDamp(TPCameraTrans.localPosition, newTarget, ref currentVelocity, 0.1f);
        }
        else
        {
            // Move the camera to offsetCamera
            TPCameraTrans.localPosition = Vector3.SmoothDamp(TPCameraTrans.localPosition, offsetCamera, ref currentVelocity, 0.1f);
        }
    }
}