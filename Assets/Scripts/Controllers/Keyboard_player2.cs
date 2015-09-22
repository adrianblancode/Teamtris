using UnityEngine;
using System.Collections;

public class Keyboard_player1 : Keyboard {

	// TODO(Douglas): Able to set the controls.
	public override KeyCode right{ get { return b_right; } set{} }
	public override KeyCode left{ get { return b_left; } set{} }
	public override KeyCode up{ get { return b_up; } set{} }
	public override KeyCode down{ get { return b_down; } set{} }
	public override KeyCode rotRight{ get { return b_rotRight; } set{} }
	public override KeyCode rotLeft{ get { return b_rotLeft; } set{} }
	public override KeyCode confirm{ get { return b_confirm; } set{} }
	public override KeyCode reject{ get { return b_reject; } set{} }
	public override KeyCode pause{ get { return b_pause; } set{} }

	private KeyCode 
		b_right,
		b_left,
		b_up,
		b_down,
		b_rotRight,
		b_rotLeft,
		b_confirm,
		b_reject,
		b_pause;

	/*
	 * Default bindings for player 1
	 */
	public Keyboard_player2() {
		b_right = KeyCode.D;
		b_left = KeyCode.A;
		b_up = KeyCode.W;
		b_down = KeyCode.S;
		b_rotRight = KeyCode.W;
		b_rotLeft = KeyCode.S;
		b_confirm = KeyCode.E;
		b_reject = KeyCode.R;
		b_pause = KeyCode.Q;
	}
}
