using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWon : MonoBehaviour {

    private PlayerController controller;
    public Image wonIndicator;
    Color initialCol = new Color(159 / 255, 61 / 255, 61 / 255, 0 / 255);
    Color endCol = new Color(159 / 255, 61 / 255, 61 / 255, 255 / 255);

    float resetLevelTimer = 0;
    float t = 0;
    bool indicatorDrawn = false;

    public void Start() {
        controller = GetComponent<PlayerController>();
    }

    public void Update() {
        if(controller.won && !indicatorDrawn) {
            controller.stopMoving();
            wonIndicator.color = new Color(wonIndicator.color.r, wonIndicator.color.g, wonIndicator.color.b, wonIndicator.color.a + 1);
            indicatorDrawn = true;
            resetLevelTimer = 2;
        } else if(!controller.won && indicatorDrawn) {
            wonIndicator.color = new Color(wonIndicator.color.r, wonIndicator.color.g, wonIndicator.color.b, wonIndicator.color.a - 1);
            indicatorDrawn = false;
        }

        if(resetLevelTimer > t) {
            resetLevelTimer -= Time.deltaTime;

            if(resetLevelTimer <= t) {
                resetLevelTimer = 0;

                // Go back to main menu or do whatever
            }
        }
    }
}