using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controller1 : BaseController {

	private Controller2 other_controller;
	private Spawner spawner;

	public Controller1(){
		team = 1;
		other_team = 2;
	}


	protected override void Awake () {
		other_controller = GameObject.Find ("BlockController2").GetComponent<Controller2> ();
		gameBoard = GameObject.FindGameObjectWithTag ("Player1_GameBoard");

		spawner = FindObjectOfType<Spawner> ();

		for (int i = 0; i < 4; i++) {
			ghost [i] = (GameObject)Instantiate (ghostPrefab,
			                                     transform.position + new Vector3 (i, 10, 0),
			                                     Quaternion.identity);
		}

		blockGrid = new Grid (5, 25, 5);
	}

	protected override void Start () {		
		// TODO(Douglas): Make this work for multiple controllers (if needed)
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
			ci.setController(team, new Keyboard_player1());
		}
		
		currentBlock = spawner.spawnNext();
		other_controller.setBlock (currentBlock, 0, 0);
	}

	protected override void FixedUpdate(){
		if (spawn && !game_over) {
			currentBlock = spawner.spawnNext();
			int k1 = Random.Range(0, 3);
			int k2 = Random.Range(0, 3);
			currentBlock.transform.Rotate(k1*90, k2*90, 0, Space.World);
			other_controller.setBlock(currentBlock, k1, k2);
			spawn = false;
		}
	}

	protected override IEnumerator Fall(){
		// Modify position
		currentBlock.transform.position += new Vector3(0, -1, 0);
		
		// See if valid
		if (isValidGridPos()) {
			// It's valid. Update grid.
			updateGrid();
		} else {
			// It's not valid. revert.
			currentBlock.transform.position += new Vector3(0, 1, 0);
			// Clear filled horizontal lines
			linesDeleted = blockGrid.deleteFullPlans();
			// Update the scores depending on the number of lines deleted
			updateScores(linesDeleted);
			
			// Spawn next Group
			spawn = true;
			
		}
		lastFall = Time.time;
		yield return new WaitForSeconds(fastFallRate);
		fall = false;
	}

	// Every speedUpRate seconds, increases speed and shows text
	protected override void speedUp(){
		
		if(game_over){
			return;
		}
		
		int currentTime = (int) Time.timeSinceLevelLoad;
		
		// We check that we have waited speedUpRate time
		if ((currentTime - lastSpeedUp) / speedUpRate >= 1) {
			fallRateMultiplier *= speedUpMultiplier;
			lastSpeedUp = currentTime;
			
			// Show text
			speedUpText.text = "SPEED UP!";
		}
		
		// After three seconds, hide text
		if (currentTime - lastSpeedUp > 3) {
			speedUpText.text = "";
		}
	}

	void OnDestroy() {
		game_over = true;
		speedUpText.text = "Game Over";
	}
}