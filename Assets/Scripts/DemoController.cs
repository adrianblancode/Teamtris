using UnityEngine;
using System.Collections;

//
// Demonstration how to use the Controllerinterface on an object
//

public class DemoController : MonoBehaviour {

	private Vector3 startPos;
	public float fallDistance = 7f;
	public float fallSpeed = 0.03f;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (fallSpeed * Vector3.down, Space.World);
		if (Vector3.Distance (transform.position, startPos) > fallDistance)
			transform.position = startPos;

		// Check controller input left
		if (ControllerInterface.MoveLeft(1))
			transform.Translate (Vector3.left, Space.World);

		// Check controller input right
		if (ControllerInterface.MoveRight(1))
			transform.Translate (Vector3.right, Space.World);

		// Check key for player 1 (translate)
		if (ControllerInterface.ActionButton(1))
			transform.position = startPos;


		// Check controller rotate left
		if (ControllerInterface.RotLeft(1))
			transform.Rotate (new Vector3(0f, 0f, -90f), Space.Self);
		
		// Check controller rotate right
		if (ControllerInterface.RotRight(1))
			transform.Rotate (new Vector3(0f, 0f, 90f), Space.Self);

		// Check key for player 2 (rotate)
		if (ControllerInterface.ActionButton(2))
			transform.position = startPos - fallDistance * Vector3.down;

	
	}
}
