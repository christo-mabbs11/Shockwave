using UnityEngine;
using System.Collections;

public class HitMarkManager : MonoBehaviour {

	private float LifeTime = 0.3f;
	private float LifeTimer = 0.0f;
	
	// Update is called once per frame
	void Update () {
	
		LifeTimer += Time.deltaTime;
		if ( LifeTimer >= LifeTime ) {
			Destroy (this.gameObject);
		}

	}
}
