using UnityEngine;

public class PlayCardCloseButton : MonoBehaviour {

	[SerializeField] UICardObject card;

	void OnMouseDown(){
		card.OnCloseButtonClick();
	}
}
