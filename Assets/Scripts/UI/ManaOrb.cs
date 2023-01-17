using NaughtyAttributes;
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

	Color lastUsedMaterialColor, lastUsedEmissionColor;

    public void SetStatus(ManaStatus status)
    {
		ManaStatus = status;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
		switch (ManaStatus)
		{
			case ManaStatus.Used:
				SetUsed();
				break;
			case ManaStatus.Active:
				SetActive();
				break;
			case ManaStatus.Preview:
				SetPreview();
				break;
		}
	}
	[Button("Set Used")]
	void SetUsed()
	{
		particleSystems.ToList().ForEach(delegate (ParticleSystem obj) {
			if (obj.isPlaying || obj.IsAlive())
			{
				obj.Stop(true);
				particleSystemSpeed = obj.playbackSpeed;
				obj.playbackSpeed = particleSystemSpeed * 5f;
			}
		});

		SetRendererColor(Color.black, Color.black);

	}
	[Button("Set Active")]
	void SetActive()
	{
		particleSystems.ToList().ForEach(delegate (ParticleSystem obj) {
			if (!obj.isPlaying)
			{
				obj.Play();
				obj.playbackSpeed = particleSystemSpeed;
			}
		});

		SetRendererColor(originalColor, originalEmissionColor);
	}
	[Button("Set Preview")]
	void SetPreview()
	{
		particleSystems.ToList().ForEach(delegate (ParticleSystem obj) {
			if (!obj.isPlaying)
			{
				obj.Play();
				obj.playbackSpeed = particleSystemSpeed;
			}
		});

		SetRendererColor(Color.red, Color.red);
	}
	void SetRendererColor(Color materialColor, Color emissionColor)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			var renderer = renderers[i];

			if (materialColor != lastUsedMaterialColor)
				renderer.material.color = Color.Lerp(lastUsedMaterialColor, materialColor, Time.deltaTime);

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
