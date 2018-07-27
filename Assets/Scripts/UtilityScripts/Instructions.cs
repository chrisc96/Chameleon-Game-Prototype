using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour {

    public Sprite instructionalImage;
    public Image staticImageHolder;
    // Prefix of Tut_Instructions_ for filename, 1,2,3 etc is at the end of the name.
    // We can find the image this way
    public int imageNameIndex;

    // Use this for initialization
    void Start () {
        instructionalImage = gameObject.GetComponentInChildren<Image>().sprite;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            staticImageHolder.sprite = instructionalImage;
            Color c = staticImageHolder.color;
            c.a = 1;
            staticImageHolder.color = c;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player") {
            staticImageHolder.sprite = null;
            Color c = staticImageHolder.color;
            c.a = 0;
            staticImageHolder.color = c;
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}