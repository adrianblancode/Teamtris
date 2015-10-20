using UnityEngine;
using System.Collections;

public class CustomShader : MonoBehaviour {

	// We attach our custom shader to a material
	public Material material;

	// And then we use that material on the texture, before the final image is displayed
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit (source, destination, material);
	}
}
