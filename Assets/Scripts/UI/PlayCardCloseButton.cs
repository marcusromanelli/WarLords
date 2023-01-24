using UnityEngine;

public class PlayCardCloseButton : MonoBehaviour {

	[SerializeField] CardObject card;

	void OnMouseDown(){
		card.OnCloseButtonClick();
	}
}
