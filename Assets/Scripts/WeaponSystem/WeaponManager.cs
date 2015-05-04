using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public GameObject defaultWeaponPrefab = null;	// Default weapon to equip the player with
	public Transform weaponLocation = null;			// Location to instantiate the weapons.
													// NB: This should be a child object of the player camera.

	private readonly List<WeaponBase> weaponInventory = new List<WeaponBase>(); // Weapon Inventory
	private readonly List<AmmunitionInvItem> ammoInventory = new List<AmmunitionInvItem>(); // Ammunition Inventory

	// Ammunition Inventory
	[SerializeField] private AmmunitionInvItem pistolAmmo				= new AmmunitionInvItem(AmmunitionType.PistolAmmo, 0, 0);
	[SerializeField] private AmmunitionInvItem rifleAmmo				= new AmmunitionInvItem(AmmunitionType.RifleAmmo, 0, 0);
	[SerializeField] private AmmunitionInvItem shotgunCartridges		= new AmmunitionInvItem(AmmunitionType.ShotgunCartridge, 0, 0);
	[SerializeField] private AmmunitionInvItem handGrenades				= new AmmunitionInvItem(AmmunitionType.HandGrenade, 0, 0);
	[SerializeField] private AmmunitionInvItem rockets					= new AmmunitionInvItem(AmmunitionType.Rocket, 0, 0);
	[SerializeField] private AmmunitionInvItem remoteMines				= new AmmunitionInvItem(AmmunitionType.RemoteMine, 0, 0);
	[SerializeField] private AmmunitionInvItem proximityMines			= new AmmunitionInvItem(AmmunitionType.ProximityMine, 0, 0);
	[SerializeField] private AmmunitionInvItem timedMines				= new AmmunitionInvItem(AmmunitionType.TimedMine, 0, 0);
	[SerializeField] private AmmunitionInvItem throwingKnives			= new AmmunitionInvItem(AmmunitionType.ThrowingKnife, 0, 0);
	[SerializeField] private AmmunitionInvItem grenadeRounds			= new AmmunitionInvItem(AmmunitionType.GrenadeRound, 0, 0);
	[SerializeField] private AmmunitionInvItem magnumBullets			= new AmmunitionInvItem(AmmunitionType.MagnumBullet, 0, 0);
	[SerializeField] private AmmunitionInvItem goldenBullets			= new AmmunitionInvItem(AmmunitionType.GoldenBullet, 0, 0);
	[SerializeField] private AmmunitionInvItem bugs						= new AmmunitionInvItem(AmmunitionType.Bug, 0, 0);
	[SerializeField] private AmmunitionInvItem goldeneyeKeys			= new AmmunitionInvItem(AmmunitionType.GoldeneyeKey, 0, 0);
	[SerializeField] private AmmunitionInvItem plastiques				= new AmmunitionInvItem(AmmunitionType.Plastique, 0, 0);
	[SerializeField] private AmmunitionInvItem watchLaserBattries		= new AmmunitionInvItem(AmmunitionType.WatchLaserBattery, 0, 0);
	[SerializeField] private AmmunitionInvItem watchMagnetAttractRounds	= new AmmunitionInvItem(AmmunitionType.WatchMagnetAttract, 0, 0);
	[SerializeField] private AmmunitionInvItem tankShells				= new AmmunitionInvItem(AmmunitionType.TankShell, 0, 0);

	[ReadOnly] [SerializeField] private WeaponBase activeWeapon; // Currently active weapon, pulled from the Weapon Inventory

	private WeaponBase defaultWeapon; // Default weapon reference, pulled from the Weapon Inventory

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

	public void Awake()
	{
		ammoInventory.Add(pistolAmmo);
		ammoInventory.Add(rifleAmmo);
		ammoInventory.Add(shotgunCartridges);
		ammoInventory.Add(handGrenades);
		ammoInventory.Add(rockets);
		ammoInventory.Add(remoteMines);
		ammoInventory.Add(proximityMines);
		ammoInventory.Add(timedMines);
		ammoInventory.Add(throwingKnives);
		ammoInventory.Add(grenadeRounds);
		ammoInventory.Add(magnumBullets);
		ammoInventory.Add(goldenBullets);
		ammoInventory.Add(bugs);
		ammoInventory.Add(goldeneyeKeys);
		ammoInventory.Add(plastiques);
		ammoInventory.Add(watchLaserBattries);
		ammoInventory.Add(watchMagnetAttractRounds);
		ammoInventory.Add(tankShells);
	}

	public void Start()
	{
		if (ValidateImperatives())
		{
			var weapon = Instantiate(defaultWeaponPrefab);
			defaultWeapon = weapon.GetComponent<WeaponBase>();
			if (defaultWeapon != null)
			{
				PickUpWeapon(defaultWeapon);
			}
			else
			{
				Debug.LogError("Default Weapon Prefab is incorrectly configured! It does not contain a WeaponBase component");
			}
		}
	}

	// Use to validate all specified imperitives.
	private bool ValidateImperatives()
	{
		var success = true;

		if (defaultWeaponPrefab == null)
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
			weapon.OnPickup(weaponLocation.transform, ammoInventory.GetByType(weapon.ammo.ammunitionType));
			weaponInventory.Add(weapon);
			SetActiveWeapon(weapon);
		}
	}

	public void PickUpAmmo(AmmunitionComponent ammoComp)
	{
		// See if we can find the ammo AmmunitionType in the ammo inventory
		var clone = ammoInventory.GetByType(ammoComp.ammunition.ammunitionType);
		if (clone != null)
		{
			// We were able to find the ammo AmmunitionType in the ammo inventory
			clone.AddRounds(ammoComp.ammunition);
            ammoComp.OnPickup(gameObject.transform);
		}
	}

	// Drop the weapon of provided AmmunitionType from the player's inventory
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
		if (activeWeapon.Equals(defaultWeapon)) return;
		var clone = activeWeapon;
		// Switch to prior inventory weapon
		LastWeapon();
		// Set the weapon as active as the LastWeapon() call deactivated it
		clone.gameObject.SetActive(true);
		clone.OnDrop();
		weaponInventory.Remove(clone);
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

	// Set the active weapon. Checks for existance of a weapon AmmunitionType in the weapon inventory.
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

	// Checks wether the provided weapon's AmmunitionType exists in the weapon inventory
	public bool PlayerHasWeapon(WeaponBase weapon)
	{
		return weaponInventory.GetWeapon(weapon) != null;
	}
}
