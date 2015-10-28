using UnityEngine;
using System.Collections;

public class QuitWatcher : MonoBehaviour {

	public SceneLoader sl;
	private ControllerInterface ci;

	// WARNING This disables the wiimote for debugging
	private bool ENABLE_WIIMOTE;

	// Wiimote controller
	private WiimoteReceiver receiver = null;

	void Start() {
		ENABLE_WIIMOTE = Wiimote.enableWiiMote ();

		// Initialize wiimote receiver
		// TODO(Douglas): Make this work for multiple controllers (if needed)
		ci = ControllerInterface.Instance;
		if (ENABLE_WIIMOTE) {
			receiver = WiimoteReceiver.Instance;
			receiver.connect ();
			while (true) { 
				if (receiver.wiimotes.ContainsKey (1)) {
					ci.setController (1, receiver.wiimotes [1]);
					break;
				}
			}
		}
	}

	// If the user presses escape, quit
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		// Start game if user press <SPACE>
		if (Input.GetKeyDown(KeyCode.Space)) {
			sl.LoadScene("TeamPlay");
		}

	if (Input.GetKeyDown(KeyCode.Return)) {
			sl.LoadScene("TeamPlay");
		}

		if (ENABLE_WIIMOTE) {
			if (ci.RotRight(1)) {
				sl.LoadScene("TeamPlay");
			}
		}
	}
}
