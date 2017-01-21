using UnityEngine;
using System.Collections;

public class BombPlantManager : MonoBehaviour {

	public string TeamName = "";

	GameObject GameManagerRef;

	void Awake (  ) {
		GameManagerRef = GameObject.FindGameObjectWithTag ("GameManager");
	}

	public void BombPlanted () {
		GameManagerRef.GetComponent<GameManager> ().Endgame ();
	}
}
