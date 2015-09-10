using UnityEngine;
using System.Collections;

public class BackToTitleWatcher : MonoBehaviour {

	SceneLoader sl = new SceneLoader();

	// If the user presses escape, go back to title
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			sl.LoadScene("TitleScreen");
		}
	}
}
