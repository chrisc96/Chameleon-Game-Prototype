// Wouldn't let me build with it, saying UnityEditor doesn't exist.. clearly does.

using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor (typeof (PatrollingEnemy))]
//public class FOVEditor : Editor {

//    PatrollingEnemy ai;

//    private void OnSceneGUI() {
//        ai = (PatrollingEnemy) target;
//        Handles.color = Color.red;

//        float angle_lookat = GetEnemyAngle();
//        float angle_start = angle_lookat - ai.angle/2;

//        Vector3 startAngle = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle_start)), 0, Mathf.Cos(Mathf.Deg2Rad * (angle_start)));
//        Handles.DrawSolidArc(ai.transform.position, Vector3.up, startAngle, ai.angle, ai.rayCastDist);
//    }

//    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
//        if(!angleIsGlobal) {
//            angleInDegrees += ai.transform.eulerAngles.y;
//        }
//        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
//    }

//    float GetEnemyAngle() {
//        return 90 - Mathf.Rad2Deg * Mathf.Atan2(ai.transform.forward.z, ai.transform.forward.x); // Left handed CW. z = angle 0, x = angle 90
//    }

//}
