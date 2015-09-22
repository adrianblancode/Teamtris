using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	// Groups
	public GameObject[] groups;

	public GameObject spawnNext() {
		// Random Index
		int i = Random.Range(0, groups.Length);

		// Spawn Group at current Position
		return (GameObject)Instantiate(groups[i],
		            transform.position,
		            Quaternion.identity);
	}
}