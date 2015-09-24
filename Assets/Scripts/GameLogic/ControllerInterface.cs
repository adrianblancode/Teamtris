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

	static readonly ControllerInterface instance = new ControllerInterface();

	// NOTE(Douglas): Do the functions need to be static?

	// Call static functions to check status of an input.
	// The value is stable during the frame and will only 
	// change between frames

	// Player 1 (Player who moves sideways
	// team = 1 for team 1

	private Controller 
		player1,
		player2;

//	private int team;

	public bool wiimoteIsConnected = false;

	public void setController(int player, Controller controller) {
		if (player == 1) {
			player1 = controller;
		} else if (player == 2) {
			player2 = controller;
		} else {
			throw new UnityException ("Tried to assign controller to unknown player");
		}
	}

	public Controller getController(int player) {
		if (player == 1) {
			return player1;
		} else if (player == 2) {
			return player2;
		} else {
			throw new UnityException ("Tried to get controller from unknown player");
		}
	}

	// Returning the instance of this class.
	public static ControllerInterface Instance { get {return instance;}}

	public ControllerInterface () {}

	public ControllerInterface (int team, bool usingRemote, WiimoteReceiver reciever = null)
	{
		//this.team = team;
		//this.reciever = reciever;
		if (!usingRemote) {
			if (team == 1) {
				player1 = new Keyboard_player1 ();
			} else {
				player2 = new Keyboard_player2 ();
			}
		} else {
			if (reciever != null) {
				do {
					if(team == 1 && reciever.wiimotes.ContainsKey(team)) {
						player1 = (Wiimote)reciever.wiimotes [team];
					} else if (reciever.wiimotes.ContainsKey(team)) {
						player2 = (Wiimote)reciever.wiimotes[team];
					}
				} while (!reciever.wiimotes.ContainsKey(team));
			} else if (team == 1) {
				player1 = new Keyboard_player1 ();
			} else {
				player2 = new Keyboard_player2 ();
			}
		}
	}
	

	public bool Quit (int team) {
		if (team == 1) {
			return player1.Quit ();
		} else {
			return player2.Quit ();
		}
	}

	public bool MoveRight (int team) {
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}
		if (team == 1) {
			return player1.MoveRight ();
		} else {
			return player2.MoveRight ();
		}
	}

	public bool MoveLeft (int team) {
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.LeftArrow); 
//		} else {
//			return Input.GetKey (KeyCode.A);
//		}
		if (team == 1) {
			return player1.MoveLeft ();
		} else {
			return player2.MoveLeft ();
		}
	}
	
	public bool MoveDown(int team) {
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}
		if (player1 != null && player2 != null) {

			if (team == 1) {
				return player1.MoveDown ();
			} else {
				return player2.MoveDown ();
			}
		}

		return false;
	}

	public bool MoveDownCombined() {
		if (player1 != null && player2 != null) {
			return player1.MoveDown () && player2.MoveDown ();
		}

		return false;
	}

	// Gives a value between -1 and 1 depending on the tilt of the WiiMote
	public float MoveTilt (int team) {
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}

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
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.DownArrow);
//		} else {
//			return Input.GetKey (KeyCode.S);
//		}
		if (team == 1) {
			return player1.RotateRight() == 1 ? true : false;
		} else {
			return player2.RotateRight() == 1 ? true : false;
		}
	}
	
	public bool RotLeft (int team) {
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}
//		if (team == 1) {
//			return Input.GetKey (KeyCode.UpArrow);
//		} else {
//			return Input.GetKey (KeyCode.W);
//		}
		if (team == 1) {
			return player1.RotateLeft() == 1 ? true : false;
		} else {
			return player2.RotateLeft() == 1 ? true : false;
		}
	}

	// Gives a value between -1 and 1 depending on the tilt of the WiiMote
	public float RotTilt (int team) {
		// Might be a performance sink. Refactor if needed
//		if (reciever != null && reciever.wiimotes.ContainsKey(team)) {
//			if (team == 1) {
//				player1 = (Wiimote)reciever.wiimotes [team];
//			} else {
//				player2 = (Wiimote)reciever.wiimotes [team];
//			}
//		}
		
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
