using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour {



    [HideInInspector]
    public bool colliding = false;
    [HideInInspector]
    // Where the view frustum clip points are when colliding
    public Vector3[] adjCameraClipPoints;
    [HideInInspector]
    // Where the view frustum clip points would be if not colliding
    public Vector3[] desCameraClipPoints;

    Camera camera;

    public void Initialise(Camera cam) {
        camera = cam;
        // 4 points on near clip plane and cam pos
        adjCameraClipPoints = new Vector3[5];
        desCameraClipPoints = new Vector3[5];
    }

    public void UpdateCamClipPoints(Vector3 camPos, Quaternion camRot, ref Vector3[] intoArray) {
        if (!camera) return;

        intoArray = new Vector3[5]; // clear array

        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView) * z;
        float y = x / camera.aspect;

        // Find clip points for each one of our four clip points on near clip plane
        intoArray[0] = (camRot * new Vector3(-x, y, z)) + camPos; // Top Left
        intoArray[1] = (camRot * new Vector3(x, y, z)) + camPos; // Top Right
        intoArray[2] = (camRot * new Vector3(-x, -y, z)) + camPos; // Bottom Left
        intoArray[3] = (camRot * new Vector3(x, -y, z)) + camPos; // Bottom Right
        intoArray[4] = camPos - camera.transform.forward;
    }

    public bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 rayFromPos) {
        for (int i = 0; i < clipPoints.Length; i++) {
            Ray ray = new Ray(rayFromPos, clipPoints[i] - rayFromPos);
            float dist = Vector3.Distance(clipPoints[i], rayFromPos);
            if (Physics.Raycast(ray, dist)) {
                return true;
            }
        }
        return false;
    }


    public float CamDistToTarget(Vector3 playerPos) {
        float dist = -1;

        // Find shortest dist between any of the clip points that are colliding
        for (int i = 0; i < desCameraClipPoints.Length; i++) {
            Ray ray = new Ray(playerPos, desCameraClipPoints[i] - playerPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (dist == -1) dist = hit.distance;
                else {
                    if (hit.distance < dist) {
                        dist = hit.distance;
                    }
                }
            }
        }

        if (dist == -1) return 0;
        else return dist;
    }
    public void CheckColliding(Vector3 targetPos) {
        // If we pass a ray from player pos to clip points and collision occured
        if (CollisionDetectedAtClipPoints(desCameraClipPoints, targetPos)) {
            colliding = true;
        } else {
            colliding = false;
        }
    }
}
