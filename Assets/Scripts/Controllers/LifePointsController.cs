using UnityEngine;

[ExecuteInEditMode]
public class LifePointsController : MonoBehaviour {

	[SerializeField] TextMesh sideA;
	[SerializeField] TextMesh sideB;
	[SerializeField] float rotationSpeed = 350;
	[SerializeField] bool ROTATE;
	[SerializeField] Side currentSide;

	private int startLife;
	private int currentLife;
	private bool hasInitialized = false;
	private Quaternion sideARotation = Quaternion.Euler(new Vector3(0, 180, 0));
	private Quaternion sideBRotation = Quaternion.Euler(new Vector3(0, 180, 180));

		private enum Side{ A, B }
	

	void Start(){
		currentSide = Side.A;
	}

	public void Setup(int startLife)
    {
		this.startLife = startLife;

		SetLife(startLife);

		hasInitialized = true;

	}

	public void UpdateLife()
	{
		if (!hasInitialized)
		{
			sideA.text = currentLife.ToString();
			return;
		}

		switch (currentSide) {
			case Side.A:
				sideB.text = currentLife.ToString();
				currentSide = Side.B;
				break;
			case Side.B:
				sideA.text = currentLife.ToString();
				currentSide = Side.A;
				break;
		}

		transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles + Vector3.forward);
	}

	public void SetLife(int newLife)
    {
		currentLife = newLife;

		if (currentLife < 0) currentLife = 0;

		UpdateLife();
    }

	void RotateChip()
    {
		Quaternion targetRotation;

		switch (currentSide)
		{
			default:
			case Side.A:
				targetRotation = sideARotation;
				break;
			case Side.B:
				targetRotation = sideBRotation;
				break;
		}

		if(transform.rotation != targetRotation)
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
	}

	void Update () {
        if (ROTATE)
        {
			SetLife(currentLife - 1);

			ROTATE = false;
		}

		RotateChip();
	}


}
