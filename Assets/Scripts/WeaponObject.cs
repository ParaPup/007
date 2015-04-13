using UnityEngine;
using System.Collections;

public class WeaponObject : MonoBehaviour {
	public string WeaponName;

	//List of Weapons

	/*  Unarmed
		Hunting Knife
		Throwing Knife
		PP7 Special Issue
		Silenced PP7
		DD44 Dostovei
		Klobb
		KF7 Soviet
		ZMG (9mm)
		D5K Deutsche
		Silenced D5K
		Phantom
		AR33 Assault Rifle
		RC-P90
		Shotgun
		Automatic Shotgun
		Sniper Rifle
		Cougar Magnum
		Golden Gun
		Silver PP7
		Gold PP7
		Moonraker Laser
		Watch Laser
		Grenade Launcher
		Rocket Launcher
		Hand Grenade
		Timed Mine
		Proximity Mine
		Remote Mine
		Detonator
		Taser
		Tank
	 */

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player"){
			other.gameObject.SendMessage("WeaponPickUp",WeaponName);
			Destroy(gameObject);
		}
	}
}