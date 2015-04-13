using UnityEngine;
using System.Collections;

public class ObjectBlowUp : MonoBehaviour {
	public float health = 300;
	public AudioClip explode;
	public Material mCurrent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void hit(float damage){
		if(health <= 0){
			Death();
			GetComponent<BoxCollider>().enabled = false;
		}
		else{
			health -= damage;
		}
	}

	void Death(){
		GetComponent<ParticleSystem>().Play();
		GetComponent<AudioSource>().PlayOneShot(explode);
		GetComponent<Renderer>().material = mCurrent;
	}
}