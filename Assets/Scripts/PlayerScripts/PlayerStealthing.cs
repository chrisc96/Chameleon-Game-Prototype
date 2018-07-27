using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStealthing : MonoBehaviour {

    private PlayerController controller;
    List<StealthAgainstStruct> currentStealths;
    public Image stealthIndicator;
    Color initialCol = new Color(159/255, 61/255, 61/255, 0/255);
    Color endCol = new Color(159/255, 61/255, 61/255, 255/255);

    float duration = 3f;
    private float t = 0;
    bool indicatorDrawn = false;

    public void Start() {
        controller = GetComponent<PlayerController>();
        currentStealths = new List<StealthAgainstStruct>();
    }

    public void Update() {
        foreach(StealthAgainstStruct obj in currentStealths) {
            if (obj.col == controller.col) {
                controller.stealthed = true;
                stealthAgainstObj();
            }
        }

        if (controller.stealthed && !indicatorDrawn) {
            stealthIndicator.color = new Color(stealthIndicator.color.r, stealthIndicator.color.g, stealthIndicator.color.b, stealthIndicator.color.a + 1);
            indicatorDrawn = true;
        }
        else if (!controller.stealthed && indicatorDrawn) {
            stealthIndicator.color = new Color(stealthIndicator.color.r, stealthIndicator.color.g, stealthIndicator.color.b, stealthIndicator.color.a - 1);
            indicatorDrawn = false;
        }
    }

    private void stealthAgainstObj() {
        return;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "stealthObj") {
            currentStealths.Add(other.gameObject.GetComponent<StealthAgainstStruct>());
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "stealthObj") {
            currentStealths.Remove(other.gameObject.GetComponent<StealthAgainstStruct>());
            controller.stealthed = false;
        }
    }
}