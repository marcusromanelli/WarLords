using UnityEngine;
using System;
using System.Collections;

public class SceneController : MonoBehaviour
{

	private static SceneController _singleton;
	public static SceneController Singleton
	{
		get
		{
			if (_singleton == null)
			{
				SceneController aux = GameObject.FindObjectOfType<SceneController>();
				if (aux == null)
				{
					_singleton = (new GameObject("-----Global Controller-----", typeof(SceneController))).GetComponent<SceneController>();
				}
				else
				{
					_singleton = aux;
				}
			}
			DontDestroyOnLoad(_singleton);
			return _singleton;
		}
	}


	public void LoadLevel(MenuScreens level = MenuScreens.Loader, float fadeout = 0.5f, float fadein = 0.5f)
	{
		StartCoroutine(_loadLevel((int)level, fadeout, fadein));
	}
	/*public void LoadLevel(StageNames level = StageNames.Swamp, float fadeout = 0.5f, float fadein = 0.5f)
	{
		int number = Enum.GetNames(typeof(MenuScreens)).Length - 1;
		StartCoroutine(_loadLevel((int)level + number + 1, fadeout, fadein));
	}*/
	IEnumerator _loadLevel(int level, float fadeout, float fadein)
	{
		ScreenController.showLoadingIcon = true;
		ScreenController.FadeOut(fadeout);
		while (ScreenController.singleton.currentFade < 1)
		{
			yield return null;
		}
		LevelController.AsyncPreLoadLevel(level);

		while (LevelController.singleton.Async.progress < 0.9f)
		{
			yield return null;
		}
		LevelController.AsyncLoadLevel();


		yield return new WaitForSeconds(6);
		ScreenController.FadeIn(fadein);
	}
}