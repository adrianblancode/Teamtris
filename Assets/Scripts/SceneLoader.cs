using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	public void LoadScene(int index)
	{
		Application.LoadLevel(index);
	}
	
	public void LoadScene(string levelName)
	{
		Application.LoadLevel(levelName);
	}
}
