using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    private ThirdPersonController TPController;

    [SerializeField] private Transform cameraPole;

    [Header("Settings")]
    [SerializeField] private float cameraSensitivity = 5;
    [SerializeField] private float moveInputDeadZone;

    [Header("Third person camera settings")]
    [SerializeField] private LayerMask cameraObstacleLayers;
    private Vector3 offsetCamera;
    private float maxCameraDistance;

    // Camera control
    private Vector2 oldPos;
    private float cameraPitch;

    private Vector3 targetPos, currentVelocity;
    private bool newTarget;
    private void Start()
    {
        TPController = GetComponent<ThirdPersonController>();

        offsetCamera = TPController.thirdPersonCamera.transform.localPosition;

        // Set max camera distance to the distance the camera is from the player in the editor
        maxCameraDistance = TPController.thirdPersonCamera.transform.localPosition.z;

        // calculate the movement input dead zone
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);

        // Get the initial angle for the camera pole
        cameraPitch = cameraPole.localRotation.eulerAngles.x;
    }

    private void Update()
    {
        SmoothUpDown();
    }

    private void SmoothUpDown()
    {
        if (newTarget)
        {
            cameraPole.transform.localPosition = Vector3.SmoothDamp(cameraPole.transform.localPosition, targetPos, ref currentVelocity, 0.1f);
            cameraPole.transform.localPosition = new Vector3(0, cameraPole.transform.localPosition.y, 0);
            float dist = Vector3.Distance(cameraPole.transform.localPosition, targetPos);
            if (Mathf.Abs(dist) < 0.01f)
            {
                newTarget = false;
            }
        }
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    public void LookAround(Vector2 newPos)
    {
        Vector3 differencePos = (newPos - oldPos).normalized;
        cameraPitch = Mathf.Clamp(cameraPitch - differencePos.y * (cameraSensitivity / 2), -45f, 35f);
        cameraPole.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, differencePos.x * cameraSensitivity);
        oldPos = newPos;
    }

    public void setCameraPositionY(float y)
    {
        Vector3 newPos = cameraPole.transform.localPosition;
        newPos.y = y;
        targetPos = newPos;
        newTarget = true;
    }

    private void MoveCamera()
    {
        Transform TPCameraTrans = TPController.thirdPersonCamera.transform;
        Vector3 startVector = cameraPole.position;
        startVector.y += 1;
        Vector3 rayDir = TPCameraTrans.position - startVector;

        Debug.DrawRay(startVector, rayDir, Color.red);
        // Check if the camera would be colliding with any obstacle
        if (Physics.Raycast(startVector, rayDir, out RaycastHit hit, Mathf.Abs(maxCameraDistance), cameraObstacleLayers))
        {
            // Move the camera to the impact point
            Vector3 newCameraPos = hit.point;
            newCameraPos.y = offsetCamera.y;

            TPCameraTrans.position = Vector3.SmoothDamp(TPCameraTrans.position, newCameraPos, ref currentVelocity, 0.1f);
        }
        else
        {
            // Move the camera to the max distance on the local z axis
            TPCameraTrans.localPosition = Vector3.SmoothDamp(TPCameraTrans.localPosition, offsetCamera, ref currentVelocity, 0.1f);
        }
    }
}