using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ForceOrthographicSortMode : MonoBehaviour 
{
	public void Start() 
	{
		GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
	}
}