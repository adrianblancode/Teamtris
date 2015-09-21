using UnityEngine;
using System.Collections;

/*
 * Implement this class and define the getters for the controller.
 * This may not be needed if we make sure the players do not share buttons,
 * but it works right now so there is no need to fix it.
 */
public abstract class Keyboard : Controller {

	abstract public KeyCode right{ get; set; }
	abstract public KeyCode left{ get; set; }
	abstract public KeyCode up{ get; set; }
	abstract public KeyCode down{ get; set; }
	abstract public KeyCode rotRight{ get; set; }
	abstract public KeyCode rotLeft{ get; set; }
	abstract public KeyCode confirm{ get; set; }
	abstract public KeyCode reject{ get; set; }
	abstract public KeyCode pause{ get; set; }

	public bool MoveRight ()
	{
		return Input.GetKey (right);
	}

	public bool MoveLeft ()
	{
		return Input.GetKey (left);
	}

	public bool MoveUp ()
	{
		return Input.GetKey (up);
	}

	public bool MoveDown ()
	{
		return Input.GetKey (down);
	}

	public float RotateRight ()
	{
		return Input.GetKey (rotRight) ? 1f : 0f;
	}

	public float RotateLeft ()
	{
		return Input.GetKey (rotLeft) ? 1f : 0f;
	}

	public bool Confirm ()
	{
		return Input.GetKey (confirm);
	}

	public bool Reject ()
	{
		return Input.GetKey (reject);
	}

	public bool Pause ()
	{
		return Input.GetKey (pause);
	}
}
