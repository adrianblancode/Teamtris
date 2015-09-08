using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	// The Grid itself
	public static int w = 10;
	public static int h = 20;
	public static Transform[,] grid = new Transform[w, h];
	public GameObject leftBorder = GameObject.Find("LeftBorder");
	public GameObject rightBorder = GameObject.Find("RightBorder");
	public GameObject bottom = GameObject.Find ("Bottom");


	public Vector2 roundVec2(Vector2 v){
		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
	}

	public bool insideBorder(Vector2 pos){

		Vector2 rounded_pos = roundVec2 (pos);
		return (rounded_pos.x >= (leftBorder.transform.position.x + 0.5) &&
		        rounded_pos.x <= (rightBorder.transform.position.x - 0.5) &&
		        rounded_pos.y >= (bottom.transform.position.y + 0.5));
	}

	public static void deleteRow(int y) {
		for (int x = 0; x < w; ++x) {
			Destroy(grid[x, y].gameObject);
			grid[x, y] = null;
		}
	}

	public static void decreaseRow(int y) {
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

	public static void decreaseRowsAbove(int y) {
		for (int i = y; i < h; ++i)
			decreaseRow(i);
	}
}