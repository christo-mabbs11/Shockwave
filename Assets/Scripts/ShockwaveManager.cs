using UnityEngine;
using System.Collections;

public class ShockwaveManager : MonoBehaviour {

	private float StartOpacity = 0.75f;
	private float EndOpacity = 0.0f;
	private float WaveSpeed = 2.0f;

	private float WaveTime = 0.2f;
	private float WaveTimer = 0.0f;

	private SpriteRenderer SpriteRendererRef;

	void Awake() {
		SpriteRendererRef = this.GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {


		// Increase size
		float TempIncrease = WaveSpeed * Time.deltaTime;
		this.transform.localScale = new Vector3 ( this.transform.localScale.x+TempIncrease, this.transform.localScale.y+TempIncrease, this.transform.localScale.z+TempIncrease );

		// Set transparency
		Color tmp = SpriteRendererRef.color;
		tmp.a = (StartOpacity-EndOpacity)*(1-WaveTimer/WaveTime);
		SpriteRendererRef.color = tmp;


		WaveTimer += Time.deltaTime;
		if ( WaveTimer >= WaveTime ) {
			Destroy (this.gameObject);
		}
	}

	public void SetWaveTime ( float Arg_Time ) {
		WaveTime = Arg_Time;
	}

	public void SetWaveSpeed ( float Arg_Speed ) {
		WaveSpeed = Arg_Speed;
	}
}
