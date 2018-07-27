using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorInventory : MonoBehaviour {

    public Material UIRed;
    public Material UIBlue;
    public Material UIYellow;
    public Material UIOrange;

    public Material PlayerRed;
    public Material PlayerBlue;
    public Material PlayerYellow;
    public Material PlayerOrange;

    public int invSpaceIndex;

    PlayerController playerController;
    PlayerColorPickup player;
    Image img;
    Button yourButton;

    // Use this for initialization
    void Start () {
		img = GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerColorPickup>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        yourButton = gameObject.GetComponent<Button>();
        yourButton.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void LateUpdate () {
        Color c = img.color;
        if (player.pickUpInv.Count - 1 >= invSpaceIndex) {
            PickupStruct p = (PickupStruct) player.pickUpInv[invSpaceIndex];
            if(p != null) {
                c = new Color(1, 1, 1, 1);
                img.color = c;
                switch(p.col) {
                    case PickupStruct.Colors.BLUE:
                        img.material = UIBlue;
                    break;
                    case PickupStruct.Colors.RED:
                        img.material = UIRed;
                    break;
                    case PickupStruct.Colors.YELLOW:
                        img.material = UIYellow;
                        break;
                    case PickupStruct.Colors.ORANGE:
                        img.material = UIOrange;
                        break;
                }
            }
            else {
                c = new Color(1, 1, 1, 0);
                img.color = c;
            }
        }
	}

    void TaskOnClick() {
        if(player.pickUpInv.Count - 1 >= invSpaceIndex) {
            PickupStruct p = (PickupStruct) player.pickUpInv[invSpaceIndex];
            Material material = null;

            switch(p.col) {
                case PickupStruct.Colors.BLUE:
                    playerController.col = StealthAgainstStruct.Colors.BLUE;
                    material = PlayerBlue;
                    break;
                case PickupStruct.Colors.RED:
                    playerController.col = StealthAgainstStruct.Colors.RED;
                    material = PlayerRed;
                    break;
                case PickupStruct.Colors.YELLOW:
                    playerController.col = StealthAgainstStruct.Colors.YELLOW;
                    material = PlayerYellow;
                    break;
                case PickupStruct.Colors.ORANGE:
                    playerController.col = StealthAgainstStruct.Colors.ORANGE;
                    material = PlayerOrange;
                    break;
            }

            Renderer playerShader = player.gameObject.GetComponent<Renderer>();
            playerShader.sharedMaterial = material;

            player.pickUpInv.Remove(p);

            Image img = player.colInvs[player.pickUpInv.Count];
            img.material = null;
            Color c = new Color(1,1,1,1);
            img.color = c;
        }
    }
}