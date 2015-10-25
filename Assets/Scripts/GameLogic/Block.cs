using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)]
public class Block : MonoBehaviour {
	public int x_size, y_size, z_size;

	private Vector3[] vertices;

	public Block (){

	}
	
	private void generateVertices(){
		vertices = new Vector3[(x_size + 1) * (y_size + 1) * (z_size + 1)];
	}

	private void OnDrawGizmos() {
		if (vertices == null) {
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; ++i) {
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}
}