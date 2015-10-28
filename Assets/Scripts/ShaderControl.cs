using UnityEngine;
using System.Collections;

public class ShaderControl : MonoBehaviour {
	
	private MeshRenderer rend;
	private Material mat;
	private bool circleOn;
	private bool bordersOn;
	
	void Start () {
		rend = this.GetComponent<MeshRenderer>();
		mat = rend.material;
		circleOn = false;
		bordersOn = false;
	}

	// Update is called once per frame
	void Update () {

//		updateCircle();
//		updateBorders();
	}

	void updateCircle() {
		Vector3 center = this.transform.position;
		mat.SetVector("_Center", new Vector4(center.x, center.y, center.z, 0));

		if (Input.GetKeyDown("space")) {
			circleOn = !circleOn;
		}
		
		if (!circleOn) {
			float curtime = Time.time;
			float newRadius = 0.14F * (6.0F - Mathf.Cos(curtime * 2.0F));
			mat.SetFloat("_Radius", newRadius);
		} else {
			mat.SetFloat("_Radius", 0.0F);
		}
	}

	void updateBorders() {
		Vector3 center = this.transform.position;
		mat.SetVector("_Center", new Vector4(center.x, center.y, center.z, 0));

		if (Input.GetKeyDown("space")) {
			bordersOn = !bordersOn;
		}
		
		if (!bordersOn) {
			float curtime = Time.time;
			float newStep = Mathf.Sin(curtime)/200.0F + 0.005F;
			mat.SetFloat("_Step", newStep);
		} else {
			mat.SetFloat("_Step", 0.01F);
		}

	}
}
