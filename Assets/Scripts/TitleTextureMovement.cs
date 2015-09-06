using UnityEngine;
using System.Collections;

public class TitleTextureMovement : MonoBehaviour {

	float xpos;
	float offset;

	// Use this for initialization
	void Start () {
		xpos = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.down);

		offset = Random.Range (-200, 200);

		if (transform.position.y < -100) {
			transform.position = new Vector3(xpos + offset, Screen.height + 50, 0);
		}
	}
}
