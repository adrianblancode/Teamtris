using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockController : MonoBehaviour {

	// Which team owns this blockcontroller
	// TODO make work for both teams
	private int team = 1;

	// Time since last gravity tick
	private float lastFall = 0;

	// Rate in seconds between each natural fall of the block
	private float fallRate = 0.75f;
	private float fallRateMultiplier = 1.0f;

	// Rate in seconds between each fastfall of the block
	private float fastFallRate = 0.05f;

	// Rate in seconds a block can be rotated
	private float rotateRate = 0.4f;

	// Rate in seconds a block can be moved horizontally
	private float horizontalRate = 0.2f;

	private Grid blockGrid;
	public GameObject currentBlock;
	private Spawner spawner;

	private bool left, right, rotate, fall = false;

	// Level and its display
	private int level = 1;
	public Text levelText;

	// Number of lines deleted in the level and its display
	private int lineCount = 0;
	public Text countText;

	// Global score and its display
	private int score = 0;
	public Text scoreText;

	// Number of lines deleted each time a block falls down (ranges 0..4)
	private int linesDeleted;

	// Number of times in a row that 4 lines where deleted at the same time
	private int combo = 1;
	
	void Start () {
		spawner = FindObjectOfType<Spawner> ();
		blockGrid = new Grid (10, 25);

		currentBlock = spawner.spawnNext();

		updateTexts();
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

		// Move Left
		if (ControllerInterface.MoveLeft(team) && !left) {
			left = true;
			StartCoroutine("MoveLeft");
		}

		// Move Right
		else if (ControllerInterface.MoveRight(team) && !right) {
			right = true;
			StartCoroutine("MoveRight");
		}

		// Rotate Left
		else if (ControllerInterface.RotLeft(team) && !rotate) {
			rotate = true;
			StartCoroutine("RotateLeft");
		}

		// Rotate Left
		else if (ControllerInterface.RotRight(team) && !rotate) {
			rotate = true;
			StartCoroutine("RotateRight");
		}

		// Move Downwards and Fall
		else if ((ControllerInterface.ActionButtonCombined(1) ||
		         Time.time - lastFall >= fallRate * fallRateMultiplier) && !fall) {
			fall = true;
			StartCoroutine("Fall");
		}
	}

	// CoRoutine for moving left
	IEnumerator MoveLeft(){
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

	// CoRoutine for moving right
	IEnumerator MoveRight(){
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

	// CoRoutine for rotating
	IEnumerator RotateLeft(){
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

	// CoRoutine for rotating
	IEnumerator RotateRight(){
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
			// Clear filled horizontal lines
			linesDeleted = blockGrid.deleteFullRows();
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
		for (int y = 0; y < blockGrid.getHeight(); ++y)
			for (int x = 0; x < blockGrid.getWidth(); ++x)
				if (blockGrid.grid[x, y] != null)
					if (blockGrid.grid[x, y].parent == currentBlock.transform)
						blockGrid.grid[x, y] = null;
		
		// Add new children to grid
		foreach (Transform child in currentBlock.transform) {
			Vector2 v = blockGrid.roundVec2(child.position);
			blockGrid.grid[(int)v.x, (int)v.y] = child;
		}        
	}

	// Checks if the current block is in a valid grid position
	bool isValidGridPos() {        
		foreach (Transform child in currentBlock.transform) {
			Vector2 v = blockGrid.roundVec2(child.position);
			
			// Not inside Border?
			if (!blockGrid.insideBorder(v))
				return false;
			
			// Block in grid cell (and not part of same group)?
			Debug.Log (v);
			if (blockGrid.grid[(int)v.x, (int)v.y] != null &&
			    blockGrid.grid[(int)v.x, (int)v.y].parent != currentBlock.transform)
				return false;
		}
		return true;
	}

	// Displays the new score, level and number of lines to go till next level
	void updateTexts() {
		levelText.text = "Level : " + level.ToString();
		countText.text = "Lines to go : " + (10 - lineCount).ToString();
		scoreText.text = "Score : " + score.ToString();
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
}


