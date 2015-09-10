using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	// The Grid itself
	private int w;
	private int h;
	public Transform[,] grid;

	public Grid(int width, int height) {
		w = width;
		h = height;
		grid = new Transform[w, h];
	}

	public int getHeight(){
		return h;
	}

	public int getWidth(){
		return w;
	}

	/*
	 * Deletes one row of full blocks,
	 * helper function for deleteFullRows
	 */
	private void deleteRow(int y) {
		for (int x = 0; x < w; ++x) {
			Destroy(grid[x, y].gameObject);
			grid[x, y] = null;
		}
	}

	/*
	 * Moves all rows above 'y' one step downwards,
	 * helper function for deleteFullRows and decreaseRowsAbove
	 */
	private void decreaseRow(int y) {
		for (int x = 0; x < w; ++x) {
			if (grid[x, y] != null) {
				// Move one towards bottom
				grid[x, y-1] = grid[x, y];
				grid[x, y] = null;
				
				// Update Block position
				grid[x, y-1].position += new Vector3(0, -1, 0);
			}
		}
	}

	/*
	 * Decreases all rows above y
	 */
	private void decreaseRowsAbove(int y) {
		for (int i = y; i < h; ++i)
			decreaseRow(i);
	}

	/*
	 * Checks whether row 'y' is full or not
	 */
	private bool isRowFull(int y) {
		for (int x = 0; x < w; ++x)
			if (grid[x, y] == null)
				return false;
		return true;
	}

	public Vector2 roundVec2(Vector2 v){
		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
	}

	/*
	 * Checks if the given Vector2 is within the gameboards border
	 * (Currently hardcoded borders)
	 * TODO: Make it dynamic so it calculates based on the gameboards position
	 */
	public bool insideBorder(Vector2 pos){
		Vector2 rounded_pos = roundVec2 (pos);
		return (rounded_pos.x >= 0 &&
		        rounded_pos.x < w &&
		        rounded_pos.y >= 0);
	}

	/*
	 * Goes through all rows and deletes the full ones,
	 * then moves all the rows above one step down
	 * called after each discrete timestep
	 * @return The number of deleted rows.
	 */
	public int deleteFullRows() {
		int linesDeleted = 0;
		for (int y = 0; y < h; ++y) {
			if (isRowFull(y)) {
				linesDeleted++;
				deleteRow(y);
				decreaseRowsAbove(y+1);
				--y;
			}
		}
		return linesDeleted;
	}
}