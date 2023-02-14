using UnityEngine;

public class NextPhaseButton : MonoBehaviour {

	[SerializeField] Player player;

	void Awake () {
		Hide();
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	
	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void OnClick(){
		player.OnClickNextPhase();
	}
}