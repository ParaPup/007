using UnityEngine;
using System.Collections;

public class ButtonHandle : MonoBehaviour {
	public GameObject oGate;
	public bool Open;

	void intract(){
		if(Open){
			Open=false;
			oGate.GetComponent<Animator>().SetBool("Open",false);
			}
		else{
			Open=true;
			oGate.GetComponent<Animator>().SetBool("Open",true);
		}
	}
}