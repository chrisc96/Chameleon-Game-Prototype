using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPad : MonoBehaviour {

    public TeleportPad nextIntermediateCheckpoint; // For intermediate
    // Following three are mutually exclusive, only one should be true at a time
    public bool isIntermediate;
    public bool isEndofLevel;
    public bool isSpawn;

    protected CameraFollow cf;
    protected PlayerController player;
    public bool copied = false;

    private void Start() {
        cf = Camera.main.GetComponent<CameraFollow>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update() {
        if (player.dead) {
            copied = false;
        }
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            if(isSpawn && !copied) {
                copyPlayerInventory();
                copied = true;
            }
            else if(isEndofLevel) {
                copied = false;
                player.moveToNextLevel();
            }
            else if (isIntermediate) {
                // If we want to teleport to another node
                if (nextIntermediateCheckpoint != null) {
                    player.moveToNextNode(nextIntermediateCheckpoint);
                }
                // If we don't assign nextCheckpoint this means that it was just a
                // portal to move through and we're already where we want to be
            }
        }
    }

    protected void copyPlayerInventory() {
        // Copy contents of players pickups into spare array
        PlayerColorPickup pcp = player.GetComponent<PlayerColorPickup>();
        pcp.pickUpInvCopy = new List<PickupStruct>(pcp.pickUpInv);
    }
}