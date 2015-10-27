using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcBlock : MonoBehaviour {
	
	private LinkedList<GameObject> block;
//	private Stack<GameObject> block;
	private bool generate = false;
	private Vector3 block_scale = new Vector3(0.9f, 0.9f, 0.9f);

	private int x_limit, y_limit, z_limit;

    public GameObject[] block_parts;

    private enum Directions { Forward=0, Backward, Left, Right, Up, Down, Invalid };
    private Dictionary<Directions, Vector3> direction_mapping;
    private Dictionary<Directions, Directions> directions_opposite;

    void Awake() {
		block = new LinkedList<GameObject>();

        direction_mapping = new Dictionary<Directions, Vector3>();
        
        direction_mapping.Add(Directions.Right,     new Vector3(1, 0, 0));
        direction_mapping.Add(Directions.Left,      new Vector3(-1, 0, 0));
        direction_mapping.Add(Directions.Up,        new Vector3(0, 1, 0));
        direction_mapping.Add(Directions.Down,      new Vector3(0, -1, 0));
        direction_mapping.Add(Directions.Forward,   new Vector3(0, 0, 1));
        direction_mapping.Add(Directions.Backward,  new Vector3(0, 0, -1));

        directions_opposite = new Dictionary<Directions, Directions>();

        directions_opposite.Add(Directions.Right, 		Directions.Left);
        directions_opposite.Add(Directions.Left, 		Directions.Right);
        directions_opposite.Add(Directions.Up, 			Directions.Down);
        directions_opposite.Add(Directions.Down, 		Directions.Up);
        directions_opposite.Add(Directions.Forward, 	Directions.Backward);
        directions_opposite.Add(Directions.Backward, 	Directions.Forward);
    }

	// Use this for initialization
	void Start () {
		int i = Random.Range (0, block_parts.Length);
		GameObject first_piece = (GameObject)Instantiate (block_parts [i]);
		first_piece.transform.position = new Vector3 (0, 0, 0);
		first_piece.transform.localScale = block_scale;
		block.AddLast (first_piece);

		StartCoroutine ("test");
	}

	IEnumerator apply_rule1(){
		WaitForSeconds wait = new WaitForSeconds (1.0f);

		rule1 (block.Last.Value);

		yield return wait;
		generate = false;
	}

	IEnumerator apply_rule2(){
		WaitForSeconds wait = new WaitForSeconds (1.0f);
		
		rule2 (block.Last.Value);
		
		yield return wait;
		generate = false;
	}

	IEnumerator test(){
		for (int i = 0; i < 6; ++i) {
			rule2 (block.Last.Value);
			yield return new WaitForSeconds (1.0f);
		}
	}

	void FixedUpdate(){
//		if (!generate) {
//			generate = true;
//			StartCoroutine("apply_rule2");
//		}
	}
	
	private bool is_valid_position(Vector3 position){
		LinkedList<GameObject>.Enumerator it = block.GetEnumerator ();
		while (it.MoveNext()) {
			if(it.Current.transform.position == position){
				return false;
			}
		}
		return true;
	}

	// Basic rules

	/* Rule1:
	 *  _	   _ _
	 * |_| -> |_|_|
	 * 
	 */
	private void rule1(GameObject lh){
		Directions dir = (Directions)Random.Range (0, 6);
		Vector3 new_pos = lh.transform.position + direction_mapping [dir];
		while (!is_valid_position(new_pos)) {
			dir = (Directions)Random.Range (0, 6);
			new_pos = lh.transform.position + direction_mapping [dir];
		}
		GameObject child = (GameObject)Instantiate (lh, new_pos, lh.transform.rotation);
		child.transform.localScale = block_scale;
		block.AddLast (child);
	}

//	/* Rule2:
//	 *  _	   _ _ _
//	 * |_| -> |_|_|_|
//	 * 
//	 */
//	
	private void rule2(GameObject lh){
		Directions dir1 = (Directions)Random.Range (0, 6);
		Directions dir2 = (Directions)Random.Range (0, 6);
		Vector3 new_pos1 = lh.transform.position + direction_mapping [dir1];
		Vector3 new_pos2 = lh.transform.position + direction_mapping [dir2];
		while (!is_valid_position(new_pos1) && is_valid_position(new_pos2)) {
			dir1 = (Directions)Random.Range (0, 6);
			dir2 = (Directions)Random.Range (0, 6);
			new_pos1 = lh.transform.position + direction_mapping [dir1];
			new_pos2 = lh.transform.position + direction_mapping [dir2];
		}
		GameObject child1 = (GameObject)Instantiate (lh, new_pos1, lh.transform.rotation);
		GameObject child2 = (GameObject)Instantiate (lh, new_pos2, lh.transform.rotation);
		child1.transform.localScale = block_scale;
		child2.transform.localScale = block_scale;
		block.AddLast (child1);
		block.AddLast (child2);
	}
//
//	// More complex rules
//
//	/* Rule4:	 
//	 *  _ _	     _ _ _
//	 * |_|_| -> |_|_|_|
//	 * 
//	 */
//	private GameObject rule4(GameObject lh){
//	
//	}

	/* Rule5:
	 *  _	   _ _
	 * |_| -> |_|_|
	 * |_| -> |_|
	 */
}
