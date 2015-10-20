using UnityEngine;
using System.Collections;

public class MatchEffectControl : MonoBehaviour {

	// #of spots in front, the rest are assumed to be on the side
	const int frontSpots = 4; 

	// Use this for initialization
	void Start () {
	//	StartCoroutine("WalkingLights");


		// Activate a laser beam from every child object having
		// a LineRenderer component
		foreach (Transform child in transform) {
			if (child.GetComponent<LineRenderer>()) {
				StartCoroutine("Laser", child.transform);
			}
		}
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

	//
	// Activates a laser beam from a Component containing
	// a LineRenderer component
	// 
	// direction:  Use transform.forward as default
	//
	IEnumerator Laser(Transform laserEmitter) {
		LineRenderer line = laserEmitter.GetComponent<LineRenderer> ();

		line.enabled = true;
		print ("Passerat line.enabled!");

		while (true) {
			// Cycles texture over time
			line.material.mainTextureOffset = new Vector2(0, Time.time);

			Ray ray = new Ray (laserEmitter.position, laserEmitter.forward);
			RaycastHit hit;

			line.SetPosition (0, ray.origin);

			if (Physics.Raycast (ray, out hit, 100)) {
				line.SetPosition (1, hit.point);

				// Tutorial example shooting at a point of an object
				if (hit.rigidbody) {
					hit.rigidbody.AddForceAtPosition(laserEmitter.forward, hit.point);
				}

			} else {
				line.SetPosition (1, ray.GetPoint(100));
			}
			yield return null;
		}
	}
}
