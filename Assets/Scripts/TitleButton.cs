using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

// Used for changing the style of a button on hover
public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public Button button;
	public Text buttonTitle;
	public Text buttonSubtitle;
	Color accentColorPrimary = new Color(0.482f, 0.294f, 1.0f);

	public void OnPointerEnter(PointerEventData eventData) {
		button.image.fillCenter = true;
		buttonTitle.color = accentColorPrimary;

		//Not every button has a subtitle
		if (buttonSubtitle != null) {
			buttonSubtitle.color = accentColorPrimary;
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		button.image.fillCenter = false;
		buttonTitle.color = Color.white;

		//Not every button has a subtitle
		if (buttonSubtitle != null) {
			buttonSubtitle.color = Color.white;
		}
	}
}
