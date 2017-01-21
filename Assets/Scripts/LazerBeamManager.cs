﻿using UnityEngine;
using System.Collections;

public class LazerBeamManager : MonoBehaviour {

	private float LifeTime = 0.1f;
	private float LifeTimer = 0.0f;

	// Update is called once per frame
	void Update () {

		LifeTimer += Time.deltaTime;
		if ( LifeTimer >= LifeTime ) {
			Destroy (this.gameObject);
		}

	}
}
