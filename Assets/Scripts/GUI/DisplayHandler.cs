using UnityEngine;
using System.Collections;

public class DisplayHandler : MonoBehaviour {
	static int displaysInUse = 1;

	Camera rCam, lCam, mainCam, mainCam2, sCam, sCam2, icCam1, icCam2;

	// Use this for initialization
	void Start () {
		mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
		lCam = GameObject.Find("Left Camera").GetComponent<Camera>();
		rCam = GameObject.Find("Right Camera").GetComponent<Camera>();
		sCam = GameObject.Find("Score Camera").GetComponent<Camera>();
		sCam2 = GameObject.Find("Score Camera2").GetComponent<Camera>();
		icCam1 = GameObject.Find("Icon P1 Camera").GetComponent<Camera>();
		icCam2 = GameObject.Find("Icon P2 Camera").GetComponent<Camera>();

		print ("Screen is " + Screen.width 
		       + "x" + Screen.height);

		float ratio = (float)Screen.width / (float)Screen.height;

		// Very Wide screen --> assume two separate displays merged 
		// by the operating system into one screen surface
		if (ratio >= 2.0f) {
			print ("Two screen mode");

			mainCam.enabled = false;
			rCam.enabled = true;
			lCam.enabled = true;

			// Center cameras for Left window (half)
			sCam.rect = new Rect (0f, 0f, 0.35f, 1f);

			// Relation ratio vs start point estimated manually in editor
			icCam1.rect = new Rect (0.144f + 0.022f*ratio, 0f, 0.5f, 1f);


			// Center cameras for Right window (half)
			sCam2.enabled = true; // In position, but disabled
			// Relation ratio vs start point estimated manually in editor
			icCam2.rect = new Rect (0.356f - 0.022f*ratio, 0f, 0.5f, 1f);

		} else {
			print ("One screen mode");

			mainCam.enabled = true;
			rCam.enabled = false;
			lCam.enabled = false;
		}

		//
		// Using two separate displays
		//
		// *** Untested! probably needs a Multi Display license to work!
		//
		print ("Multidisplaylicens = " + Display.MultiDisplayLicense ());
		
		if (Display.MultiDisplayLicense() && (Display.displays.Length >= 2)){
			
			displaysInUse = Display.displays.Length;
			if (displaysInUse >= 2) {
				print ("Activates 2:nd screen showing Right Camera");
				Display.displays [1].Activate (1920,1080,60);
				Display.displays [1].SetRenderingResolution (1920, 1080);
				rCam.SetTargetBuffers (Display.displays [1].colorBuffer, Display.displays [1].depthBuffer);
				rCam.enabled = true;
			}
		}		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
