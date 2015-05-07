using UnityEngine;
using System.Collections;

public class smashBottle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void hit(){
		gameObject.GetComponent<Animator>().SetBool("Shot",true);
	}
}
