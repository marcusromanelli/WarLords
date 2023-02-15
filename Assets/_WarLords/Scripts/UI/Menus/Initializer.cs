using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Initializer : MonoBehaviour {
	[SerializeField] Image image;
	[SerializeField] CanvasGroup canvasGroup;
	[SerializeField] Sprite[] icons;
	[SerializeField] float speed = 0.01f;	

	private ILoadableBaseClass[] loadableComponents;

	void Start () {
		LoadRoutine();
		StartCoroutine(FadeRoutine());

		Application.targetFrameRate = 60;
	}

	void Finish(){
		SceneController.LoadLevel(MenuScreens.Menu);
	}
	void LoadRoutine()
	{
		loadableComponents = GetComponents<ILoadableBaseClass>();

		foreach (var component in loadableComponents)
		{
			component.Load();
		}
	}
	IEnumerator FadeRoutine(){

		int currentIndex = 0;

		while(currentIndex < icons.Length)
        {
			image.sprite = icons[currentIndex];

			yield return fadeIn();
			yield return fadeOut();

			currentIndex++;
		}

		int numberLoaded = 0;
        while (numberLoaded < loadableComponents.Length)
        {
			numberLoaded = 0;

			foreach (var component in loadableComponents)
            {
				if (!component.HasLoaded())
					break;

				numberLoaded++;
            }
			yield return null;
        }

		Finish();
	}
	IEnumerator fadeIn()
	{
		float c = 0;

		while (c < 1)
		{
			c += speed * Time.deltaTime;

			canvasGroup.alpha = c;
			yield return null;
		}
	}
	IEnumerator fadeOut()
	{
		float c = 0;

		while (c > 0)
		{
			c -= speed * Time.deltaTime;

			canvasGroup.alpha = c;
			yield return null;
		}
	}
}
