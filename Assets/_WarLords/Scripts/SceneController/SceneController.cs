using System.Collections;
using UnityEngine;

public class SceneController : Singleton<SceneController>
{
#if UNITY_EDITOR
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
			LoadLevel(MenuScreens.Loader);
    }
#endif
	public static void LoadLevel(MenuScreens level = MenuScreens.Loader, float fadeOt = 0.5f, float fadeIn = 0.5f)
	{
		Instance._loadLevel(level, fadeOt, fadeIn);
	}
	void _loadLevel(MenuScreens level, float fadeOt, float fadeIn)
	{
		StartCoroutine(_loadLevel((int)level, fadeOt, fadeIn));
	}
	/*public void LoadLevel(StageNames level = StageNames.Swamp, float fadeout = 0.5f, float fadein = 0.5f)
	{
		int number = Enum.GetNames(typeof(MenuScreens)).Length - 1;
		StartCoroutine(_loadLevel((int)level + number + 1, fadeout, fadein));
	}*/
	IEnumerator _loadLevel(int level, float fadeOt, float fadeIn)
	{
		ScreenController.showLoadingIcon = true;
		ScreenController.FadeOut(fadeOt);

		while (ScreenController.Instance.currentFade < 1)
		{
			yield return null;
		}

		LevelController.AsyncPreLoadLevel(level);

		while (LevelController.Instance.Async.progress < 0.9f)
		{
			yield return null;
		}

		LevelController.AsyncLoadLevel();

		ScreenController.FadeIn(fadeIn);
	}
}