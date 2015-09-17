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

	// 1 == player 1, 2 == player 2
	public static bool MoveRight (int player) {
		if (player == 1) {
			return Input.GetKey (KeyCode.RightArrow);
		} else {
			return Input.GetKey (KeyCode.D);
		}
	}

	public static bool MoveLeft (int player) {
		if (player == 1) {
			return Input.GetKey (KeyCode.LeftArrow); 
		} else {
			return Input.GetKey (KeyCode.A);
		}
	}

	public static bool LeftButton(int player){
		return MoveLeft(player);
	}
	
	public static bool RightButton(int player){
		return MoveRight(player);
	}
	
	public static bool RotRight (int player) {
		if (player == 1) {
			return Input.GetKey (KeyCode.DownArrow);
		} else {
			return Input.GetKey (KeyCode.S);
		}
	}
	
	public static bool RotLeft (int player) {
		if (player == 1) {
			return Input.GetKey (KeyCode.UpArrow);
		} else {
			return Input.GetKey (KeyCode.W);
		}
	}

	public static bool OneButton(int player){
		return RotLeft(player);
	}

	public static bool TwoButton(int player){
		return RotRight(player);
	}

	/* 
	 * Action button, this should be the button for menu selection and drop blocks 
	 */

	// Checks if a specific player has pushed the action button
	// TODO assign wiimote buttons to players
	public static bool ActionButton (int player) {
		if(player == 1) {
			return Input.GetKey (KeyCode.Space);
		} else if (player == 2) {
			return Input.GetKey (KeyCode.Space);
		}

		return false;
	}

	// Checks whether both players have pressed the action button
	public static bool ActionButtonCombined (int player) {
		return ActionButton(1) && ActionButton(2); 
	}
}
