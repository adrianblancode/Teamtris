using UnityEngine;
using System.Collections;

public class TitleTextureMovement : MonoBehaviour {

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

		if (transform.position.y < -100) {
			repositionBlock();
		}
	}

	// Puts the block at the top of the screen at the appropriate x coordinate
	void repositionBlock() {
		offset = Random.Range (0, (Screen.width / 2) - 300);
		
		if (isLeft) {
			xpos = offset;
		} else {
			xpos = Screen.width - offset;
		}

		transform.position = new Vector3(xpos, Screen.height + 50, 0);
	}
}
