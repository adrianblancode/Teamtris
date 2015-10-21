using UnityEngine;
using System.Collections;

public class MatchEffectControl : MonoBehaviour {
	// Script keeps an own active flag, since deactivating the 
	// Component's flag will stop the script and deactivate forever
	public bool active = false;
	private bool notStarted = true;

	void Start () {
		activationMode(active);	
	}
	

	// Called when a value is changed in inspector
	void OnValidate () {
		activationMode(active);
	}

	void Update () {

		// Toggle this effect on/off by key L = "laser"
		if (Input.GetKeyDown (KeyCode.L)) {
			active = !active;
			activationMode(active);
		}
	}

	//
	// Set activationmode
	//
	// Shows or hides all objects used for the matcheffect feature
	//
	void activationMode(bool active) {
		foreach (Transform child in transform) {
			child.gameObject.SetActive(active);
		}

		if (active && notStarted) {
			// Activate a laser beam from every child object having
			// a LineRenderer component
			foreach (Transform child in transform) {
				if (child.GetComponent<LineRenderer> ()) {
					StartCoroutine ("Laser", child.transform);
				}
			}
			notStarted = false;
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
			// Idle, unless active
			if (active) {
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
			}
			else {
				haloPoint2.enabled = false;
			}
			yield return null;
		}
	}
}
