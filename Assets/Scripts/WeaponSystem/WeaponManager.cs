using System.Collections.Generic;
using UnityEngine;
using WeaponBaseExtensions;

public class WeaponManager : MonoBehaviour
{
	public GameObject defaultWeapon = null; // Default weapon to equip the player with
	public Transform weaponLocation = null; // Location to instantiate the weapons. NB: This should be a child object of the player camera.

	private readonly List<WeaponBase> weaponInventory = new List<WeaponBase>(); // Weapon Inventory

	[ReadOnly] [SerializeField] private WeaponBase activeWeapon;
		// Currently active weapon, pulled from the Weapon Inventory

	// Input events are handled via method delegation per update. TODO implement an InputManager
	public void Update()
	{
		// Handle primary fire input
		if (Input.GetButtonDown("Fire1"))
		{
			activeWeapon.StartFire();
		}
		else if (Input.GetButtonUp("Fire1"))
		{
			activeWeapon.EndFire();
		}

		// Handle mouse scroll input
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			NextWeapon();
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			LastWeapon();
		}

		// Handle reload input
		if (Input.GetButtonDown("Reload"))
		{
			activeWeapon.Reload();
		}

		// Handle weapon drop button
		if (Input.GetButtonDown("Drop Weapon"))
		{
			DropActiveWeapon();
		}
	}

	public void Start()
	{
		if (ValidateImperatives())
		{
			var weapon = Instantiate(defaultWeapon);
			if (weapon != null)
			{
				PickUpWeapon(weapon.GetComponent<WeaponBase>());
			}
		}
	}

	// Use to validate all specified imperitives.
	private bool ValidateImperatives()
	{
		var success = true;

		if (defaultWeapon == null)
		{
			success = false;
			Debug.LogError("Default Weapon has not been specified or is not valid!");
		}
		if (weaponLocation == null)
		{
			success = false;
			Debug.LogError("Weapon Location has not been specified or is not valid!");
		}

		return success;
	}

	public void Reset()
	{
		weaponInventory.Clear();
		activeWeapon = null;
	}

	// Performs checks and equips a WeaponBase object. Called via trigger event on weapons.
	public void PickUpWeapon(WeaponBase weapon)
	{
		if (!PlayerHasWeapon(weapon))
		{
			weapon.OnPickup(weaponLocation.transform);
			weaponInventory.Add(weapon);
			SetActiveWeapon(weapon);
		}
	}

	// Equip the next weapon in the weapon inventory
	private void NextWeapon()
	{
		SetActiveWeapon(weaponInventory.WeaponAfter(activeWeapon));
	}

	// Equip the previous weapon in the weapon inventory
	private void LastWeapon()
	{
		SetActiveWeapon(weaponInventory.WeaponBefore(activeWeapon));
	}

	// Set the active weapon. Checks for existance of a weapon type in the weapon inventory.
	private void SetActiveWeapon(WeaponBase weapon)
	{
		if (!weapon.Equals(activeWeapon) && PlayerHasWeapon(weapon))
		{
			// weapon is not currently active yes the player does possess it
			// Perform switch
			if (activeWeapon != null) activeWeapon.gameObject.SetActive(false);
			activeWeapon = weapon;
			activeWeapon.gameObject.SetActive(true);
		}
	}

	// Checks wether the provided weapon's type exists in the weapon inventory
	public bool PlayerHasWeapon(WeaponBase weapon)
	{
		return weaponInventory.GetWeapon(weapon) != null;
	}

	// Drop the weapon of provided type from the player's inventory
	public void DropWeapon(WeaponBase weapon)
	{
		if (weaponInventory.Count == 1) return;
		var clone = weaponInventory.GetWeapon(weapon);

		// Check if the weapon object was found in the list
		if (clone != null)
		{
			// Check if the weapon object is the active weapon
			if (clone.Equals(activeWeapon))
			{
				// The weapon object is the active weapon
				DropActiveWeapon();
			}
			else
			{
				// Set true as weapon is inactive (Not the active weapon)
				clone.gameObject.SetActive(true);
				clone.OnDrop();
				weaponInventory.Remove(clone);
			}
		}
	}

	// Drop the currently active weapon. Active weapon is switched out before drop.
	public void DropActiveWeapon()
	{
		if (weaponInventory.Count == 1) return;
		var clone = activeWeapon;
		// Switch to prior inventory weapon
		LastWeapon();
		// Set the weapon as active as the LastWeapon() call deactivated it
		clone.gameObject.SetActive(true);
		clone.OnDrop();
		weaponInventory.Remove(clone);
		Debug.Log(clone);
	}
}