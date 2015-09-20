using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconHandler : MonoBehaviour {

	// TODO make work for both teams
	public int player;

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

	// Down icon
	public GameObject downObject;
	public Texture downIcon;
	public Texture downActiveIcon;
	public Texture downLeftIcon;
	public Texture downRightIcon;
	private RawImage downRawImage;

	void Start(){
		moveRawImage = (RawImage) moveObject.GetComponent<RawImage>();
		rotRawImage = (RawImage) rotObject.GetComponent<RawImage>();
		downRawImage = (RawImage) downObject.GetComponent<RawImage>();
	}

	void Update() {
		if (ControllerInterface.MoveLeft (player)) {
			moveRawImage.texture = moveActiveIcon;
		} else if (ControllerInterface.MoveRight (player)) {
			moveRawImage.texture = moveActiveIcon;
		} else {
			moveRawImage.texture = moveIcon;
		}

		if (ControllerInterface.RotLeft (player)) {
			rotRawImage.texture = rotActiveIcon;
		} else if (ControllerInterface.RotRight (player)) {
			rotRawImage.texture = rotActiveIcon;
		} else {
			rotRawImage.texture = rotIcon;
		}

		if (ControllerInterface.ActionButtonCombined (player)) {
			downRawImage.texture = downActiveIcon;
		} else if (ControllerInterface.ActionButton (player, 1)) {
			downRawImage.texture = downLeftIcon;
		} else if (ControllerInterface.ActionButton (player, 2)) {
			downRawImage.texture = downRightIcon;
		} else {
			downRawImage.texture = downIcon;
		}

		moveRawImage.transform.eulerAngles = new Vector3(0, 0, -40 * ControllerInterface.MoveTilt(player));
		rotRawImage.transform.eulerAngles = new Vector3(0, 0, -45 * ControllerInterface.RotTilt(player));
	}
}
