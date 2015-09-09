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

	private void deleteRow(int y) {
		for (int x = 0; x < w; ++x) {
			Destroy(grid[x, y].gameObject);
			grid[x, y] = null;
		}
	}

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

	private void decreaseRowsAbove(int y) {
		for (int i = y; i < h; ++i)
			decreaseRow(i);
	}

	private bool isRowFull(int y) {
		for (int x = 0; x < w; ++x)
			if (grid[x, y] == null)
				return false;
		return true;
	}

	public Vector2 roundVec2(Vector2 v){
		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
	}
	public bool insideBorder(Vector2 pos){
		Vector2 rounded_pos = roundVec2 (pos);
		return (rounded_pos.x >= 0 &&
		        rounded_pos.x < w &&
		        rounded_pos.y >= 0);
	}

	public void deleteFullRows() {
		for (int y = 0; y < h; ++y) {
			if (isRowFull(y)) {
				deleteRow(y);
				decreaseRowsAbove(y+1);
				--y;
			}
		}
	}
}