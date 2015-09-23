using UnityEngine;
using System.Collections;

public class Grid {
	// The Grid itself. Origin (0,0,0) is
	// Left, bottom, "nearest"
	private int w; // Width
	private int h; // Heigth
	private int d; // Depth

	private ArrayList grid3d = new ArrayList();

	public Grid(int x, int y, int z) {
		w = x;
		h = y;
		d = z;

		for (int i = 0; i < d; ++i) {
			Transform[,] grid = new Transform[w,h];
			grid3d.Add(grid);
		}
	}

	public Transform[,] getGrid(int z){
		return (Transform[,])grid3d[z];
	}

	public int getHeight(){
		return h;
	}

	public int getWidth(){
		return w;
	}

	public int getDepth(){
		return d;
	}

	/*
	 * Deletes one plan of full blocks,
	 * helper function for deleteFullPlans
	 */
	private void deletePlan(int y) {
		for (int x = 0; x < w; ++x) {
			for (int z = 0; z < d; ++z) {
				if (getGrid(z)[x, y] != null) {
					MonoBehaviour.Destroy(getGrid(z)[x, y].gameObject);
					getGrid(z)[x, y] = null;
				}
			}
		}
	}
	
	/*
	 * Moves all plans above 'y' one step downwards,
	 * helper function for deleteFullPlans and decreasePlansAbove
	 */
	private void decreasePlan(int y) {
		for (int x = 0; x < w; ++x) {
			for (int z = 0; z < d; z++) {
				if (getGrid(z)[x, y] != null) {
					// Move one towards bottom
					getGrid(z)[x, y-1] = getGrid(z)[x, y];
					getGrid(z)[x, y] = null;
					// Update Block position
					getGrid(z)[x, y-1].position += new Vector3(0, -1, 0);
				}
			}
		}
	}
	
	/*
	 * Decreases all plans above y
	 */
	private void decreasePlansAbove(int y) {
		for (int i = y; i < h; ++i)
			decreasePlan(i);
	}
	
	//	/*
	//	 * Checks whether plan 'y' is full or not
	//	 */
	private bool isPlanFull(int y) {

		// Player looking through x,y into z (from front)
		for (int x = 0; x < w; ++x) {
			bool zEmpty = true;
			for (int z = 0; z < d; z++)
				if (getGrid (z) [x, y] != null)
					zEmpty = false;
			if (zEmpty) return false;
		}

		// Player looking through z,y into x (from left)
		for (int z = 0; z < d; z++) {
			bool xEmpty = true;
			for (int x = 0; x < w; ++x)
				if (getGrid (z) [x, y] != null)
					xEmpty = false;
			if (xEmpty) return false;
		}

		return true;
	}
	
	//private bool isPlanFull(int y) {
	//	for (int x = 0; x < w; ++x)
	//		for (int z = 0; z < d; z++)
	//			if (getGrid(z)[x, y] == null)
	//				return false;
	//	return true;
	//}


	//	/*
	//	 * Checks whether plan 'y' has two full sides
	//	 * from the players points of vue
	//	 */
//	private bool areSidesFull(int y) {
//		// TODO : check for the front point of vue of each player
//		for (int x = 0; x < w; ++x)
//			if (getGrid(0)[x, y] == null)
//				return false;
//		for (int z = 0; z < d; ++z)
//			if (getGrid(z)[0, y] == null)
//				return false;
//		return true;
//	}
	
	public Vector3 roundVec3(Vector3 v){
		return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round (v.z));
	}

	/*
	 * Checks if the given Vector2 is within the gameboards border
	 * (Currently hardcoded borders)
	 * TODO: Make it dynamic so it calculates based on the gameboards position
	 */
	public bool insideBorder(Vector3 pos){
		Vector3 rounded_pos = roundVec3 (pos);
		return (rounded_pos.x >= 0 &&
		        rounded_pos.x < w &&
		        rounded_pos.y >= 0 &&
		        rounded_pos.z >= 0 &&
		        rounded_pos.z < d);
	}

	/*
	 * Goes through all plans and deletes the full ones,
	 * then moves all the plans above one step down
	 * called after each discrete timestep
	 * @return The number of deleted plans.
	 */
	public int deleteFullPlans() {
		int plansDeleted = 0;
		for (int y = 0; y < h; ++y) {
			// Uncomment if using the "2sides" gameplay
			// if (areSidesFull(y)) {
			if (isPlanFull(y)) {
				plansDeleted++;
				deletePlan(y);
				decreasePlansAbove(y+1);
				--y;
			}
		}
		return plansDeleted;
	}
}
