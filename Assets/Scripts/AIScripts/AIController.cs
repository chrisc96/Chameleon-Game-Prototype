using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    Animator anim;
    NavMeshAgent agent;

    public float stoppingDist;
    public bool movingToPos;
    public Vector3 destPos;

    public bool patrolling;
    public bool chasing;

    // To implement/use later
    public bool shooting;

    public float attackSpeed;
    public float walkSpeed;
    public float chaseSpeed;

    public int alertLevel;

    private void Start() {
        anim = GetComponent<Animator>();
        setupAnimator();

        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 500;
        agent.autoBraking = false;
        agent.stoppingDistance = stoppingDist;
        agent.updateRotation = true;
    }

    private void Update() {
        if(movingToPos) {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(destPos);

            float distToTarget = Vector3.Distance(transform.position, destPos);
            
            if (distToTarget <= stoppingDist) {
                stopMoving();
            }
        } 
        else {
            agent.isStopped = true;
            agent.updateRotation = false;
        }
        HandleSpeed();
    }

    private void HandleSpeed() {
        if (patrolling) {
            agent.speed = walkSpeed;
        }
        else if (chasing) {
            agent.speed = chaseSpeed;
        }
        else if (shooting) {
            agent.speed = attackSpeed;
        }
    }

    public void CallFunctionWithString(string functionName, float delay) {
        Invoke(functionName, delay);
    }

    public void changeToPatrolling() {
        patrolling = true; 
        chasing = false;
        movingToPos = false;
        shooting = false;
    }

    public void changeToChasing() {
        patrolling = false;
        chasing = true;
        shooting = false;
        movingToPos = false;
    }

    public void changeToShooting() {
        shooting = true;
        chasing = false;
        patrolling = false;
        movingToPos = false;
    }

    private void setupAnimator() {
        anim = GetComponent<Animator>();
        // Do animation stuff
    }

    public void stopMoving() {
        movingToPos = false;
    }

    public void moveToPosition(Vector3 pos) {
        movingToPos = true;
        destPos = pos;
    }
}
