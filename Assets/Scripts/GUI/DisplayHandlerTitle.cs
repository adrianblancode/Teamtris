using UnityEngine;
using System.Collections;

public class DisplayHandlerTitle : MonoBehaviour {
	static int displaysInUse = 1;

	Camera mainCam, mainCam2;

	// Use this for initialization
	void Start () {
		mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
		mainCam2 = GameObject.Find("Main Camera2").GetComponent<Camera>();

		print ("Screen is " + Screen.width 
		       + "x" + Screen.height);
		
		// Very Wide screen --> assume two separate displays merged 
		// by the operating system into one screen surface
		if ((float)Screen.width / (float)Screen.height >= 2.0f) {
			print ("Two screen mode");

			mainCam2.enabled = true;

			// Center 1:1 cameras for Left window (half)
			mainCam.rect = new Rect (0f, 0f, 0.5f, 1f);
			mainCam.orthographicSize = mainCam2.orthographicSize;
		} else {
			print ("One screen mode");
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
				mainCam2.SetTargetBuffers (Display.displays [1].colorBuffer, Display.displays [1].depthBuffer);
				mainCam2.enabled = true;
			}
		}		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
