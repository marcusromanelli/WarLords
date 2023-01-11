using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LifePointsController : MonoBehaviour {

	public float rotationSpeed;
	public enum Side{ A, B }
	public Side side;
	TextMesh SideA, SideB;
	public bool doDamage;

	void Start(){
		side = Side.A;
		SideA = transform.Find ("Life1").GetComponent<TextMesh>();
		SideB = transform.Find ("Life2").GetComponent<TextMesh> ();

		SideA.text = SideB.text = GameConfiguration.startLife.ToString();
	}

	public void setNewLife(int life){
		if(life<0) life=0;
		switch (side) {
		case Side.A:
			SideB.text = life.ToString();
			side = Side.B;
			break;
		case Side.B:
			SideA.text = life.ToString();
			side = Side.A;
			break;
		}
		transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles+Vector3.forward*1);
	}
	int life = 15;
	void Update () {
		if(doDamage){
			doDamage=false;
			life--;
			setNewLife (life--);
		}

		switch (side) {
		case Side.A:
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler (new Vector3 (0, 180, 0)), rotationSpeed*Time.deltaTime);
			break;
		case Side.B:
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 180, 180)), rotationSpeed*Time.deltaTime);
			break;
		}
	}
}
