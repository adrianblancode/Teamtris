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
	// Activates a laser beam from an object containing
	// - a LineRenderer component (laserbeam)
	// - a Light component in a child (halo point at line start)
	//
	IEnumerator Laser(Transform laserEmitter) {
		LineRenderer line = laserEmitter.GetComponent<LineRenderer> ();
		Light haloPoint1 = laserEmitter.GetComponentInChildren<Light> ();

		// Make a second halo point for the beam hitting something
		Light haloPoint2 = GameObject.Instantiate (haloPoint1);
		haloPoint2.enabled = false;

		line.enabled = true;

		while (true) {
			// Cycles texture over time
			line.material.mainTextureOffset = new Vector2(0, Time.time);

			Ray ray = new Ray (laserEmitter.position, laserEmitter.forward);
			RaycastHit hit;

			line.SetPosition (0, ray.origin);

			if (Physics.Raycast (ray, out hit, 10)) {
				// Beam ends at object it hits, add halo glow there
				line.SetPosition (1, hit.point);
				// Move halo a bit to get it just outside the block
				haloPoint2.transform.position = hit.point - 0.15f*laserEmitter.forward;
				haloPoint2.enabled = true;

				// If the object is a physics object, we can blow it away
				if (hit.rigidbody) {
//					hit.rigidbody.AddForceAtPosition(laserEmitter.forward*10, hit.point);
				}
			}
			else {
				// Beam goes to infinity, no halo at the end point
				haloPoint2.enabled = false;
				line.SetPosition (1, ray.GetPoint(100));
			}
			yield return null;
		}
	}
}
