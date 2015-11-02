using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Block : MonoBehaviour {
	public int x_size, y_size, z_size;

	private Vector3[] vertices;

	void Awake(){
		StartCoroutine ("generateVertices");
	}
	
	IEnumerator generateVertices(){
		WaitForSeconds wait = new WaitForSeconds (0.05f);
		vertices = new Vector3[(x_size + 1) * (y_size + 1) * (z_size + 1)];
		for (int i = 0, z = 0; z <= z_size; ++z) {
			for(int y = 0; y <= y_size; ++y){
				for(int x = 0; x <= x_size; ++x){
					vertices[i] = new Vector3(x,y,z);
					yield return wait;
				}
			}
		}
	}

	private void OnDrawGizmos() {
		Debug.Log ("Hej");
		if (vertices == null) {
			return;
		}
		Debug.Log ("Drawing");
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; ++i) {
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}
}