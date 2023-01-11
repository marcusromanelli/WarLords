using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode]
public class Grid : MonoBehaviour {

	private static Grid _singleton;
	public static Grid Singleton{
		get{
			if (_singleton == null) {
				Grid aux = GameObject.FindObjectOfType<Grid> ();
				if (aux == null) {
					_singleton = (new GameObject ("-----Grid Generator-----", typeof(Grid))).GetComponent<Grid> ();
				} else {
					_singleton = aux;
				}
			}
			return _singleton;
		}
	}

	public GameObject[] gridTiles;
	public int numberOfLanes = 3;
	public int numberOfSquares = 10;
	public int numberOfSpawnAreasPerLane = 2;

	public bool doGenerate;
	public bool updateSquareSize;

	List<GameObject> tiles;
	public float squareSize;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (doGenerate) {
			doGenerate = false;
			generate ();
		}

		if (numberOfLanes % 2 == 0) {
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning ("The number of lanes must be an odd number.");
		}
	}


	void generate(){
		if (numberOfLanes % 2 != 0) {
			eraseAllGrid ();

			int currentLane = 0;

			Vector3 aux;
			GameObject aux2;
			GameObject lane;

			while(currentLane<numberOfLanes){
				aux = transform.position;
				aux.x += currentLane * squareSize;
				lane = new GameObject ("Lane "+(currentLane+1));
				lane.transform.position = aux;
				lane.transform.SetParent (transform, true);

				for (int i = 0; i < numberOfSquares; i++) {
					aux.z = i * squareSize;
					aux2 = (GameObject)Instantiate (gridTiles[currentLane], Vector3.zero, Quaternion.identity);
					aux2.transform.position = aux;
					aux2.transform.SetParent (lane.transform);
					tiles.Add (aux2);
				}

				currentLane++;
			}

		} else {
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning ("The number of lanes must be an odd number.");
		}
	}

	void eraseAllGrid(){
		if (tiles == null) {
			tiles = new List<GameObject> ();
			return; 
		}
		tiles.ForEach (delegate(GameObject aux) {
			if(aux!=null){
				DestroyImmediate(aux);
			}
		});

		GetComponentsInChildren<Transform> ().ToList ().ForEach (delegate(Transform aux) {
			if(aux!=null && aux!=transform){
				DestroyImmediate(aux.gameObject);
			}	
		});

		tiles.Clear ();
	}


	public static Vector3 GridToUnity(Vector2 pos){
		return new Vector3 ((pos.x * Grid.Singleton.squareSize) + Grid.Singleton.transform.position.x, Grid.Singleton.transform.position.y, (pos.y * Grid.Singleton.squareSize) + Grid.Singleton.transform.position.z);
	}

	public static Vector2 UnityToGrid(Vector3 pos){
		float x = Mathf.RoundToInt ((pos.x - (Grid.Singleton.transform.position.x)) / Grid.Singleton.squareSize);
		x = x < 0 ? 0 : x;
		x = x > (Grid.Singleton.numberOfLanes - 1) ? Grid.Singleton.numberOfLanes - 1 : x;

		float y = Mathf.RoundToInt ((pos.z - (Grid.Singleton.transform.position.z)) / Grid.Singleton.squareSize);
		y = y < 0 ? 0 : y;
		y = y > (Grid.Singleton.numberOfSquares - 1) ? Grid.Singleton.numberOfSquares - 1 : y;


		return new Vector2 (x, y);
	}

	public static Vector2 Normalize(Vector2 pos){
		float x = pos.x;
		x = x < 0 ? 0 : x;
		x = x > (Grid.Singleton.numberOfLanes - 1) ? Grid.Singleton.numberOfLanes - 1 : x;

		float y = pos.y;
		y = y < 0 ? 0 : y;
		y = y > (Grid.Singleton.numberOfSquares - 1) ? Grid.Singleton.numberOfSquares - 1 : y;


		return new Vector2 (x, y);
	}
}