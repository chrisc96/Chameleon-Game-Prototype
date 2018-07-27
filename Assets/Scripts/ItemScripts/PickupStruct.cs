using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupStruct : MonoBehaviour {

    public Colors col;

    public enum Colors {
        RED,
        YELLOW,
        BLUE,
        ORANGE
        // Add more here
    }

    private void Update() {
        transform.Rotate(new Vector3(0, 120, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            gameObject.SetActive(false);
        }
    }
}
