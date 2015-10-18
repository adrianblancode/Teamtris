using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseController : MonoBehaviour {
	// WARNING This disables the wiimote for debugging
	// Fixes crashes upon going into the editor
	protected bool ENABLE_WIIMOTE = false;
	
	// Wiimote controller
	protected WiimoteReceiver receiver = null;
	
	// The wii controllers for this team
	protected Wiimote player1;
	
	// Gameboard for this script
	protected GameObject gameBoard;
	
	// Time since last gravity tick
	protected float lastFall = 0;
	
	// Rate in seconds between each natural fall of the block
	protected float fallRate = 1.6f;
	protected float fallRateMultiplier = 1.0f;
	
	// Rate in seconds between each fastfall of the block
	protected float fastFallRate = 0.05f;
	
	// Rate in seconds a block can be rotated
	protected float rotateRate = 0.4f;
	
	// Rate in seconds a block can be moved horizontally
	protected float horizontalRate = 0.2f;
	
	protected Grid blockGrid;
	protected GameObject currentBlock;

	// Ghost objects
	public GameObject ghostPrefab;
	public GameObject[] ghost;
//	public ParticleSystem effect;
	
//	protected Spawner spawner;
	
	protected bool move, rotate, fall, spawn, game_over = false;
	
	// Time in seconds it takes for each speedup
	protected int speedUpRate = 30;
	
	// Time in seconds last speedup occurred at
	protected int lastSpeedUp = 0;
	protected float speedUpMultiplier = 0.92f;
	
	// Level and its display
	protected int level = 1;
	
	// Number of lines deleted in the level and its display
	protected int lineCount = 0;
	
	// Global score and its display
	protected int score = 0;
	public Text speedUpText;
	public Text scoreText;
	
	// Number of lines deleted each time a block falls down (ranges 0..4)
	protected int linesDeleted;
	
	// Number of times in a row that 4 lines where deleted at the same time
	protected int combo = 1;
	
	// Need a team atm
	protected int team, other_team;
	
//	protected BaseController other_controller;
	public ControllerInterface ci;

	protected virtual void Update() {
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			game_over = true;
			Debug.Log("GAME OVER");
			Destroy (currentBlock);
			Destroy(this);
		}
		
		
		// TODO(Douglas): Clean up button checking for wiimotes.
		// Move Left
		if(ci.MoveLeft(team) && !move){
			move = true;
			StartCoroutine("MoveLeftX");
		}
		
		// Move Right
		else if(ci.MoveRight(team) && !move){
			move = true;
			StartCoroutine ("MoveRightX");
		}
		
		// Rotate Left
		else if(ci.RotLeft(team) && !rotate){
			rotate = true;
			StartCoroutine("RotateLeftX");
		}
		
		// Rotate Left
		else if(ci.RotRight(team) && !rotate){
			rotate = true;
			StartCoroutine("RotateRightX");
		}
		
		if(ci.MoveLeft(other_team) && !move){
			move = true;
			StartCoroutine ("MoveLeftZ");
		}
		
		else if(ci.MoveRight(other_team) && !move){
			move = true;
			StartCoroutine ("MoveRightZ");
		}
		
		else if(ci.RotLeft(other_team) && !rotate){
			rotate = true;
			StartCoroutine ("RotateLeftZ");
		}
		
		else if(ci.RotRight(other_team) && !rotate){
			rotate = true;
			StartCoroutine ("RotateRightZ");
		}

		if((ci.MoveDownCombined() ||
		    Time.time - lastFall >= fallRate * fallRateMultiplier) && !fall){
			fall = true;
			StartCoroutine ("Fall");
		}
		
		applyTransparency();
		speedUp();
	}

	protected virtual void Awake(){
	}

	protected virtual void Start(){
	}

	protected virtual void FixedUpdate(){
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
	protected virtual IEnumerator Fall(){
		// Modify position
		currentBlock.transform.position += new Vector3(0, -1, 0);
		
		// See if valid
		if (isValidGridPos()) {
			// It's valid. Update grid.
			updateGrid();
		} else {
			// It's not valid. revert.
			currentBlock.transform.position += new Vector3(0, 1, 0);

			linesDeleted =  blockGrid.deleteFullPlans();
			updateScores(linesDeleted);
		}
		lastFall = Time.time;
		yield return new WaitForSeconds(fastFallRate);
		fall = false;
	}
	
	// Updating the grid with new positions
	protected void updateGrid() {
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
			//child.parent = currentBlock.transform;
			blockGrid.getGrid ((int)v.z)[(int)v.x, (int)v.y] = child;
		}
		
		// Update ghost
		updateGhost();
	}
	
	// Checks if the current block is in a valid grid position
	protected bool isValidGridPos() {
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

	/*
	 * Update the position of the ghost blocks.
	 * If there is a free spot where the ghost should be, 
	 * place one of the 4 blocks, otherwise disable its renderer.
	 */
	protected void updateGhost() {
		int gap = 25;
		int newGap = 25;
		//		int block = 0;
		
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

	// Applies transparency to all blocks that are behind the current block
	protected void applyTransparency() {
		int nearestZ = getNearestCurrentBlockZPos ();
		
		disableTransparency ();
		
		// Apply transparency to all blocks in front of nearest
		for (int z = 0; z < nearestZ; z++) {
			Transform[,] grid = blockGrid.getGrid (z);
			for (int y = 0; y < blockGrid.getHeight(); ++y) {
				for (int x = 0; x < blockGrid.getWidth(); ++x) {
					if(grid[x, y] != null && grid[x, y].parent != currentBlock.transform){
						foreach(Transform childBlock in grid[x, y].parent){
							if(Mathf.Abs(childBlock.position.z - grid[x, y].position.z) < 0.1f){
								Renderer r = childBlock.GetComponent<Renderer>();
								Color newColor = r.material.color;

								float distance = ((float) nearestZ - z);
								newColor.a = 0.8f - 0.25f * distance;
								childBlock.GetComponent<Renderer>().material.color = newColor;
							}
						}
					}
				}
			}
		}
	}
	
	// Returns the the nearest grid position on the Z-axis of a current block
	int getNearestCurrentBlockZPos(){
		
		int transparencyZ = 999;
		
		for (int z = 0; z < blockGrid.getDepth(); z++) {
			Transform[,] grid = blockGrid.getGrid (z);
			for (int y = 0; y < blockGrid.getHeight(); ++y) {
				for (int x = 0; x < blockGrid.getWidth(); ++x){
					if(grid[x, y] != null && grid[x, y].parent == currentBlock.transform){
						if(z < transparencyZ){
							transparencyZ = z;
						}
					}
				}
			}
		}
		
		if (transparencyZ >= 999) {
			return 0;
		}
		
		return transparencyZ;
	}
	
	void disableTransparency(){
		disableTransparency (0);
	}
	
	// Disable transparency at all blocks at depth z and higher
	void disableTransparency(int depth){
		
		for (int y = 0; y < blockGrid.getHeight(); ++y) {
			for (int x = 0; x < blockGrid.getWidth(); ++x) {
				for (int z = depth; z < blockGrid.getDepth(); ++z) {
					
					Transform[,] grid = blockGrid.getGrid(z);
					if (grid[x, y] != null) {
						Transform p = grid[x, y].parent;
						
						foreach(Transform childBlock in p){
							Renderer r = childBlock.GetComponent<Renderer>();
							Color newColor = r.material.color;
							newColor.a = 1.0f;
							r.material.color = newColor;
						}
					}
				}
			}
		}
	}

	// Displays the new score, level and number of lines to go till next level
	void updateTexts() {
		if(scoreText != null)
			scoreText.text = score.ToString();
	}
	
	// Updates the score depending of the number of lines deleted with the last fallen block
	protected void updateScores(int linesDeleted) {
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
	
	// Every speedUpRate seconds, increases speed
	protected virtual void speedUp(){
		int currentTime = (int) Time.timeSinceLevelLoad;
		
		// We check that we have waited speedUpRate time
		if ((currentTime - lastSpeedUp) / speedUpRate >= 1) {
			fallRateMultiplier *= speedUpMultiplier;
			lastSpeedUp = currentTime;
		}
	}
	
	void OnApplicationQuit(){
		if(receiver != null){
			receiver.disconnect ();
		}
	}
	
}