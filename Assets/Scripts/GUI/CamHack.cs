using UnityEngine;
using System.Collections;


	// Based on example code from: http://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html
	//
	// Set an off-center projection, where perspective's vanishing
	// point is not necessarily in the center of the screen.
	//
	// left/right/top/bottom define near plane size, i.e.
	// how offset are corners of camera's near plane.
	// Tweak the values and you can see camera's frustum change.
	
[ExecuteInEditMode]

public class CamHack : MonoBehaviour {
	public float left = -0.2F;
	public float right = 0.2F;
	public float top = 0.2F;
	public float bottom = -0.2F;

	void LateUpdate() {
		// Work on *this* camera. Original demo script haunts the main camera :-(
		//Camera cam = Camera.main;
		Camera cam = GetComponent<Camera>();

		Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
		cam.projectionMatrix = m;
	}

	static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far) {
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;
		return m;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
