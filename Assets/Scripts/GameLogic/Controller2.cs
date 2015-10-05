using UnityEngine;
using UnityEngine.UI;

public class Controller2 : BaseController {

	private GameObject spawner;

	public Controller2(){
		other_team = 1;
		team = 2;
	}

	protected override void Awake () {
//		other_controller = GameObject.Find ("BlockController1").GetComponent<Controller1>();
		gameBoard = GameObject.FindGameObjectWithTag ("Player2_GameBoard");
		
		spawner = GameObject.Find ("Spawner2");
		
		for (int i = 0; i < 4; i++) {
			ghost [i] = (GameObject)Instantiate (ghostPrefab,
			                                     transform.position + new Vector3 (i, 10, 0),
			                                     Quaternion.identity);
		}
		
		blockGrid = new Grid (5, 25, 5);
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

	public void setBlock(GameObject block){
		currentBlock = (GameObject)Instantiate (block, spawner.transform.position, spawner.transform.rotation);
	}

}

