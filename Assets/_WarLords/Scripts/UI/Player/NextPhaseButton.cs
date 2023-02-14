using UnityEngine;

public class NextPhaseButton : MonoBehaviour {

	[SerializeField] Player player;

	public void OnClick(){
		player.OnClickNextPhase();
	}
}