using UnityEngine;
using System.Collections;

public class MatchEffectControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("WalkingLights");	
	}
	
	// Update is called once per frame
	void Update () {			
	}

	IEnumerator WalkingLights() {
		while (true) {
			print("koko  ");
			foreach (Transform child in transform) {
				Light spot = child.GetComponent<Light>();
				spot.enabled = true;
			}
			yield return new WaitForSeconds(.3f);

			foreach (Transform child in transform) {
				Light spot = child.GetComponent<Light>();
				spot.enabled = false;
			}
			yield return new WaitForSeconds(.8f);
		}
	}
}
