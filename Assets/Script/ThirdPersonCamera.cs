using System.Collections;
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
    private float maxRotationX = 60;
    private float minRotationX = -45;
    //x = min of range & y = max of range
    private Vector2 cameraZRange = new Vector2(0, 0.36f);
    private Vector2 poleZRange = new Vector2(-0.18f, 0.21f);

    // For LookAround method
    private Vector2 oldPos;
    private float cameraPitch;

    //For SmoothUpDown method
    private Vector3 targetPos, currentVelocity;
    private bool newTarget;
    public Vector3 differencePos;
    public Transform model;

    private void Start()
    {
        TPController = GetComponent<ThirdPersonController>();

        offsetCamera = TPController.ThirdPersonCameraHolder.localPosition;

        // Set max camera distance to the distance the camera is from the player in the editor
        maxCameraDistance = TPController.ThirdPersonCameraHolder.localPosition.z;

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
        differencePos = (newPos - oldPos).normalized;
        cameraPitch = Mathf.Clamp(cameraPitch - differencePos.y * (cameraSensitivity / 2), minRotationX, maxRotationX);
        cameraPole.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        oldPos = newPos;

        SetCameraZ();

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, differencePos.x * cameraSensitivity);
    }

    //This method will move the camera up or down according to the player's height at that time.
    public void SettingCamera(float y, float maxX, float minX, Vector2 poleZ, Vector2 cameraZ)
    {
        //Set camera property
        Vector3 newPos = cameraPole.transform.localPosition;
        newPos.y = y;
        maxRotationX = maxX;
        minRotationX = minX;
        poleZRange = poleZ;
        cameraZRange = cameraZ;

        //Set rotation cameraPole
        cameraPitch = Mathf.Clamp(cameraPitch, minRotationX, maxRotationX);
        cameraPole.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        
        SetCameraZ();

        //For SmoothUpDown method active
        targetPos = newPos;
        newTarget = true;
    }

    //Set Z axis according to the appropriateness of looking down and looking up
    private void SetCameraZ()
    {
        Vector3 newZ = cameraPole.localPosition;
        newZ.z = Mathf.Lerp(poleZRange.x, poleZRange.y, (cameraPitch - minRotationX) / (maxRotationX - minRotationX));
        cameraPole.localPosition = newZ;

        Transform TPCameraTrans = TPController.ThirdPersonCameraHolder.GetChild(0);
        newZ = TPCameraTrans.localPosition;
        newZ.z = Mathf.Lerp(cameraZRange.x, cameraZRange.y, (cameraPitch - minRotationX) / (maxRotationX - minRotationX));
        TPCameraTrans.localPosition = newZ;
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
            if (Mathf.Abs(dist) < 0.05f)
            {
                cameraPole.transform.localPosition = targetPos;
                newTarget = false;
            }
        }
    }

    //This method will move the camera in or out as appropriate.
    private void MoveCamera()
    {
        Transform TPCameraHolderTrans = TPController.ThirdPersonCameraHolder;
        Vector3 startVector = cameraPole.position;
        startVector.y -= 0.15f;
        startVector.x += 0.246f;
        Vector3 rayDir = TPCameraHolderTrans.position - startVector;

        Debug.DrawRay(startVector, rayDir, Color.red);

        // Check if the camera would be colliding with any obstacle
        if (Physics.Raycast(startVector, rayDir, out RaycastHit hit, Mathf.Abs(maxCameraDistance), cameraObstacleLayers))
        {
            // Move the camera to the impact point
            Vector3 newCameraPos = hit.point;
            TPCameraHolderTrans.position = Vector3.SmoothDamp(TPCameraHolderTrans.position, newCameraPos, ref currentVelocity, 0.1f);
        }
        else
        {
            // Move the camera to offsetCamera
            TPCameraHolderTrans.localPosition = Vector3.SmoothDamp(TPCameraHolderTrans.localPosition, offsetCamera, ref currentVelocity, 0.1f);
        }
    }
}