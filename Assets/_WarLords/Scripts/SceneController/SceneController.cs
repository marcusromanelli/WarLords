using System;
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
		StopAllCoroutines();

		StartCoroutine(_loadLevel((int)level, fadeOt, fadeIn));
	}

	IEnumerator _loadLevel(int level, float fadeOt, float fadeIn)
	{
		ScreenController.showLoadingIcon = true;

		yield return DoFadeOut(fadeOt);

		yield return LoadScene(level);

		yield return DoFadeIn(fadeIn);
	}

	IEnumerator LoadScene(int level)
    {
		LevelController.AsyncPreLoadLevel(level);

		yield return WhileDo(() => { return LevelController.Instance.Async.progress < 0.9f; });

		LevelController.AsyncLoadLevel();
	}

	IEnumerator DoFadeOut(float fadeOut)
	{
		ScreenController.FadeOut(fadeOut);

		yield return WhileDo(() => { return ScreenController.Instance.currentFade < 1; });
	}
	IEnumerator DoFadeIn(float fadeIn)
	{
		ScreenController.FadeIn(fadeIn);

		yield return WhileDo(() => { return ScreenController.Instance.currentFade > 0; });
	}

	IEnumerator WhileDo(Func<bool> whileCondition, Action doAction = null)
    {
        while (whileCondition())
        {
			doAction?.Invoke();
			yield return null;
        }
    }
}