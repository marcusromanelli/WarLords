using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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

	enum Side{ A, B }
	bool doDamage;

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

		switch (currentSide)
		{
			case Side.A:
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 180, 0)), rotationSpeed * Time.deltaTime);
				break;
			case Side.B:
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 180, 180)), rotationSpeed * Time.deltaTime);
				break;
		}
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
