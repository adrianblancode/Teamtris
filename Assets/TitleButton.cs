using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public Text buttonText;
	Color accentColorPrimary = new Color(0.482f, 0.294f, 1.0f);
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		buttonText.color = accentColorPrimary;
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		buttonText.color = Color.white;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
