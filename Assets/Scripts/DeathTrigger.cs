using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnControllerColliderHit(Collider other) {
		if(other.gameObject.name == "Player"){
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}
