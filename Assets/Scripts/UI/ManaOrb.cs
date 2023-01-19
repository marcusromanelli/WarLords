using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ManaOrb : MonoBehaviour
{
	public ManaStatus ManaStatus { get; private set; } = ManaStatus.Active;

	[SerializeField] Renderer[] renderers;
	[SerializeField] ParticleSystem[] particleSystems;
	[SerializeField] float particleSystemSpeed = 1;
	[SerializeField] Color originalColor;
	[SerializeField] Color originalEmissionColor;
	[SerializeField] float materialInterpolationSpeed;

	Color lastUsedMaterialColor, lastUsedEmissionColor;
	private float materialSpeed;
	ManaStatus lastManaStatus;

	public void SetStatus(ManaStatus status)
	{
		if (status == lastManaStatus)
			return;

		ManaStatus = status;
		UpdateVisuals();

		lastManaStatus = ManaStatus;
	}

	void UpdateVisuals()
	{
		switch (ManaStatus)
		{
			case ManaStatus.Used:
				StartCoroutine("SetUsed");
				break;
			case ManaStatus.Active:
				StartCoroutine("SetActive");
				break;
			case ManaStatus.Preview:
				StartCoroutine("SetPreview");
				break;
		}

	}
	[Button("Set Used")]
	void setUsed()
	{
		StartCoroutine("SetUsed");
	}
	[Button("Set Active")]
	void setActive()
	{
		StartCoroutine("SetActive");
	}
	[Button("Set Preview")]
	void setPreview()
	{
		StartCoroutine("SetPreview");
	}
	IEnumerator SetUsed()
	{
		materialSpeed = 0;
		while (materialSpeed < 1)
		{
			particleSystems.ToList().ForEach(delegate (ParticleSystem obj)
			{
				if (obj.isPlaying || obj.IsAlive())
				{
					obj.Stop(true);
					//particleSystemSpeed = obj.playbackSpeed;
					//obj.playbackSpeed = particleSystemSpeed * 5f;
				}
			});

			SetRendererColor(Color.black, Color.black, materialSpeed);

			materialSpeed += materialInterpolationSpeed * Time.deltaTime;
			yield return null;
		}
	}
	IEnumerator SetActive()
	{
		materialSpeed = 0;
		while (materialSpeed < 1)
		{
			particleSystems.ToList().ForEach(delegate (ParticleSystem obj) {
				if (!obj.isPlaying)
				{
					obj.Play();
					obj.playbackSpeed = particleSystemSpeed;
				}
			});

			SetRendererColor(originalColor, originalEmissionColor, materialSpeed);

			materialSpeed += materialInterpolationSpeed;
			yield return null;
		}
	}
	IEnumerator SetPreview()
	{
		materialSpeed = 0;
		while (materialSpeed < 1)
		{
			particleSystems.ToList().ForEach(delegate (ParticleSystem obj) {
				if (!obj.isPlaying)
				{
					obj.Play();
					obj.playbackSpeed = particleSystemSpeed;
				}
			});

			SetRendererColor(Color.red, Color.red, materialSpeed);

			materialSpeed += materialInterpolationSpeed;
			yield return null;
		}
	}
	void SetRendererColor(Color materialColor, Color emissionColor, float interpolation)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			var renderer = renderers[i];

			if (!renderer.material.HasColor("_Color") && !renderer.material.HasColor("_EmissionColor"))
				continue;

			if (materialColor != lastUsedMaterialColor);
				renderer.material.color = Color.Lerp(renderer.material.color, materialColor, interpolation);

			if (materialColor != lastUsedEmissionColor)
				renderer.material.SetColor("_EmissionColor", emissionColor);
		}

		lastUsedMaterialColor = materialColor;
		lastUsedEmissionColor = emissionColor;
	}
	/*void RestartParticleSystem()
	{
		for (int i = 0; i < particleSystem.Length; i++)
		{
			var obj = particleSystem[i];

			if (obj.isPlaying || obj.IsAlive())
			{
				obj.Stop(true);
				particleSystemSpeed = obj.playbackSpeed;
				obj.playbackSpeed = particleSystemSpeed * 5f;
			}
		}
	}
	void DisableParticleSystem()
	{
		for (int i = 0; i < particleSystem.Length; i++)
		{
			var obj = particleSystem[i];

			if (!obj.isPlaying)
			{
				obj.Play();
				obj.playbackSpeed = particleSystemSpeed;
			}
		}
	}*/

}
