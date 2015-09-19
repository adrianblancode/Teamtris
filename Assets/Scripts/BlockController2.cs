using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockController2 : MonoBehaviour {

	// WARNING This disables the wiimote for debugging
	// Fixes crashes upon going into the editor
	private bool ENABLE_WIIMOTE = true;

	// Which team owns this blockcontroller
	// TODO make work for both teams
	private int team = 1;

	// Wiimote controller
	private WiimoteReceiver receiver = null;

	// The wii controllers for this team
	private Wiimote player1;

	// Gameboard for this script
	private GameObject gameBoard;

	// Time since last gravity tick
	private float lastFall = 0;

	// Rate in seconds between each natural fall of the block
	private float fallRate = 1.5f;
	private float fallRateMultiplier = 1.0f;

	// Rate in seconds between each fastfall of the block
	private float fastFallRate = 0.05f;

	// Rate in seconds a block can be rotated
	private float rotateRate = 0.4f;

	// Rate in seconds a block can be moved horizontally
	private float horizontalRate = 0.2f;

	private Grid blockGrid;
	private GameObject currentBlock;
	public ParticleSystem effect;

	private GameObject spawner;

	private bool move, rotate, fall = false;

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

	private Component master_controller;

	void Awake () {
		master_controller = GameObject.Find ("BlockController1").GetComponent ("BlockController1");
		gameBoard = GameObject.FindGameObjectWithTag ("Player2_GameBoard");

		spawner = GameObject.FindGameObjectWithTag("Spawner2");

		blockGrid = new Grid (5, 25, 5);

		// Initialize wiimote receiver
		// TODO(Douglas): Make this work for multiple controllers (if needed)
		if (ENABLE_WIIMOTE) {
			receiver = WiimoteReceiver.Instance;
			receiver.connect ();

			// Create a dummy wiimote to avoid the NullReferenceException in Update()
			player1 = new Wiimote ();
		}
	}

//	public void setGrid(Grid g){
////		blockGrid = g;
//	}

	public void setBlock(GameObject block){
		currentBlock = (GameObject)Instantiate (block, spawner.transform.position, spawner.transform.rotation);
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
//			Destroy (currentBlock);
			Destroy(this);
		}

		// Grab the wiimote
		if (receiver != null && receiver.wiimotes.ContainsKey (1)) {
			player1 = (Wiimote)receiver.wiimotes [1];
		}

		// TODO(Douglas): Clean up button checking for wiimotes.
		// Move Left
		//		if ((ControllerInterface.MoveLeft (team)) && !left) {
		if(Input.GetKey(KeyCode.LeftArrow) && !move){
			move = true;
			StartCoroutine("MoveRightZ");
		}

		// Move Right
		//		if (ControllerInterface.MoveRight (team) && !right) {
		if(Input.GetKey(KeyCode.RightArrow) && !move){
			move = true;
			StartCoroutine ("MoveLeftZ");
		}

		// Rotate Left
		//		if (ControllerInterface.RotLeft (team) && !rotate) {
		if(Input.GetKey(KeyCode.UpArrow) && !rotate){
			rotate = true;
			StartCoroutine("RotateRightZ");
		}

		// Rotate Left
		//		if (ControllerInterface.RotRight (team) && !rotate) {
		if(Input.GetKey(KeyCode.DownArrow) && !rotate){
			rotate = true;
			StartCoroutine("RotateLeftZ");
		}

		if(Input.GetKey(KeyCode.A) && !move){
			move = true;
			StartCoroutine ("MoveLeftX");
		}

		if(Input.GetKey(KeyCode.D) && !move){
			move = true;
			StartCoroutine ("MoveRightX");
		}

		if(Input.GetKey(KeyCode.W) && !rotate){
			rotate = true;
			StartCoroutine ("RotateLeftX");
		}

		if(Input.GetKey(KeyCode.S) && !rotate){
			rotate = true;
			StartCoroutine ("RotateRightX");
		}

		// Move Downwards and Fall
		//		if (ControllerInterface.ActionButtonCombined (1) ||
		//			Time.time - lastFall >= fallRate * fallRateMultiplier && !fall) {
		if(Input.GetKey(KeyCode.Space) ||
		   Time.time - lastFall >= fallRate * fallRateMultiplier && !fall){
			fall = true;
			StartCoroutine ("Fall");
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
		move = false;
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
		move = false;
	}

	// CoRoutine for rotating left around the x-axis
	IEnumerator RotateLeftX(){
		currentBlock.transform.Rotate(0, 0, 90, Space.World);

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (0, 0, -90, Space.World);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
	}

	// CoRoutine for rotating right around the x-axis
	IEnumerator RotateRightX(){
		currentBlock.transform.Rotate(0, 0, -90, Space.World);

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (0, 0, 90, Space.World);
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
		move = false;
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
		move = false;
	}

	// CoRoutine for rotating left around the z-axis
	IEnumerator RotateLeftZ(){
		currentBlock.transform.Rotate(-90, 0, 0, Space.World);

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (90, 0, 0, Space.World);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
	}

	// CoRoutine for moving right around the z-axis
	IEnumerator RotateRightZ(){
		currentBlock.transform.Rotate(90, 0, 0, Space.World);

		// See if valid
		if (isValidGridPos ()) {
			// It's valid. Update grid.
			updateGrid ();
		} else {
			// It's not valid. revert.
			currentBlock.transform.Rotate (-90, 0, 0, Space.World);
		}
		yield return new WaitForSeconds(rotateRate);
		rotate = false;
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
			linesDeleted = blockGrid.deleteFullPlans();
			//			linesDeleted = 0;
			// Update the scores depending on the number of lines deleted
//			updateScores(linesDeleted);
			// Spawn next Group
//			currentBlock = spawner.spawnNext();
		}
		lastFall = Time.time;
		yield return new WaitForSeconds(fastFallRate);
		fall = false;
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
			Vector3 temp = child.position - gameBoard.transform.position;
			Vector3 v = blockGrid.roundVec3(temp);

			blockGrid.getGrid ((int)v.z)[(int)v.x, (int)v.y] = child;
		}
	}

	// Checks if the current block is in a valid grid position
	bool isValidGridPos() {
		foreach (Transform child in currentBlock.transform) {
			// Offset the position with the gameboards position
			Vector3 temp = child.position - gameBoard.transform.position;
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