using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {

	// Time since last gravity tick
	float lastFall = 0;
	float fallTime = 1.0f;
	private Grid blockGrid;
	public GameObject currentBlock;

	private bool left, right, rotate, fall = false;
	private float rotateRate = 0.1f;
	private float horizontalRate = 0.2f;
	private float fallRate = 0.2f;

	// Use this for initialization
	void Start () {
		blockGrid = new Grid (10, 25);

		currentBlock = FindObjectOfType<Spawner>().spawnNext();

	}

	public void setRotateRate(float r){
		rotateRate = r;
	}

	public void setHorizontalRate(float h){
		horizontalRate = h;
	}

	public void setFallRate(float f){
		fallRate = f;
	}

	public void setFallTime(float f){
		fallTime = f;
	}
	// Update is called once per frame
	void Update() {
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			Debug.Log("GAME OVER");
			Destroy(this);
		}

		// Move Left
		if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftArrow)) && !left) {
			left = true;
			StartCoroutine("MoveLeft");
		}

		// Move Right
		else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow)) && !right) {
			right = true;
			StartCoroutine("MoveRight");
		}
		// Rotate
		else if (Input.GetKeyDown(KeyCode.UpArrow) && !rotate) {
			rotate = true;
			StartCoroutine("Rotate");
		}
		// Move Downwards and Fall
		else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey (KeyCode.DownArrow) ||
		         Time.time - lastFall >= fallTime) && !fall) {
			fall = true;
			StartCoroutine("Fall");
		}
	}

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
	
	IEnumerator Rotate(){
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
			blockGrid.deleteFullRows();
			// Spawn next Group
			currentBlock = FindObjectOfType<Spawner>().spawnNext();
		}
		lastFall = Time.time;
		yield return new WaitForSeconds(fallRate);
		fall = false;
	}
	
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
}
