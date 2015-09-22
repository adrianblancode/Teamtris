using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

	public Text timerText;

	void OnGUI () {
		string minStr = ((int) Time.timeSinceLevelLoad / 60).ToString();
		string secStr = ((int) Time.timeSinceLevelLoad % 60).ToString();

		if (minStr.Length == 1 ){
			minStr = "0" + minStr;
		}

		if (secStr.Length == 1) {
			secStr = "0" + secStr;
		}

		timerText.text = minStr + ":" + secStr;
	}
}
