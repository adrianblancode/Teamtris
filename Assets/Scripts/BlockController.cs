using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {

	// Time since last gravity tick
	float lastFall = 0;
	private Grid blockGrid;
	public GameObject currentBlock;
	// Use this for initialization
	void Start () {
		blockGrid = new Grid (10, 25);

		currentBlock = FindObjectOfType<Spawner>().spawnNext();

	}
	
	// Update is called once per frame
	void Update() {
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			Debug.Log("GAME OVER");
			Destroy(gameObject);
		}

		// Move Left
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			// Modify position
			currentBlock.transform.position += new Vector3(-1, 0, 0);
			
			// See if valid
			if (isValidGridPos())
				// Its valid. Update grid.
				updateGrid();
			else
				// Its not valid. revert.
				currentBlock.transform.position += new Vector3(1, 0, 0);
		}

		// Move Right
		else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			// Modify position
			currentBlock.transform.position += new Vector3(1, 0, 0);
			
			// See if valid
			if (isValidGridPos())
				// It's valid. Update grid.
				updateGrid();
			else
				// It's not valid. revert.
				currentBlock.transform.position += new Vector3(-1, 0, 0);
		}
		// Rotate
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			if(currentBlock.tag != "freeze"){
				currentBlock.transform.Rotate(0, 0, -90);
			}
			
			// See if valid
			if (isValidGridPos())
				// It's valid. Update grid.
				updateGrid();
			else
				// It's not valid. revert.
				currentBlock.transform.Rotate(0, 0, 90);
		}
		// Move Downwards and Fall
		else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey (KeyCode.DownArrow) ||
		         Time.time - lastFall >= 1) {
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
		}
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
