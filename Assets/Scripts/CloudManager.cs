using UnityEngine;
using System.Collections;

public class CloudManager : MonoBehaviour {

	private float CloudSpeed = 0.4f;
	private float RandomizedSpeed;

	void Awake () {
		RandomizedSpeed = ((float)Random.Range (0,100))/100.0f*1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3 ( this.transform.position.x-(CloudSpeed+RandomizedSpeed)*Time.deltaTime, this.transform.position.y, 0 );

		if ( this.transform.position.x <= -42.62f ) {
			this.transform.position = new Vector3 ( 41.96f, this.transform.position.y, 0 );
		}

	}
}
