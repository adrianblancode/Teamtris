using UnityEngine;
using System.Collections;

public class QuitWatcher : MonoBehaviour {

	// If the user presses escape, quit
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
