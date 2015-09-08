using UnityEngine;
using System.Collections;

//
// Interface between the Teamtris game and the 
// HW drivers for the Teamtris controller(s)
//
// *** NOTE: This script should execute before 
// *** the default timeline to ensure that it
// *** presents the same state to all readers 
// *** during a frame
//

public class ControllerInterface : MonoBehaviour {

	// Call static functions to check status of an input.
	// The value is stable during the frame and will only 
	// change between frames
	//

	//
	// Player 1 (Player who moves sideways
	//
	// team = 1 for team 1
	// Todo: support for team = 2
	public static bool MoveRight (int team) {
		if (team == 1)
			return Input.GetKeyDown("d"); 
		return false;
	}

	public static bool MoveLeft (int team) {
		if (team == 1)
			return Input.GetKeyDown("a"); 
		return false;
	}

	public static bool MoveButton (int team) {
		if (team == 1)
			return Input.GetKeyDown("q"); 
		return false;
	}


	//
	// Player 2 (Player who rotates
	//
	// team = 1 for team 1
	// Todo: support for team = 2
	public static bool RotRight (int team) {
		if (team == 1)
			return Input.GetKeyDown("s"); 
		return false;
	}
	
	public static bool RotLeft (int team) {
		if (team == 1)
			return Input.GetKeyDown("w"); 
		return false;
	}
	
	public static bool RotButton (int team) {
		if (team == 1)
			return Input.GetKeyDown("e"); 
		return false;
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
