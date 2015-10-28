using UnityEngine;
using System.Collections;

public class BackToTitleWatcher : MonoBehaviour {

	public SceneLoader sl;
	private ControllerInterface ci;

	// WARNING This disables the wiimote for debugging
	private bool ENABLE_WIIMOTE;
	private Controller1 controller;

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

	// If the user presses escape, backspace 
	// or WIImote home, go back to title
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			sl.LoadScene("TitleScreen");
		}

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			sl.LoadScene("TitleScreen");
		}

		if (ENABLE_WIIMOTE) {
			if (ci.Quit(1)) {
				sl.LoadScene("TitleScreen");
			}
		}
	}
}
