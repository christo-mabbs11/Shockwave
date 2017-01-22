using UnityEngine;
using System.Collections;

public class GhostManager : MonoBehaviour {

	private float GhostTime = 2.0f;
	private float GhostTimer = 0.0f;

	private SpriteRenderer SpriteRendererRef;
	Color GhostColor;
	private float StartOpacity = 0.0f;
	private float EndOpacity = 0.5f;

	void Awake () {
		SpriteRendererRef = this.GetComponent<SpriteRenderer> ();
		GhostColor  = SpriteRendererRef.material.color;
		GhostColor.a = 0.0f;
		SpriteRendererRef.material.color = GhostColor;
	}

	void Update () {

		// Update timer
		GhostTimer += Time.deltaTime;

		// Apply opacity
		GhostColor  = SpriteRendererRef.material.color;
		GhostColor.a = (EndOpacity - StartOpacity) * (GhostTimer / GhostTime);
		SpriteRendererRef.material.color = GhostColor;

		// Kill ghost at time
		if ( GhostTimer >= GhostTime ) {
			Destroy (this.gameObject);
		}
	}
}
