using UnityEngine;
using System.Collections;

public class BackToTitleWatcher : MonoBehaviour {

	public SceneLoader sl;

	// If the user presses escape, go back to title
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			sl.LoadScene("TitleScreen");
		}
	}
}
