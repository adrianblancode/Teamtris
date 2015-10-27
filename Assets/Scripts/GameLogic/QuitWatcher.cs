using UnityEngine;
using System.Collections;

public class QuitWatcher : MonoBehaviour {

	public SceneLoader sl;

	// If the user presses escape, quit
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		// Start game if user press <SPACE>
		if (Input.GetKeyDown(KeyCode.Space)) {
			sl.LoadScene("TeamPlay");
		}
	}
}