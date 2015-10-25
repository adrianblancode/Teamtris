using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcBlock : MonoBehaviour {

	private Vector3 block_scale = new Vector3(0.9f, 0.9f, 0.9f);

	private int x_limit, y_limit, z_limit;

    public GameObject[] block_parts;

    private enum Directions { Forward=0, Backward, Left, Right, Up, Down, Invalid };
    private Dictionary<Directions, Vector3> direction_mapping;
    private Dictionary<Directions, Directions> directions_opposite;

    ProcBlock() {
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
		GameObject b = genBlock ();
	}

	// Update is called once per frame
	void Update () {

	}

    public GameObject genBlock(){
		GameObject parent = new GameObject ();

        int b = Random.Range(0, block_parts.Length);
        int num_childs = Random.Range(0, 20);
        
        Directions last_direction = Directions.Invalid;
		Vector3 last_position = parent.transform.position;
        for(int i = 0; i < num_childs; ++i){
            Directions dir = (Directions)Random.Range(0, 6);
			while(last_direction != Directions.Invalid && dir != directions_opposite[last_direction]){
				dir = (Directions)Random.Range(0, 6);
			}
			Vector3 new_position = last_position + direction_mapping[dir];
			GameObject current = (GameObject)Instantiate(block_parts[b], new_position, parent.transform.rotation);

			current.transform.localScale = block_scale;
			current.transform.SetParent(parent.transform);

			last_position = new_position;
            last_direction = dir;
        }

        return parent;
    }
}
