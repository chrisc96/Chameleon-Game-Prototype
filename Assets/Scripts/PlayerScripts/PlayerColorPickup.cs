using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerColorPickup : MonoBehaviour {

    public List<PickupStruct> pickUpInv;
    public List<PickupStruct> pickUpInvCopy;
    public Image[] colInvs = new Image[4];

    // Use this for initialization
    void Start () {
        pickUpInv = new List<PickupStruct>();
        pickUpInvCopy = new List<PickupStruct>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Pickup") {
            addToPickups(other);
            other.gameObject.SetActive(false);
        }
    }

    private void addToPickups(Collider other) {
        PickupStruct p = other.gameObject.GetComponent<PickupStruct>();
        foreach(PickupStruct pickup in pickUpInv) {
            if (pickup != null) {
                if(p.col == pickup.col) return;
            }
        }
        if (pickUpInv.Count == 4) {
            pickUpInv[0] = null;
            for (int i = 1; i < pickUpInv.Count; i++) {
                if (pickUpInv[i] != null) {
                    pickUpInv[i-1] = pickUpInv[i];
                }
            }
        }
        pickUpInv.Add(other.gameObject.GetComponent<PickupStruct>());
    }
}
