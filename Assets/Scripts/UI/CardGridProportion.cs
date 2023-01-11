using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(GridLayoutGroup)), ExecuteInEditMode]
public class CardGridProportion : MonoBehaviour {

	public float cardProportion = 1.73f;
	GridLayoutGroup grid;
	RectTransform rect;

	Vector2 sizeDelta, lastSizeDelta;


	void Start () {
	
	}
	

	void Update () {
		if (grid == null) {
			grid = GetComponent<GridLayoutGroup> ();
		}
		if (rect == null) {
			rect = GetComponent<RectTransform> ();
		}

		sizeDelta = grid.cellSize;
		sizeDelta.y = rect.sizeDelta.y;


		if (lastSizeDelta != sizeDelta) {
			sizeDelta.x = sizeDelta.y / cardProportion;

			grid.cellSize = sizeDelta;

			//calcular proporção
			lastSizeDelta = sizeDelta;
		}
	}
}
