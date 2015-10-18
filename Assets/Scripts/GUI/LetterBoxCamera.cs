using UnityEngine;
using System.Collections;

//
// Adapts camera height to get the desired width x height
// ratio, aka "letterboxing".
//

[ExecuteInEditMode]

public class LetterBoxCamera : MonoBehaviour {
	[Header("Scales camera heigth to desired ratio ")]
	
	[Tooltip("Ratio of the camera view to fit in (a part of) the display.")]
	public float ratioWidth;
	[Tooltip("Ratio of the camera view to fit in (a part of) the display.")]
	public float ratioHeight;
	[Tooltip("Set viewport here, script calculates a new viewport for the camera that fits the display ratio.")]
	public Rect desiredCameraViewportRect;

	private int w = 0, h = 0;
	private Camera cam;

	//
	// Set up defaults when script is added to the camera
	//
	void Reset() {
		cam = GetComponent<Camera>();
		ratioWidth = 16;  ratioHeight = 9;
		desiredCameraViewportRect = new Rect(
			cam.rect.x, cam.rect.y,
			cam.rect.width, cam.rect.height);
		SetupRatio();
	}

	// Use this for initialization
	// Called once, when script is loaded
	void Awake () {
		cam = GetComponent<Camera>();
	}

	// Called when a value is changed in inspector
	void OnValidate () {
		cam = GetComponent<Camera>();
		if (ratioWidth <= 0) ratioWidth = 1;
		if (ratioHeight <= 0) ratioHeight = 1;

		SetupRatio();
	}

	// Update is called once per frame
	void Update () {
		// Change in real time if screen is resized
		//
		// Could be located in Start(), but this 
		// reacts immediately  on resized window in editor :-)
		if (w != Screen.width || h != Screen.height) {
			w = Screen.width; h = Screen.height;
			SetupRatio();
		}
	}

	//
	// Setup new ratio
	// Very infrequent, needed at start or when screen is resized
	//
	private void SetupRatio () {
		//
		// Calculate desired camera parameters
		//
		// Formulas:
		// w / h = aspect  <--> w = aspect * h <--> h = w / aspect
		// aspect > 1 means more width than tallness
		//

		// Pixels and aspect ratio of the screen part this camera should fill
		float portPixelWidth = desiredCameraViewportRect.width * (float)w;
		float portPixelHeight = desiredCameraViewportRect.height * (float)h;

		// Ratio w:h that we want the player to see on screen
		float camAspect = ratioWidth / ratioHeight;

		//
		// Setup camera 
		//
		cam.aspect = camAspect; // Camera captures with correct aspect

		// Image captured by camera image must fit a screen part that may 
		// have a different aspect -->
		// fit by shrinking heigth or width of the camera image
		//
		if (portPixelWidth * ratioHeight >= portPixelHeight * ratioWidth) {
			float camHeight = portPixelWidth * camAspect;

			cam.rect = new Rect (
				desiredCameraViewportRect.x,
				desiredCameraViewportRect.y + 0.5f * 
					(1f - camHeight/(float)h), // center
				desiredCameraViewportRect.width, // width is fixed
				camHeight/(float)h);
		}
		else {
			// Shrink camera width to match aspect
			float camWidth = portPixelHeight / camAspect;

			cam.rect = new Rect (
				desiredCameraViewportRect.x + 0.5f *
					(1f - camWidth/(float)w), // center
				desiredCameraViewportRect.y,
				camWidth/(float)w,
				desiredCameraViewportRect.height);
	    }
	}
}

