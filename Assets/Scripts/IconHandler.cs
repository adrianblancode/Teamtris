using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconHandler : MonoBehaviour {

	// TODO make work for both teams
	private int team = 1;

	// Move icon
	public GameObject moveObject;
	public Texture moveIcon;
	public Texture moveActiveIcon;
	private RawImage moveRawImage;

	// Rotate icon
	public GameObject rotObject;
	public Texture rotIcon;
	public Texture rotActiveIcon;
	private RawImage rotRawImage;
	
	void Start(){
		moveRawImage = (RawImage) moveObject.GetComponent<RawImage>();
		rotRawImage = (RawImage) rotObject.GetComponent<RawImage>();
	}

	void Update() {
		if (ControllerInterface.MoveLeft (team)) {
			moveRawImage.texture = moveActiveIcon;
		} else if (ControllerInterface.MoveRight (team)) {
			moveRawImage.texture = moveActiveIcon;
		} else {
			moveRawImage.texture = moveIcon;
		}

		if (ControllerInterface.RotLeft (team)) {
			rotRawImage.texture = rotActiveIcon;
		} else if (ControllerInterface.RotRight (team)) {
			rotRawImage.texture = rotActiveIcon;
		} else {
			rotRawImage.texture = rotIcon;
		}
	}
}
