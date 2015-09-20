﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockController : MonoBehaviour {

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
	public GameObject ghostPrefab;
	public GameObject[] ghost;
	public ParticleSystem effect;
	private Spawner spawner;

	private bool left, right, rotate, fall = false;

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
		if (team == 1) {
			gameBoard = GameObject.FindGameObjectWithTag ("Team1_GameBoard");
		} else {
			gameBoard = GameObject.FindGameObjectWithTag ("Team2_GameBoard");
		}

		spawner = FindObjectOfType<Spawner> ();
		currentBlock = spawner.spawnNext();

		for (int i = 0; i < 4; i++) {
			ghost[i] = (GameObject)Instantiate(	ghostPrefab,
			           							transform.position + new Vector3(i, 0, 0),
			                                  	Quaternion.identity);
		}

		blockGrid = new Grid (10, 25, 10);

//		effect = (ParticleSystem)Instantiate(effect,
//		                                     transform.position,
//		                                     Quaternion.identity);
//		effect.transform.Rotate(-170, 0, 0);
//		effect.Stop();
		
		updateGhost();
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
		if ((ControllerInterface.MoveLeft (team)) && !left) {
			left = true;
//			StartCoroutine ("MoveLeftX");
			StartCoroutine ("MoveLeftZ");
		}

		// Move Right
		else if (ControllerInterface.MoveRight (team) && !right) {
			right = true;
//			StartCoroutine("MoveRightX");
			StartCoroutine ("MoveRightZ");
		}

		// Rotate Left
		else if (ControllerInterface.RotLeft (team) && !rotate) {
			rotate = true;
//			StartCoroutine("RotateLeftX");
			StartCoroutine ("RotateLeftZ");
		}

		// Rotate Left
		else if (ControllerInterface.RotRight (team) && !rotate) {
			rotate = true;
//			StartCoroutine("RotateRightX");
			StartCoroutine ("RotateRightZ");
		}

		// Move Downwards and Fall
		else if (ControllerInterface.ActionButtonCombined (1) ||
			Time.time - lastFall >= fallRate * fallRateMultiplier && !fall) {
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
		left = false;
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
		right = false;
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
		rotate = false;
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
//			linesDeleted = blockGrid.deleteFullRows();
			linesDeleted = 0;
			// Update the scores depending on the number of lines deleted
			updateScores(linesDeleted);
			// Spawn next Group
			currentBlock = spawner.spawnNext();
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
			Vector3 temp = new Vector3(child.position.x - gameBoard.transform.position.x, child.position.y - gameBoard.transform.position.y, child.position.z);
			Vector3 v = blockGrid.roundVec3(temp);

			blockGrid.getGrid ((int)v.z)[(int)v.x, (int)v.y] = child;
		}

		// Update ghost
		updateGhost();
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

	/*
	 * Update the position of the ghost blocks.
	 * If there is a free spot where the ghost should be, 
	 * place one of the 4 blocks, otherwise disable its renderer.
	 */
	void updateGhost() {
		int gap = 25;
		int newGap = 25;
		int block = 0;
		
		for (int i = 0; i < 4; i++) {
			ghost[i].GetComponent<MeshRenderer>().enabled = false;
		}

		// Consider each block of the current tetrimino
		foreach (Transform child in currentBlock.transform) {
			int x = (int)(child.transform.position.x - gameBoard.transform.position.x);
			int y = (int)(child.transform.position.y - gameBoard.transform.position.y);
			int z = (int)(child.transform.position.z);
			Transform[,] grid = blockGrid.getGrid(z);

			// If the block is at the bottom of the grid
			if (y == 0) {
				gap = 0;
				break;
			} else if (y == 1) {
				gap = 1;
				continue;

			// If there is nothing beneath the current child
			} else if (grid[x, y-1] == null) {
				y--;
				newGap = 1;
				while (grid[x, y-1] == null) {
					newGap++;
					y--;
					if (y < 1) break;
				}
				if (newGap < gap) {
					gap = newGap;
				}

			// If there is another block that is not part of the tetrimino then the block is at the bottom
			} else if (grid[x, y-1] != null && grid[x, y-1].parent != currentBlock.transform) {
				gap = 0;
				break;
			
			// If there is another block of the current tetrimino, do nothing
			} else {
				continue;
			}
		}

		int ghostblock = 0;
		foreach (Transform child in currentBlock.transform) {
			int x = (int)(child.transform.position.x - gameBoard.transform.position.x);
			int y = (int)(child.transform.position.y - gameBoard.transform.position.y);
			int z = (int)(child.transform.position.z);
			Transform[,] grid = blockGrid.getGrid(z);

			// If the space is free, place the ghost block
			if (grid[x, y-gap] == null) {
				ghost[ghostblock].GetComponent<MeshRenderer>().enabled = true;
				ghost[ghostblock].transform.position = child.transform.position + new Vector3(0, -gap, 0);

			// Else there is a tetriminos block, so disable the rendering of the ghost block
			} else {
				ghost[ghostblock].GetComponent<MeshRenderer>().enabled = false;
			}
			ghostblock++;
		}
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