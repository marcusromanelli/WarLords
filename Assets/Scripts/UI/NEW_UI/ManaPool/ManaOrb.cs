using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ManaOrb : MonoBehaviour
{
	public ManaStatus ManaStatus { get; private set; } = ManaStatus.Active;

	[SerializeField] GameObject activeState;
	[SerializeField] GameObject usedState;
	[SerializeField] GameObject previewState;

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
	void setUsed()
	{
		SetUsed();
	}
	[Button("Set Active")]
	void setActive()
	{
		SetActive();
	}
	[Button("Set Preview")]
	void setPreview()
	{
		SetPreview();
	}

	void SetUsed()
	{
		activeState.SetActive(false);
		previewState.SetActive(false);
		usedState.SetActive(true);
	}
	void SetActive()
	{
		activeState.SetActive(true);
		previewState.SetActive(false);
		usedState.SetActive(false);
	}
	void SetPreview()
	{
		activeState.SetActive(false);
		previewState.SetActive(true);
		usedState.SetActive(false);
	}
}
