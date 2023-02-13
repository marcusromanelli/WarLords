using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhaseTitle : MonoBehaviour
{
	[SerializeField] Image image;
	[SerializeField] float fadeSpeed = 0.05f;
	public Sprite Combat, Enemy, Movement, Your, Win, Lose;
	public bool IsChanging => isChanging;
	bool isChanging;

	public void SetWinner(bool isLocal)
	{
		image.sprite = isLocal ? Win : Lose;

		StartCoroutine(Fade());
	}
	public void ChangePhase(PhaseType nextPhase, bool localPlayer) 
	{
		SetImage(nextPhase, localPlayer);
		StopAllCoroutines();
		StartCoroutine(Fade());
	}
	void SetImageAlpha(float value)
    {
		Color aux = image.color;
		aux.a = value;
		image.color = aux;
	}
	IEnumerator FadeIn()
	{
		if (image.sprite == null)
			yield break;

		var alpha = 0f;
		while (alpha < 1)
		{
			SetImageAlpha(alpha += fadeSpeed);

			yield return null;
		}
	}
	IEnumerator FadeOut()
	{
		if (image.sprite == null)
			yield break;

		var alpha = 1f;
		while (alpha > 0)
		{
			SetImageAlpha(alpha -= fadeSpeed);

			yield return null;
		}
	}
	IEnumerator Fade()
	{
		isChanging = true;

		yield return FadeIn();

        yield return new WaitForSeconds(1f);

		yield return FadeOut();

		isChanging = false;
	}

	void SetImage(PhaseType next, bool localPlayer)
	{
		image.enabled = true;
		switch (next)
		{
			case PhaseType.Draw:
				image.sprite = localPlayer ? Your : Enemy;
				break;
			case PhaseType.Movement:
				image.sprite = Movement;
				break;
			case PhaseType.Attack:
				image.sprite = Combat;
				break;
			default:
				image.sprite = null;
				image.enabled = false;
				break;
		}
	}
}