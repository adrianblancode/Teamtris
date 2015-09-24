using UnityEngine;
using System.Collections;

public class QuitWatcher : MonoBehaviour {

	public SceneLoader sl;

	// If the user presses escape, quit
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			sl.LoadScene("TeamPlay");
		}
	}
}