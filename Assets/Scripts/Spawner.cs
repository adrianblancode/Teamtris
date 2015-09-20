using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	// Groups
	public GameObject[] groups;
	public int players = 2;
	private int blocksSent = 0;
	private static int blockIndex;

	public void Awake(){
		blockIndex = Random.Range (0, groups.Length);
	}

	public GameObject getNext() {
		blocksSent++;
		GameObject block = groups [blockIndex];

		// Random Index
		if (blocksSent >= players) {
			blockIndex = Random.Range (0, groups.Length);
			blocksSent = 0;
		}

		return block;
	}

	public GameObject spawnNext() {
		// Random Index
		int i = Random.Range(0, groups.Length);

		// Spawn Group at current Position
		return (GameObject)Instantiate(groups[i],
		            transform.position,
		            Quaternion.identity);
	}
}