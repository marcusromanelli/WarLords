﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhaseTitle : MonoBehaviour
{
	[SerializeField] Image image;
	[SerializeField] float fadeSpeed = 0.05f;
	public Sprite Combat, Enemy, Movement, Your, Win, Lose;
	public bool IsChanging => isChanging;
	bool isChanging;

	public void setWinner(Player player)
	{
		/*if (!finish)
		{
			if (player.GetCivilization() == gameController.GetLocalPlayer().GetCivilization())
			{
				image.sprite = Singleton.Win;
			}
			else
			{
				image.sprite = Singleton.Lose;
			}
			finish = true;
			StartCoroutine(Fade());
		}*/
	}
	public void ChangePhase(Phase nextPhase, bool localPlayer) 
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

	void SetImage(Phase next, bool localPlayer)
	{
		image.enabled = true;
		switch (next)
		{
			case Phase.Draw:
				image.sprite = localPlayer ? Your : Enemy;
				break;
			case Phase.Movement:
				image.sprite = Movement;
				break;
			case Phase.Attack:
				image.sprite = Combat;
				break;
			default:
				image.sprite = null;
				image.enabled = false;
				break;
		}
	}
}