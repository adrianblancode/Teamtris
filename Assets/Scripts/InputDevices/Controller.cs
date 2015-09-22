using UnityEngine;
using System.Collections;

public interface Controller {

	bool MoveRight();
	bool MoveLeft();
	bool MoveUp();
	bool MoveDown();

	/*
	 * When implementing this function for wiimotes (or other non-discrete controllers)
	 * return a value between (-1, 1).
	 * 
	 * Otherwise
	 * 	0: FALSE
	 * 	1: TRUE 
	 */
	float RotateRight();
	float RotateLeft();

	bool Confirm();
	bool Reject();
	bool Pause();
}
