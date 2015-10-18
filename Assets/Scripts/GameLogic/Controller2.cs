using UnityEngine;
using UnityEngine.UI;

public class Controller2 : BaseController {

	private GameObject spawner;

	public Controller2(){
		other_team = 1;
		team = 2;
	}

	protected override void Update (){
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			Debug.Log("GAME OVER");
			Destroy (currentBlock);
			Destroy(this);
		}
		
		
		// TODO(Douglas): Clean up button checking for wiimotes.
		// Move Left
		//		if ((ControllerInterface.MoveLeft (team)) && !left) {
		if(ci.MoveLeft(1) && !move){
			move = true;
			StartCoroutine("MoveRightZ");
		}
		
		// Move Right
		//		if (ControllerInterface.MoveRight (team) && !right) {
		else if(ci.MoveRight(1) && !move){
			move = true;
			StartCoroutine ("MoveLeftZ");
		}
		
		// Rotate Left
		//		if (ControllerInterface.RotLeft (team) && !rotate) {
		else if(ci.RotLeft(1) && !rotate){
			rotate = true;
			StartCoroutine("RotateRightZ");
		}
		
		// Rotate Left
		//		if (ControllerInterface.RotRight (team) && !rotate) {
		else if(ci.RotRight(1) && !rotate){
			rotate = true;
			StartCoroutine("RotateLeftZ");
		}
		
		if(ci.MoveLeft(2) && !move){
			move = true;
			StartCoroutine ("MoveLeftX");
		}
		
		else if(ci.MoveRight(2) && !move){
			move = true;
			StartCoroutine ("MoveRightX");
		}
		
		else if(ci.RotLeft(2) && !rotate){
			rotate = true;
			StartCoroutine ("RotateLeftX");
		}
		
		else if(ci.RotRight(2) && !rotate){
			rotate = true;
			StartCoroutine ("RotateRightX");
		}
		
		// Move Downwards and Fall
		//		if (ControllerInterface.ActionButtonCombined (1) ||
		//			Time.time - lastFall >= fallRate * fallRateMultiplier && !fall) {
		if((ci.MoveDownCombined() ||
		    Time.time - lastFall >= fallRate * fallRateMultiplier) && !fall){
			fall = true;
			StartCoroutine ("Fall");
		}
		
		applyTransparency();
		speedUp();
	}

	protected override void Awake () {
		gameBoard = GameObject.FindGameObjectWithTag ("Player2_GameBoard");
		
		spawner = GameObject.Find ("Spawner2");
		
		for (int i = 0; i < 4; i++) {
			ghost [i] = (GameObject)Instantiate (ghostPrefab,
			                                     transform.position + new Vector3 (i, 10, 0),
			                                     Quaternion.identity);
		}
		
		blockGrid = new Grid (4, 20, 4);
	}

	protected override void Start(){
		ci = ControllerInterface.Instance;
		if (ENABLE_WIIMOTE) {
			receiver = WiimoteReceiver.Instance;
			receiver.connect ();
			while (true) { 
				if (receiver.wiimotes.ContainsKey(team)) {
					ci.setController(team,	receiver.wiimotes[team]);
					break;
				}
			}
		} else {
			ci.setController(team, new Keyboard_player2());
		}
	}

	public void setBlock(GameObject block, int k1, int k2){
		currentBlock = (GameObject)Instantiate (block, spawner.transform.position, spawner.transform.rotation);
		currentBlock.transform.Rotate (k1 * 90, k2 * 90, 0, Space.Self);
	}

}

