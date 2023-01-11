using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

	[Range(0.5f, 7)]
	/// <summary>
	/// How much fast the camera goes from A to B (needs tweaks)
	/// </summary>
	public float cameraRotatingSpeed = 5f;
	/// <summary>
	/// How much the mouse must be moved to account a camera movement (needs tweaks)
	/// </summary>
	public float cameraRotatingSensitivity = 0.5f;
	/// <summary>
	/// The distance from map.
	/// </summary>
	public float distanceFromMap = 100f;
	/// <summary>
	/// The currently height the camera is
	/// </summary>
	public float cameraHeight = 50f;
	/// <summary>
	/// The height of the LookAtPoint
	/// </summary>
	public float cameraHeightLook = 50f;
	/// <summary>
	/// The angle in radians.
	/// </summary>
	float angleInRadians = 0;
	/// <summary>
	/// Stores the desired angle to manually rotate the camera
	/// </summary>
	public Vector2 cameraLookSlider;
	/// <summary>
	/// The camera. (Derp)
	/// </summary>
	new Camera camera;
	public Transform centerPivot;
	public bool isCameraLocked = true;

	Vector3 center;
	void Start () {
		camera = GetComponent<Camera> ();

		center = centerPivot.transform.position;//new Vector3 ((GridGenerator.Singleton.transform.position.x+GridGenerator.Singleton.numberOfLanes*GridGenerator.Singleton.squareSize)/2, 0, (GridGenerator.Singleton.transform.position.z+GridGenerator.Singleton.numberOfSquares*GridGenerator.Singleton.squareSize)/2);
	}

	void Update () {
		center = centerPivot.transform.position;
		camera.transform.position = calculatePositionAroundArena ();


		camera.transform.LookAt (center);

	}

	Vector3 calculatePositionAroundArena(){
		if ((Application.isEditor && Input.GetMouseButton (0) || (!Application.isEditor && Input.GetMouseButton (1))) && !isCameraLocked) {
			cameraLookSlider.x += (-Input.GetAxis ("Mouse X") / cameraRotatingSensitivity) * cameraRotatingSpeed;
			cameraLookSlider.x = Mathf.Clamp (cameraLookSlider.x, -1, 360);
		}

		//Compensar o LERP para ficar ciclico (gambiarra?)
		if (cameraLookSlider.x > 359)
			cameraLookSlider.x = 0;

		if (cameraLookSlider.x < 0)
			cameraLookSlider.x = 359;

		//Nada ainda
		cameraLookSlider.y = Mathf.Clamp (cameraLookSlider.y, 0, 1);

		//Converter graus para radianos?? oO
		//Edit: Sim, porque as funções trigonometricas pegam radianos -q
		angleInRadians = Mathf.Deg2Rad * cameraLookSlider.x;
		return calculatePositionInTrail(center, angleInRadians);
	}
	public Vector3 calculatePositionInTrail(Vector3 center, float radians=0){
		radians -= Mathf.Deg2Rad * -90;
		//Calcular a distância entre a CAMERA e o PIVOT;

		//Calcular a posição da CAMERA (this) baseado no centro do circulo (PIVOT), e na angulação (CAMERALOOKSLIDER.X)
		float newPositionX = center.x + (distanceFromMap * Mathf.Cos (radians));
		float newPositionZ = center.z + (distanceFromMap * Mathf.Sin (radians));


		return new Vector3 (newPositionX, cameraHeight, newPositionZ);
	}

	
}
