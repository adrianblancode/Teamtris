using UnityEngine;
using System.Collections;


public class Grid {
	// Master grid or Slave (sideview of master)
	public enum MsMode {Master, Slave};

	public struct Block
	{
		public Vector3 pos;
		public Transform transform;
	}

	// The Grid itself. Origin (0,0,0) is
	// Left, bottom, "nearest"
	private int w; // Width
	private int h; // Heigth
	private int d; // Depth
	private MsMode msMode;

	private ArrayList grid3d = new ArrayList();

	public Grid(int x, int y, int z, MsMode mode) {
		w = x;
		h = y;
		d = z;
		msMode = mode;

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
	
	/*
	 * Checks whether plan 'y' is full or not
	 * *** Deprecated, use getMatches() instead
	 */
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


	/*
	 * Returns all blocks in depth level
	 * Includes compensation of Master vs Slave view
	 *
	 * helper function for getPlanMatches()
	 */
	private ArrayList frontMatches(int x, int y) {
		ArrayList blocks = new ArrayList();
		int xx = 0, yy = 0, zz = 0;

		for (int t = 0; t < w; ++t) {
			// Master front view is x,y with z as depth
			if (msMode == MsMode.Master) {
				xx = x; yy = y; zz = t;
			}

	         // Slave front view is y,z with -x as depth
	        if (msMode == MsMode.Slave) {
				xx = -t; yy = y; zz = x;
			}

			Transform trans = getGrid(zz)[xx, yy];

			if (trans != null) {
				Block block = new Block();
				block.transform = trans;
				block.pos = new Vector3(xx,yy,zz);
				blocks.Add(block);
			}
		}
		return blocks;
	}

	
	/*
	 * Checks plan 'y' for matches and returns
	 * a list with the matching blocks.
	 * 
	 * Empty list for no match.
	 * Assumes w == d  -->  w != d support is a feature TBD
	 */
	private ArrayList getPlanMatches(int y) {

		// Assume full plane match until proven wrong
		bool fullMatch = true;

		ArrayList fullMatchBlocks = new ArrayList();
		ArrayList minMatchBlocks = new ArrayList();

		for (int t = 0; t < w; ++t) {

			// Check front view (or side view if slave)
			ArrayList fMatch = 	frontMatches(t, y);

			if (fMatch.Count == 0) {
				return new ArrayList(); // No match
			}
			else {
				// Closest block is part of the min match
				minMatchBlocks.Add(fMatch[0]);

				if (fMatch.Count < w) fullMatch = false;

				// All blocks belong to the full match 
				!!!Fortsätt här !!!
				if (fullMatch) {
					minMatchBlocks.Add(fMatch);
				}
			}



		}
		
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
