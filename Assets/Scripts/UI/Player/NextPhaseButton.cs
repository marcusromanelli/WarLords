using UnityEngine;
using System.Collections;

public class NextPhaseButton : MonoBehaviour {

	[SerializeField] Player player;
	[SerializeField] float speed = 10f;

	Vector3 showPosition = new Vector3(0, 4, -3);
	Vector3 hiddenPosition = new Vector3(2, 4, -3);

	public bool isHidden;

	void Awake () {
		isHidden = true;
		transform.localPosition = hiddenPosition;

		Hide(false);
	}
	public void Hide(bool animate = true)
	{
		StopAllCoroutines();
		if (!animate)
			transform.localPosition = hiddenPosition;
		else
			StartCoroutine(AnimateHide());

	}
	IEnumerator AnimateHide()
	{
		var inPosition = false;

		while (!inPosition)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, hiddenPosition, Time.deltaTime * speed);

			inPosition = transform.localPosition == hiddenPosition;
			yield return null;
		}
	}
	public void Show(bool animate = true)
	{
		StopAllCoroutines();
		if (!animate)
			transform.localPosition = hiddenPosition;
		else
			StartCoroutine(AnimateShow());
	}
	IEnumerator AnimateShow()
	{
		var inPosition = false;

		while (!inPosition)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, showPosition, Time.deltaTime * speed);

			inPosition = transform.localPosition == showPosition;
			yield return null;
		}
	}

	void OnMouseDown(){
		player.OnClickNextPhase();
	}
}