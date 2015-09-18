using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockController : MonoBehaviour {

	// WARNING This disables the wiimote for debugging
	// Fixes crashes upon going into the editor
	private bool ENABLE_WIIMOTE = false;

	// Which player owns this blockcontroller
	// TODO make work for both teams
	public int player = 1;

	// The other player on the team
	private int otherPlayer = 2;

	// Wiimote controller
	private WiimoteReceiver receiver = null;

	// The wii controllers for this team
	private Wiimote player1;

	// Gameboard for this script
	private GameObject gameBoard;

	// Time since last gravity tick
	private float lastFall = 0;

	// Rate in seconds between each natural fall of the block
	private float fallRate = 0.5f;
	private float fallRateMultiplier = 1.0f;

	// Rate in seconds between each fastfall of the block
	private float fastFallRate = 0.05f;

	// Rate in seconds a block can be rotated
	private float rotateRate = 0.4f;

	// Rate in seconds a block can be moved horizontally
	private float horizontalRate = 0.2f;

	private Grid blockGrid;
	public GameObject currentBlock;
	public ParticleSystem effect;
	public Spawner spawner;

	private bool left, right, rotate, fall = false;
	private bool otherPlayerMove, otherPlayerRotate, otherPlayerFall = false;

	// Level and its display
	private int level = 1;

	// Number of lines deleted in the level and its display
	private int lineCount = 0;

	// Global score and its display
	private int score = 0;
	public Text scoreText;

	// Number of lines deleted each time a block falls down (ranges 0..4)
	private int linesDeleted;

	// Number of times in a row that 4 lines where deleted at the same time
	private int combo = 1;

	void Start () {

		// Set who the other player is
		if (player == 2) {
			otherPlayer = 1;
		}

		if (player == 1) {
			gameBoard = GameObject.FindGameObjectWithTag ("Player1_GameBoard");
		} else {
			gameBoard = GameObject.FindGameObjectWithTag ("Player2_GameBoard");
		}

		GameObject block = spawner.getNext();
	
		//TODO update for small board
		//Vector3 padding = new Vector3 (1, 0, -1);

		currentBlock = (GameObject)Instantiate (block,
			                                    transform.position,
			                                    Quaternion.identity);

		if (player == 2) {
			//initializePosition();
		}

		blockGrid = new Grid (10, 25, 10);

//		effect = (ParticleSystem)Instantiate(effect,
//		                                     transform.position,
//		                                     Quaternion.identity);
//		effect.transform.Rotate(-170, 0, 0);
//		effect.Stop();

		updateTexts();

		// Initialize wiimote receiver
		// TODO(Douglas): Make this work for multiple controllers (if needed)
		if (ENABLE_WIIMOTE) {
			receiver = WiimoteReceiver.Instance;
			receiver.connect ();

			// Create a dummy wiimote to avoid the NullReferenceException in Update()
			player1 = new Wiimote ();
		}
	}

	public void initializePosition(){
		for (int i = 0; i < 3; i++) {
			StartCoroutine ("MoveRightX");
			StartCoroutine ("MoveRightZ");
		}

		StartCoroutine("RotateLeftY");
	}

	// Set rate at which user is able to rotate
	public void setRotateRate(float r){
		rotateRate = r;
	}

	// Set rate at which user is able to move horizontally
	public void setHorizontalRate(float h){
		horizontalRate = h;
	}

	// Set the rate at which the blocks fall naturally
	public void setFallRateMultiplier(float f){
		fallRateMultiplier = f;
	}

	// Set rate at which user is able to make blocks fastfall
	public void setFastFallRate(float f){
		fastFallRate = f;
	}

	void Update() {
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			Debug.Log("GAME OVER");
			Destroy(this);
		}

		// Grab the wiimote
		if (receiver != null && receiver.wiimotes.ContainsKey (1)) {
			player1 = (Wiimote)receiver.wiimotes [1];
		}


		// TODO(Douglas): Clean up button checking for wiimotes.
		// Move Left
		if ((ControllerInterface.MoveLeft (player)) && !left) {
			left = true;
			StartCoroutine ("MoveLeftX");
		}

		// Move Right
		else if (ControllerInterface.MoveRight (player) && !right) {
			right = true;
			StartCoroutine("MoveRightX");
		}

		// Rotate Left
		else if (ControllerInterface.RotLeft (player) && !rotate) {
			rotate = true;
			StartCoroutine("RotateLeftX");
		}

		// Rotate Left
		else if (ControllerInterface.RotRight (player) && !rotate) {
			rotate = true;
			StartCoroutine("RotateRightX");
		}

		// Move Downwards and Fall
		else if (ControllerInterface.ActionButtonCombined (1) ||
			Time.time - lastFall >= fallRate * fallRateMultiplier && !fall) {
			fall = true;
			StartCoroutine ("Fall");
		}

		updateOtherPlayer();
	}

	public void updateOtherPlayer(){
		// Rotate Left
		if (player == 1) {
			if ((ControllerInterface.MoveLeft (otherPlayer)) && !otherPlayerMove) {
				otherPlayerMove = true;
				StartCoroutine ("MoveLeftZ");
			} else if ((ControllerInterface.MoveRight (otherPlayer)) && !otherPlayerMove) {
				otherPlayerMove = true;
				StartCoroutine ("MoveRightZ");
			} else if (ControllerInterface.RotLeft (otherPlayer) && !otherPlayerRotate) {
				otherPlayerRotate = true;
				StartCoroutine("RotateLeftZ");
			} else if (ControllerInterface.RotRight (otherPlayer) && !otherPlayerRotate) {
				otherPlayerRotate = true;
				StartCoroutine("RotateRightZ");
			}
		} else {
			if ((ControllerInterface.MoveLeft (otherPlayer)) && !otherPlayerMove) {
				otherPlayerMove = true;
				StartCoroutine ("MoveRightZ");
			} else if ((ControllerInterface.MoveRight (otherPlayer)) && !otherPlayerMove) {
				otherPlayerMove = true;
				StartCoroutine ("MoveLeftZ");
			} else if (ControllerInterface.RotLeft (otherPlayer) && !otherPlayerRotate) {
				otherPlayerRotate = true;
				StartCoroutine("RotateLeftX");
			} else if (ControllerInterface.RotRight (otherPlayer) && !otherPlayerRotate) {
				otherPlayerRotate = true;
				StartCoroutine("RotateRightX");
			}
		} 
	}

	// CoRoutine for moving left on the x-axis
	IEnumerator MoveLeftX(){
		// Modify position
		currentBlock.transform.position += new Vector3(-1, 0, 0);
		// See if valid
		if (isValidGridPos ()) {
			// Its valid. Update grid.
			updateGrid ();
		} else {
			// Its not valid. revert.
			currentBlock.transform.position += new Vector3 (1, 0, 0);
		}
		yield return new WaitForSeconds(horizontalRate);
		left = false;
	}

	// CoRoutine for moving right on the x-axis
	IEnumerator MoveRightX(){
		// Modify position
		currentBlock.transform.position += new Vector3(1, 0, 0);

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.position += new Vector3 (-1, 0, 0);
		}
		yield return new WaitForSeconds(horizontalRate);
		right = false;
	}

	// CoRoutine for rotating left around the x-axis
	IEnumerator RotateLeftX(){
		if(currentBlock.tag != "freeze"){
			currentBlock.transform.Rotate(0, 0, -90);
		}

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (0, 0, 90);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
	}

	// CoRoutine for rotating left around the x-axis
	IEnumerator RotateLeftY(){
		if(currentBlock.tag != "freeze"){
			currentBlock.transform.Rotate(0, -90, 0);
		}
		
		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (0, 90, 0);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
	}

	// CoRoutine for rotating right around the x-axis
	IEnumerator RotateRightX(){
		if(currentBlock.tag != "freeze"){
			currentBlock.transform.Rotate(0, 0, 90);
		}

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (0, 0, -90);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
	}

	// CoRoutine for rotating right around the x-axis
	IEnumerator RotateRightY(){
		if(currentBlock.tag != "freeze"){
			currentBlock.transform.Rotate(0, 90, 0);
		}
		
		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (0, -90, 0);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
	}

	// CoRoutine for moving left on the z-axis
	IEnumerator MoveLeftZ(){
		// Modify position
		currentBlock.transform.position += new Vector3(0, 0, -1);
		// See if valid
		if (isValidGridPos ()) {
			// Its valid. Update grid.
			updateGrid ();
		} else {
			// Its not valid. revert.
			currentBlock.transform.position += new Vector3 (0, 0, 1);
		}
		yield return new WaitForSeconds(horizontalRate);
		otherPlayerMove = false;
	}

	// CoRoutine for moving right on the z-axis
	IEnumerator MoveRightZ(){
		// Modify position
		currentBlock.transform.position += new Vector3(0, 0, 1);
		// See if valid
		if (isValidGridPos ()) {
			// Its valid. Update grid.
			updateGrid ();
		} else {
			// Its not valid. revert.
			currentBlock.transform.position += new Vector3 (0, 0, -1);
		}
		yield return new WaitForSeconds(horizontalRate);
		otherPlayerMove = false;
	}

	// CoRoutine for rotating left around the z-axis
	IEnumerator RotateLeftZ(){
		if(currentBlock.tag != "freeze"){
			currentBlock.transform.Rotate(-90, 0, 0);
		}
		
		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (90, 0, 0);
		}
		yield return new WaitForSeconds(rotateRate);
		otherPlayerRotate = false;
	}

	// CoRoutine for moving right around the z-axis
	IEnumerator RotateRightZ(){
		if(currentBlock.tag != "freeze"){
			currentBlock.transform.Rotate(90, 0, 0);
		}
		
		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (-90, 0, 0);
		}
		yield return new WaitForSeconds(rotateRate);
		otherPlayerRotate = false;
	}

	// CoRoutine for making pieces fall
	IEnumerator Fall(){
		// Modify position
		currentBlock.transform.position += new Vector3(0, -1, 0);

		// See if valid
		if (isValidGridPos()) {
			// It's valid. Update grid.
			updateGrid();
		} else {
			// It's not valid. revert.
			currentBlock.transform.position += new Vector3(0, 1, 0);
			// Play the explosion effect
//			effect.transform.position = currentBlock.transform.position;
//			effect.Play();
			// Clear filled horizontal lines
//			linesDeleted = blockGrid.deleteFullRows();
			linesDeleted = 0;
			// Update the scores depending on the number of lines deleted
			updateScores(linesDeleted);

			// Spawn next Group
			GameObject block = spawner.getNext();


			//TODO update for small board
			Vector3 padding = new Vector3 (2, 0, 2);

			currentBlock = (GameObject)Instantiate(block,
			                                       transform.position + padding,
			                                       Quaternion.identity);

			if (player == 2) {
				initializePosition();
			}
		}
		lastFall = Time.time;
		yield return new WaitForSeconds(fastFallRate);
		fall = false;
		otherPlayerFall = false;
	}

	// Updating the grid with new positions
	void updateGrid() {
		// Remove old children from grid
		for (int y = 0; y < blockGrid.getHeight(); ++y) {
			for (int x = 0; x < blockGrid.getWidth(); ++x) {
				for (int z = 0; z < blockGrid.getDepth(); ++z) {
					Transform[,] grid = blockGrid.getGrid(z);
					if (grid[x, y] != null) {
						if (grid[x, y].parent == currentBlock.transform) {
							grid[x, y] = null;
						}
					}
				}
			}
		}

		// Add new children to grid
		foreach (Transform child in currentBlock.transform) {
			// Offset the position with the gameboards position
			Vector3 temp = new Vector3(child.position.x - gameBoard.transform.position.x, child.position.y - gameBoard.transform.position.y, child.position.z);
			Vector3 v = blockGrid.roundVec3(temp);

			blockGrid.getGrid ((int)v.z)[(int)v.x, (int)v.y] = child;
		}
	}

	// Checks if the current block is in a valid grid position
	bool isValidGridPos() {
		foreach (Transform child in currentBlock.transform) {
			// Offset the position with the gameboards position
			Vector3 temp = new Vector3(child.position.x - gameBoard.transform.position.x, child.position.y - gameBoard.transform.position.y, child.position.z);
			Vector3 v = blockGrid.roundVec3(temp);

			// Not inside Border?
			if (!blockGrid.insideBorder(v))
				return false;

			// Block in grid cell (and not part of same group)?
			Transform[,] grid = blockGrid.getGrid ((int)v.z);
			if (grid[(int)v.x, (int)v.y] != null &&
			    grid[(int)v.x, (int)v.y].parent != currentBlock.transform)
				return false;
		}
		return true;
	}

	// Displays the new score, level and number of lines to go till next level
	void updateTexts() {
		scoreText.text = score.ToString();
	}

	// Updates the score depending of the number of lines deleted with the last fallen block
	void updateScores(int linesDeleted) {
		// Increase the score and change the combo
		switch (linesDeleted) {
			case 1 :
				score += 40*level;
				combo = 1;
				break;
			case 2 :
				score += 100*level;
				combo = 1;
				break;
			case 3 :
				score += 300*level;
				combo = 1;
				break;
			case 4 :
				score += 1200*level*combo;
				combo++;
				break;
			default :
				// If no line has been deleted, the score and combo do not change.
				break;
		}

		// Increase this level's line count
		lineCount += linesDeleted;

		// If the player achieved the goal (10 lines)
		// then level up and count set back to 0
		if (lineCount + linesDeleted > 9) {
			lineCount = lineCount % 10;
			level++;
		}

		// Update of the displays
		updateTexts();
	}

	void OnApplicationQuit(){
		if(receiver != null){
			receiver.disconnect ();
		}
	}
}