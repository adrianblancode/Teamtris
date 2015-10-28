using UnityEngine;
using UnityEngine.UI;

public class Controller2 : BaseController {

	private GameObject spawner;

	public Controller2(){
		other_team = 1;
		team = 2;
	}

	protected override void Awake () {
		gameBoard = GameObject.FindGameObjectWithTag ("Player2_GameBoard");
		
		spawner = GameObject.Find ("Spawner2");

		nextBlockPosition = nextBlockText.transform.position + new Vector3(1, -2, 0);

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

	protected override void Update (){
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			Destroy (currentBlock);
			Destroy(this);
		}
		
		
		// TODO(Douglas): Clean up button checking for wiimotes.
		// Move Left
		if(ci.MoveLeft(1) && !move){
			move = true;
			StartCoroutine("MoveRightZ");
		}
		
		// Move Right
		else if(ci.MoveRight(1) && !move){
			move = true;
			StartCoroutine ("MoveLeftZ");
		}
		
		// Rotate Left
		else if(ci.RotLeft(1) && !rotate){
			rotate = true;
			StartCoroutine("RotateRightZ");
		}
		
		// Rotate Left
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
		if((ci.MoveDownCombined() ||
		    Time.time - lastFall >= fallRate * fallRateMultiplier) && !fall){
			fall = true;
			StartCoroutine ("Fall");
		}
		
		applyTransparency();
		speedUp();
	}

	public void setBlock(GameObject block, int k1, int k2){
		currentBlock = (GameObject)Instantiate (block, spawner.transform.position, spawner.transform.rotation);
		currentBlock.transform.Rotate (k1 * 90, k2 * 90, 0, Space.Self);
		Destroy (nextBlock);
	}

	public void setNextBlock(GameObject block){
		nextBlock = (GameObject)Instantiate (block, nextBlockPosition, block.transform.rotation);
	}

	void OnDestroy() {
		game_over = true;
		if (speedUpText) {
			speedUpText.text = "Game Over";
		}
	}
}

