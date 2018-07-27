using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDead : MonoBehaviour {

    private PlayerController controller;
    public Image deadIndicator;
    Color initialCol = new Color(159 / 255, 61 / 255, 61 / 255, 0 / 255);
    Color endCol = new Color(159 / 255, 61 / 255, 61 / 255, 255 / 255);

    float resetLevelTimer = 0;
    float t = 0;
    bool indicatorDrawn = false;

    public void Start() {
        controller = GetComponent<PlayerController>();
    }

    public void Update() {
        if(controller.dead && !indicatorDrawn) {
            controller.stopMoving();
            deadIndicator.color = new Color(deadIndicator.color.r, deadIndicator.color.g, deadIndicator.color.b, deadIndicator.color.a + 1);
            indicatorDrawn = true;
            resetLevelTimer = 2;
        }
        else if(!controller.dead && indicatorDrawn) {
            deadIndicator.color = new Color(deadIndicator.color.r, deadIndicator.color.g, deadIndicator.color.b, deadIndicator.color.a - 1);
            indicatorDrawn = false;
        }

        if (resetLevelTimer > t) {
            resetLevelTimer -= Time.deltaTime;

            if (resetLevelTimer <= t) {
                resetLevelTimer = 0;
                resetPlayer();
            }
        }
    }

    // Only called once. Do everything here. Do not wait for more ticks.
    private void resetPlayer() {
        // Copy everything that is in the players last inventory (when we were last at the spawnpoint)
        // Into his current and reset his position to that checkpoint's position
        PlayerColorPickup pcp = controller.gameObject.GetComponent<PlayerColorPickup>();
        pcp.pickUpInv = pcp.pickUpInvCopy;
        Vector3 posToPutPlayer = controller.levels[controller.currLevel].getSpawnCheckpoint().teleport.transform.position;
        posToPutPlayer.y += 2.6f;
        gameObject.transform.position = posToPutPlayer;

        // So we can recopy the players inv again just incase he dies
        controller.levels[controller.currLevel].getSpawnCheckpoint().teleport.copied = false;
        controller.dead = false;
    }
}