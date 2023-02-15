using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : Singleton<LevelController>
{
	private AsyncOperation _async;
	public AsyncOperation Async
	{
		get
		{
			if (Instance._async == null)
			{
				Instance._async = new AsyncOperation();
			}
			return Instance._async;
		}
		protected set
		{
			_async = value;
		}
	}

	public static bool doneLoading
	{
		get
		{
			return Instance.Async.isDone;
		}
		private set { }
	}

	public static void AsyncPreLoadLevel(int newFase)
	{
		Instance._AsyncPreLoadLevel(newFase);
	}

	void _AsyncPreLoadLevel(int newFase)
	{
		Debug.LogWarning("Started Loading.");
		Async = SceneManager.LoadSceneAsync(newFase);
		Async.allowSceneActivation = false;
	}

	public static void AsyncLoadLevel()
	{
		Instance._AsyncLoadLevel();
	}
	void _AsyncLoadLevel()
	{
		Async.allowSceneActivation = true;
	}
}
