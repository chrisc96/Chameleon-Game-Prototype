using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    CameraCollisionHandler collisionHandler;
    public Transform lookAt;

    [System.Serializable]
    public class PositionSettings {
        // Relative to target position (player), where do we want the cam to look at

        public Vector3 targetPosOffset = new Vector3(0.0f, 4.51f, 0);
        public float lookSmooth = 50;

        public float distanceFromTarget = -25; // When we zoom, this changes
        public float zoomSmooth = 500;
        public float maxZoom = -4;
        public float minZoom = -20;

        public bool smoothFollow = true;
        public float smooth = 0.1f;

        [HideInInspector]
        public float cameraCushion = 0.6f;
        public float adjustmentDist = 500;
    }

    [System.Serializable]
    public class RotationSettings {
        public float xRotation = -50;
        public float yRotation = -180;
    }

    [System.Serializable]
    public class InputSettings {
        public string ZOOM = "Mouse ScrollWheel";
    }

    [System.Serializable]
    public class DebugSettings {
        public bool drawDesCollisionLines = false;
        public bool drawAdjCollisionLines = false;
    }

    public RotationSettings rotationSettings = new RotationSettings();
    public PositionSettings positionSettings = new PositionSettings();
    public InputSettings inputSettings = new InputSettings();
    public DebugSettings debugSettings = new DebugSettings();

    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero; // Used when not colliding

    Vector3 adjDestination = Vector3.zero; // Used when colliding
    Vector3 camVel = Vector3.zero; // Camera's velocity for smoothing

    float zoomInput;
    
    private void Start() {
        collisionHandler = GetComponent<CameraCollisionHandler>();

        SetCameraTarget(lookAt);
        MoveToTarget();

        collisionHandler.Initialise(Camera.main);
        collisionHandler.UpdateCamClipPoints(transform.position, transform.rotation, ref collisionHandler.adjCameraClipPoints);
        collisionHandler.UpdateCamClipPoints(destination, transform.rotation, ref collisionHandler.desCameraClipPoints);
    }

    private void Update() {
        getInput();
        ZoomInOnTarget();
    }

    private void FixedUpdate() {
        LookAtTarget();
        MoveToTarget();

        collisionHandler.UpdateCamClipPoints(transform.position, transform.rotation, ref collisionHandler.adjCameraClipPoints);
        collisionHandler.UpdateCamClipPoints(destination, transform.rotation, ref collisionHandler.desCameraClipPoints);

        collisionHandler.CheckColliding(targetPos);
        positionSettings.adjustmentDist = collisionHandler.CamDistToTarget(targetPos);
    }

    void MoveToTarget() {
        targetPos = lookAt.position + Vector3.up * positionSettings.targetPosOffset.y +
                                Vector3.forward * positionSettings.targetPosOffset.z +
                                transform.TransformDirection(Vector3.right * positionSettings.targetPosOffset.x);
        destination = Quaternion.Euler(rotationSettings.xRotation, rotationSettings.yRotation + lookAt.eulerAngles.y, 0.0f) * -Vector3.forward * positionSettings.distanceFromTarget * 1.5f;
        destination += targetPos;

        if (collisionHandler.colliding) {
            adjDestination = Quaternion.Euler(rotationSettings.xRotation, rotationSettings.yRotation + lookAt.eulerAngles.y, 0.0f) * ((Vector3.forward * positionSettings.adjustmentDist * positionSettings.cameraCushion));
            adjDestination += targetPos;

            if (positionSettings.smoothFollow) {
                transform.position = Vector3.SmoothDamp(transform.position, adjDestination, ref camVel, positionSettings.smooth);
            } 
            else {
                transform.position = adjDestination;
            }
        }
        else {
            if (positionSettings.smoothFollow) {
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, positionSettings.smooth);
            } 
            else {
                transform.position = destination;
            }
        }
    }

    void LookAtTarget() {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 - Mathf.Exp(-positionSettings.lookSmooth * Time.deltaTime));
    }

    void ZoomInOnTarget() {
        positionSettings.distanceFromTarget += zoomInput * positionSettings.zoomSmooth * Time.deltaTime;
        positionSettings.distanceFromTarget = Mathf.Clamp(positionSettings.distanceFromTarget, positionSettings.minZoom, positionSettings.maxZoom);
    }

    void getInput() {
        zoomInput = Input.GetAxisRaw(inputSettings.ZOOM);
    }

    // Use if you want to change target of the camera
    public void SetCameraTarget(Transform t) {
        lookAt = t;
    }

    public void debug() {
        for (int i = 0; i < collisionHandler.desCameraClipPoints.Length; i++) {
            if (debugSettings.drawDesCollisionLines) {
                Debug.DrawLine(targetPos, collisionHandler.desCameraClipPoints[i], Color.red);
            }
            if (debugSettings.drawAdjCollisionLines) {
                Debug.DrawLine(targetPos, collisionHandler.adjCameraClipPoints[i], Color.green);
            }
        }
    }
}
