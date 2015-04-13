using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHandler : MonoBehaviour {

	//Reduntant old remove soon
	public Camera oCam;
	public AudioClip aShoot;
	public ParticleSystem flash;
	public bool canShoot=true;
	// ^^^^^^^^^^^^^^^^^^^^^^^^

	//public string CurrentWeapon;

	public List<string> availableWeapons = new List<string>();
	public string CurrentWeapon;

	void Start(){
		CurrentWeapon = "Unarmed";
	}

	void ActiveWeapon(){

	}

	// LOOP WEAPONS, FIND NEXT, assign activeweapon to NEXT in WeaponList
	void NextWeapon(){

	}

	void SwitchWeapon(){
		//Up > 0
		//Down < 0

		//Next Weapon
		if(Input.GetAxis("Mouse ScrollWheel")> 0){

		}
		//Last Weapon
		if(Input.GetAxis("Mouse ScrollWheel")< 0){
			
		}
	}

	// Update is called once per frame
	void Update () {
		SwitchWeapon();
		FireTrigger();
	}

	void WeaponPickUp(string WeaponName){
		availableWeapons.Add(WeaponName);
	}
	
	RaycastHit hit;
	void FireTrigger(){
		if (Input.GetButtonDown("Fire1") & canShoot){

			StartCoroutine(NextShot(0.5f));
			canShoot = false;
			Ray ray = oCam.ScreenPointToRay(new Vector3(oCam.pixelWidth/2, oCam.pixelHeight/2, 0));
				Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
			GetComponent<AudioSource>().PlayOneShot(aShoot);
			flash.Play();
			
			if (Physics.Raycast(ray, out hit)){
				if(hit.collider.gameObject.tag == "enemy"){
					hit.collider.gameObject.SendMessage("hit",50F);
					//Instantiate(bloodPrefab, hit.point, hit.transform.rotation);
					//hit.collider.gameObject.SendMessage("DecreaseHealth", amountOfHealthToDecrease);
				}
			}
		}
	}
	
	IEnumerator NextShot(float BulletSpeed){
		yield return new WaitForSeconds(BulletSpeed);
		canShoot = true;

	}
}
