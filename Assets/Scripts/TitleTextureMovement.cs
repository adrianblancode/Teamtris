using UnityEngine;
using System.Collections;

public class TitleTextureMovement : MonoBehaviour {

	int screenPadding = 50;
	int buttonPadding = 150;
	int blockWidth = 200;

	bool isLeft;
	float xpos;
	float offset;

	// Use this for initialization
	void Start () {
		// Init whether the block is left or right
		// This makes it not collide with the buttons
		xpos = transform.position.x;
		if (xpos < Screen.width / 2) {
			isLeft = true;
		}

		repositionBlock();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.down);

		// If the block has gone off screen
		if (transform.position.y < -100) {
			repositionBlock();
		}
	}

	// Puts the block at the top of the screen at the appropriate x coordinate
	void repositionBlock() {

		// We make sure that the button does not go offscreen, and does not collide with the buttons
		if (isLeft) {
			xpos = Random.Range (screenPadding, (Screen.width / 2) - (blockWidth + buttonPadding));
		} else {
			xpos = Random.Range ((Screen.width / 2) + blockWidth, Screen.width - screenPadding);
		}

		// Put the block at the top of the screen
		transform.position = new Vector3(xpos, Screen.height + 50, 0);
	}

	// TODO changes the image of the block
	void changeBlockType(){}
}
