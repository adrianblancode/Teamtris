using UnityEngine;
using System.Collections;

public class MatchEffectControl : MonoBehaviour {

	// #of spots in front, the rest are assumed to be on the side
	const int frontSpots = 4; 

	// Use this for initialization
	void Start () {
		StartCoroutine("WalkingLights");	
	}
	
	// Update is called once per frame
	void Update () {			
	}

	IEnumerator WalkingLights() {
		int spotNumber;

		while (true) {
			spotNumber = 1;
			foreach (Transform child in transform) {
				Light spot = child.GetComponent<Light>();

				// Enable front spots, disable side spots 
				if (spotNumber++ <= frontSpots)
					spot.enabled = true;
				else
					spot.enabled = false;
			}
			yield return new WaitForSeconds(.3f);

			spotNumber = 1;
			foreach (Transform child in transform) {
				Light spot = child.GetComponent<Light>();
				
				// Disable front spots, enable side spots 
				if (spotNumber++ <= frontSpots)
					spot.enabled = false;
				else
					spot.enabled = true;
			}
			yield return new WaitForSeconds(.8f);
		}
	}
}
