﻿using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void Play(){
		GlobalController.Singleton.LoadLevel(StageNames.Swamp, 1f, 3f);
	}
}
