using UnityEngine;
using System.Collections;

public class DoorAnimationScript : MonoBehaviour {
	public bool Open;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void intract(){
		if(Open){
			Debug.Log("Open");
			Open=false;
			gameObject.GetComponent<Animator>().SetBool("Open",false);
		}
		else if(!Open){
			Open=true;
			Debug.Log("!Open");
			gameObject.GetComponent<Animator>().SetBool("Open",true);
		}
	}
}
