using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public StealthAgainstStruct.Colors col;

    [System.Serializable]
    public class MoveSettings {
        public float forwardVel = 12;
        public float jumpVel = 25;
        public float rotateVel = 150;
        public float distToGround;
        public float inputDelay = 0.1f;
        public LayerMask ground;
    }

    [System.Serializable]
    public class PhysSettings {
        public float gravity = 0.75f;
    }

    [System.Serializable]
    public class InputSettings {
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    public MoveSettings moveSettings = new MoveSettings();
    public PhysSettings physSettings = new PhysSettings();
    public InputSettings inputSettings = new InputSettings();

    Vector3 velocity = Vector3.zero;
    Quaternion targetRotation;
    Rigidbody RB;

    // Teleporting / State handling
    public List<Level> levels = new List<Level>();
    public int currLevel = 0;

    // States
    public bool stealthed;

    public bool dead;
    public bool won;

    float forwardInput, turnInput, jumpInput;

    private void Start() {
        gameObject.transform.position = levels[currLevel].getSpawnCheckpoint().teleport.transform.position;
        moveSettings.distToGround = transform.localScale.y / 2 + 0.1f;
        targetRotation = transform.rotation;
        RB = GetComponent<Rigidbody>();
        forwardInput = turnInput = jumpInput = 0;
    }

    private void Update() {
        if (!dead || !won) {
            GetInput();
        }
    }

    private void FixedUpdate() {
        if (!dead || !won) {
            Turn();
            RB.velocity = transform.TransformDirection(velocity);

            Run();
            Jump();
        }
    }

    void Run() {
        if (Mathf.Abs(forwardInput) > moveSettings.inputDelay) {
            velocity.z = moveSettings.forwardVel * forwardInput;
        } else {
            velocity.z = 0;
        }
    }

    void Turn() {
        if (Mathf.Abs(turnInput) > moveSettings.inputDelay) {
            targetRotation *= Quaternion.AngleAxis(moveSettings.rotateVel * turnInput * Time.smoothDeltaTime, Vector3.up);
        }
        transform.rotation = targetRotation;
    }

    void Jump() {
        if (jumpInput > 0 && isGrounded()) {
            // jumping
            velocity.y = moveSettings.jumpVel;
        } 
        else if (jumpInput == 0 && isGrounded()) {
            // onGround after jump
            velocity.y = 0;
        } 
        else {
            // Falling
            velocity.y -= physSettings.gravity;
        }
    }

    public void moveToNextLevel() {
        // If there are no more levels to play
        if (currLevel + 1 > levels.Count - 1) {
            won = true;
        }
        // If there is another spawn point, there must be another level to play
        else {
            currLevel = currLevel+1;
            Vector3 posToPutPlayer = levels[currLevel].getSpawnCheckpoint().teleport.transform.position;
            posToPutPlayer.y += 2.6f;
            gameObject.transform.position = posToPutPlayer;
        }
    }

    public void moveToNextNode(TeleportPad nextCheckpoint) {
        Vector3 posToPutPlayer = nextCheckpoint.transform.position;
        posToPutPlayer.y += 2.6f;
        gameObject.transform.position = posToPutPlayer;
    }

    bool isGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, moveSettings.distToGround, moveSettings.ground);
    }

    private void GetInput() {
        forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS);
        turnInput = Input.GetAxis(inputSettings.TURN_AXIS);
        jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS); // Non interpolated. -1, 0, 1 returned
    }

    public void stopMoving() {
        forwardInput = 0;
        turnInput = 0;
        jumpInput = 0;
        RB.velocity = Vector3.zero;
    }

    public Quaternion TargetRotation {
        get { return targetRotation; }
    }

}

[System.Serializable]
public class Level {
    public List<Checkpoint> checkpoints;

    public Checkpoint getSpawnCheckpoint() {
        foreach (Checkpoint checkPt in checkpoints) {
            if (checkPt.teleport.isSpawn) {
                return checkPt;
            }
        }
        Debug.LogError("You probably don't have any spawn checkpoints for this level...");
        return null;
    }
}

[System.Serializable]
public class Checkpoint {
    public TeleportPad teleport;
}