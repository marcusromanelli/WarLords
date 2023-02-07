using UnityEngine;

public class FastForward : MonoBehaviour {

	[SerializeField] float gameSpeed = 2;

	private bool IsActive = false;

	public void OnClick(){

		Time.timeScale = IsActive ? 1 : gameSpeed;

		IsActive = !IsActive;
	}
}
