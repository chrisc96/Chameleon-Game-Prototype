using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingEnemy : MonoBehaviour {
    PlayerController playerStates; // Only used to check states of player (stealthed etc)

    public Vector3 playerLastKnownPos;

    public float angle;
    public float rayCastDist;

    bool initAlert;

    private float alertTimer;
    private float alterTimerInc = 0.3f;

    // Things to check at each way point
    bool firstTimeAtWP;
    bool lookAtTarget;

    // Wait time for each waypoint
    public bool circularList; // linear (ping pong) or circular movement
    bool forwardOnWP; // going back or forth
    float waitTimer;

    // Look Rotation
    Quaternion targetRot;

    // Waypoints
    public bool initPosAtFirstWP;
    bool goingToPos;
    public int currWPIndex;
    public List<Waypoint> waypoints = new List<Waypoint>();

    public States state;
    public enum States {
        PATROL,
        HASTARGET,
        CHASE,
        ATTACK
    }

    AIController enemyControl;
    NavMeshAgent agent;

    private void Start() {
        if(waypoints.Count > 0) transform.position = waypoints[currWPIndex].targetDest.position;
        playerStates = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        enemyControl = GetComponent<AIController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        switch(state) {
            case States.PATROL:
                DropAlerts();
                TargetAvailable();
                PatrolBehaviour();
                break;
            case States.HASTARGET:
                HasTargetBehaviour();
                break;
            case States.ATTACK:
                AttackBehaviour();
                break;
        }
    }

    private void DropAlerts() {
        if (enemyControl.alertLevel > 0) {
            alertTimer += Time.deltaTime;

            if (alertTimer > alterTimerInc * 0.5) {
                enemyControl.alertLevel--;
                alertTimer = 0;
            }
        }
    }

    void TargetAvailable() {
        if (seePlayerRaycast()) {
            ChangeAIBehaviour("AI_State_Has_Target", 0);
        }
    }

    private bool seePlayerRaycast() {
        bool retVal = false;
        RaycastHit lowerBodyRay;

        Vector3 rayStart = transform.position + new Vector3(0, 1.0f, 0);
        Vector3 dir = playerStates.gameObject.transform.position - rayStart;

        if (Physics.Raycast(rayStart, dir, out lowerBodyRay, rayCastDist)) {
            if(lowerBodyRay.transform.GetComponent<PlayerController>()) {
                if(lowerBodyRay.transform.GetComponent<PlayerController>()) {
                    // If we see the player, update the last known position so if we lose him
                    // we can chase him to that place
                    playerLastKnownPos = lowerBodyRay.transform.position;
                    retVal = true;
                }
            }
        }

        // If the angle to the player from this AI enemy isn't within the FOV or
        // The player is stealthed, then we cannot see the player

        Vector3 dir2 = playerStates.gameObject.transform.position - transform.position;
        Vector3 localForward = transform.parent.InverseTransformDirection(transform.forward);
        float angleToPlayer = Vector3.Angle(localForward, dir2);
        if (angle / 2 < angleToPlayer) retVal = false;
        // Debug.Log("angle to player (local): " + angleToPlayer + "   angle/2: " + angle / 2 + "   can see?: " + retVal);

        if (playerStates.stealthed) retVal = false;
        return retVal;
    }

    private void PatrolBehaviour() {
        if (waypoints.Count > 0) {
            Waypoint currWP = waypoints[currWPIndex];
            if (!goingToPos) {
                enemyControl.moveToPosition(currWP.targetDest.position);
                goingToPos = true;
            }
            else {
                float distToTarget = Vector3.Distance(transform.position, currWP.targetDest.position);
                if (distToTarget < enemyControl.stoppingDist) {
                    ActionsAtWaypoint(currWP);
                }
            }
        }
    }

    private void AttackBehaviour() {
        if (seePlayerRaycast()) {
            LookAtTarget(playerLastKnownPos);
            playerStates.dead = true;
        }
        else {
            ChangeAIBehaviour("AI_State_Normal", 0);
        }
    }

    private void HasTargetBehaviour() {
        if (seePlayerRaycast()) {
            enemyControl.stopMoving();
            LookAtTarget(playerLastKnownPos);
            alertTimer += Time.deltaTime;

            if(alertTimer > alterTimerInc) {
                enemyControl.alertLevel++;
                alertTimer = 0;
            }
            if (enemyControl.alertLevel > 2) {
                ChangeAIBehaviour("AI_State_Attack", 0);
                playerStates.dead = true;
            }
        }
        else {
            ChangeAIBehaviour("AI_State_Normal", 0);
        }
    }

    private void LookAtTarget(Vector3 playerLastKnownPos) {
        Vector3 dir = playerLastKnownPos - transform.position;
        dir.y = 0;

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > 0.1f) {
            targetRot = Quaternion.LookRotation(dir);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 3);
        }
    }

    public void ChangeAIBehaviour(string behaviour, float delay) {
        Invoke(behaviour, delay);
    }

    void AI_State_Normal() {
        state = States.PATROL;
        goingToPos = false;
        firstTimeAtWP = false;
        enemyControl.changeToPatrolling();
    }

    void AI_State_Has_Target() {
        state = States.HASTARGET;
        goingToPos = false;
        firstTimeAtWP = false;
        enemyControl.changeToChasing();
    }

    void AI_State_Attack() {
        state = States.ATTACK;
        enemyControl.changeToShooting();
    }

    private void ActionsAtWaypoint(Waypoint curr) {
        if (firstTimeAtWP) {
            firstTimeAtWP = false;
            lookAtTarget = curr.lookTowards;
        }
        waitTimer += Time.deltaTime;
        if (waitTimer > curr.waitTime) {
            if (circularList) {
                currWPIndex = (currWPIndex + 1) % waypoints.Count;
            }
            else {
                // Going forward
                if (forwardOnWP) {
                    // At end of the list from start to finish, we need to go backwards now
                    if (currWPIndex == waypoints.Count - 1) {
                        forwardOnWP = false;
                        currWPIndex--;
                    }
                    else {
                        // Go forward
                        currWPIndex++;
                    }
                }
                // Coming back
                else {
                    // At the very first waypoint, need to go forward again
                    if (currWPIndex == 0) {
                        forwardOnWP = true;
                        currWPIndex++;
                    }
                    else {
                        // Coming back
                        currWPIndex--;
                    }
                }
            }
            firstTimeAtWP = true;
            goingToPos = false;
            waitTimer = 0;
        }

        if (lookAtTarget) {
            Vector3 dir = curr.targetToLookTo.position - transform.position;
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            // Look at WP if we're not already looking at it
            if (angle > 0.1f) {
                targetRot = Quaternion.LookRotation(dir);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * curr.speedToLook);
            }
            // We must be looking at it, so don't look at target anymore
            else {
                lookAtTarget = false;
            }
        }
    }
}

[System.Serializable]
public struct Waypoint {
    public Transform targetDest;
    public float waitTime;
    public bool lookTowards; // If the AI wants to wait here and look around
    public Transform targetToLookTo;
    public float speedToLook;
}