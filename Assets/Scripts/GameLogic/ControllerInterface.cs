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

public class ControllerInterface {

	// NOTE(Douglas): Do the functions need to be static?

	// Call static functions to check status of an input.
	// The value is stable during the frame and will only 
	// change between frames

	// Player 1 (Player who moves sideways
	// team = 1 for team 1

	private Controller player;

	private int team;

	private WiimoteReceiver reciever;

	public ControllerInterface (int team, bool usingRemote, WiimoteReceiver reciever = null)
	{
		this.team = team;
		this.reciever = reciever;
		if (!usingRemote) {
			player = new Keyboard_player1 ();
		} else {
			if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
				player = (Wiimote)reciever.wiimotes [team];
			}
		}
	}
	

	public bool MoveRight (int team) {
		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
			player = (Wiimote)reciever.wiimotes [team];
		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.RightArrow);
//		} else {
//			return Input.GetKey (KeyCode.D);
//		}
		return player.MoveRight ();
	}

	public bool MoveLeft (int team) {
		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
			player = (Wiimote)reciever.wiimotes [team];
		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.LeftArrow); 
//		} else {
//			return Input.GetKey (KeyCode.A);
//		}
		return player.MoveLeft ();
	}

	// Gives a value between -1 and 1 depending on the tilt of the WiiMote
	public float MoveTilt (int team) {
		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
			player = (Wiimote)reciever.wiimotes [team];
		}

//		if (MoveLeft (team)) {
//			return -1;
//		} else if (MoveRight (team)) {
//			return 1;
//		}

		//TODO implement Wiimote stuff
		return 0;
	}

	// Player 2 (Player who rotates
	// team = 1 for team 1
	public bool RotRight (int team) {
		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
			player = (Wiimote)reciever.wiimotes [team];
		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.DownArrow);
//		} else {
//			return Input.GetKey (KeyCode.S);
//		}
		return player.RotateRight () == 1 ? true : false;
	}
	
	public bool RotLeft (int team) {
		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
			player = (Wiimote)reciever.wiimotes [team];
		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.UpArrow);
//		} else {
//			return Input.GetKey (KeyCode.W);
//		}
		return player.RotateLeft () == 1 ? true : false;
	}

	// Gives a value between -1 and 1 depending on the tilt of the WiiMote
	public float RotTilt (int team) {
		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
			player = (Wiimote)reciever.wiimotes [team];
		}
		
		if (RotLeft (team)) {
			return -1;
		} else if (RotRight (team)) {
			return 1;
		}
		
		//TODO implement Wiimote stuff
		return 0;
	}

	/* 
	 * Action button, this should be the button for menu selection and drop blocks 
	 */

	// Checks if a specific player on a specific team has pushed the action button
	// TODO assign wiimote buttons to players
	public bool ActionButton (int team, int player) {
		if (team == 1) {
			if(player == 1) {
				return Input.GetKey (KeyCode.Space);
			} else if (player == 2) {
				return Input.GetKey (KeyCode.Space);
			}
		} else if(team == 2) {
			if(player == 1) {
				return Input.GetKey (KeyCode.Space);
			} else if (player == 2) {
				return Input.GetKey (KeyCode.Space);
			}
		}

		return false;
	}

	// Checks whether both players of the team have pressed the action button
	public bool ActionButtonCombined (int team) {
		return ActionButton(team, 1) && ActionButton(team, 2); 
	}
}
