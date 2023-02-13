using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraController : Singleton<CameraController>
{
	[SerializeField] CardPositionData visualizeCardPositionOffset;


	public static CardPositionData CalculateForwardCameraPosition()
	{
		var mainCameraPosition = Camera.main.transform.position;
		mainCameraPosition += (Camera.main.transform.forward * Instance.visualizeCardPositionOffset.Position.z); //Adjust Z
		mainCameraPosition += (-Camera.main.transform.up * Instance.visualizeCardPositionOffset.Position.y); //Adjust Y

		return CardPositionData.Create(mainCameraPosition, Instance.visualizeCardPositionOffset.Rotation);
	}
}
