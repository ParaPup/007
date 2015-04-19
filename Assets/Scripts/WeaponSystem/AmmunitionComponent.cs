using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmunitionType
{
	PistolAmmo,
	RifleAmmo,
	ShotgunCartridge,
	HandGrenade,
	Rocket,
	RemoteMine,
	ProximityMine,
	TimedMine,
	ThrowingKnife,
	GrenadeRound,
	MagnumBullet,
	GoldenBullet,
	Bug,
	GoldeneyeKey,
	Plastique,
	WatchLaserBattery,
	WatchMagnetAttract,
	TankShell
}

[Serializable]
public class AmmunitionInvItem
{
	[SerializeField] public Ammunition ammunition;
	[SerializeField] public int max;
	[SerializeField] public GameObject ammunitionPrefab;

	public AmmunitionInvItem(AmmunitionType ammunitionType, int count, int max)
	{
		ammunition.ammunitionType = ammunitionType;
		this.max = max;
		ammunition.ammoCount = count <= max ? count : 0;
	}

	public void AddRounds(Ammunition ammo)
	{
		if (ammo.ammunitionType == ammunition.ammunitionType)
		{
			var potentialAmmo = ammunition.ammoCount + ammo.ammoCount;
			ammunition.ammoCount = potentialAmmo <= max ? potentialAmmo : max;
		}
	}
}

[Serializable]
public struct Ammunition
{
	[SerializeField] public AmmunitionType ammunitionType;
	[SerializeField] public int ammoCount;
	[SerializeField] public bool usesClip;


	// Hacky Unity drawer fix. TODO implement property drawer
	[SerializeField] private int roundsPerClip;
	[SerializeField] private int roundsInClip;

	public Ammunition(AmmunitionType ammunitionType, int ammoCount, int roundsPerClip, int roundsInClip, bool usesClip) : this()
	{
		this.ammunitionType = ammunitionType;
		this.ammoCount = ammoCount;
		RoundsPerClip = roundsPerClip;
		RoundsInClip = roundsInClip;
		this.usesClip = usesClip;
	}

	public Ammunition(Ammunition ammunition) : this()
	{
		ammunitionType = ammunition.ammunitionType;
		ammoCount = ammunition.ammoCount;
		usesClip = ammunition.usesClip;
		RoundsPerClip = ammunition.RoundsPerClip;
		RoundsInClip = ammunition.RoundsInClip;
	}

	[SerializeField]
	public int RoundsPerClip
	{
		get { return roundsPerClip; }
		set
		{
			if (usesClip)
				roundsPerClip = value;
			else
				roundsInClip = -1;
		}
	}

	//[SerializeField]
	public int RoundsInClip
	{
		get { return roundsInClip; }
		set
		{
			if (usesClip)
			{
				if (value <= RoundsPerClip)
					roundsInClip = value;
			}
			else
				roundsInClip = -1;
		}
	}

	public bool Reload()
	{
		if (usesClip)
		{
			if (ammoCount > 0 && RoundsInClip == 0)
			{
				RoundsInClip = RoundsPerClip;
				return true;
			}
		}
		return false;
	}

	public bool DispenseRound()
	{
		if (ammoCount > 0)
		{
			if (usesClip)
			{
				if (RoundsInClip == 0) return false;
				RoundsInClip--;
			}
			ammoCount--;
			return true;
		}
		return false;
	}
}

// Proxy component for AmmunitionInvItem struct. Acts as a container holding an AmmunitionInvItem object with the specified values.
public class AmmunitionComponent : MonoBehaviour
{
	public Ammunition ammunition;
	public int count;

	[SerializeField] [ReadOnly] private Collider pickupCollisionComp;
    [SerializeField] [ReadOnly] private Rigidbody physicsComp;
    [SerializeField] [ReadOnly] private MeshFilter meshComp;
    [SerializeField] [ReadOnly] private MeshRenderer meshRendComp;
    [SerializeField] [ReadOnly] private Collider modelCollisionComp;
    [SerializeField] [ReadOnly] private Material materialComp;

	public void Awake()
	{
		physicsComp = GetComponent<Rigidbody>();
		pickupCollisionComp = GetComponent<Collider>();
		Transform model = transform.FindChild("Model");
		if (model != null)
		{
			meshComp = model.GetComponent<MeshFilter>();
			meshRendComp = model.GetComponent<MeshRenderer>();
			modelCollisionComp = model.GetComponent<Collider>();
			if (meshRendComp != null) materialComp = meshRendComp.material;
		}
		ValidateImperatives();
	}

	private void ValidateImperatives()
	{
		if (physicsComp == null)
		{
			Debug.LogError(name + " does not contain a Physics Component (RigidBody)");
		}
		if (pickupCollisionComp == null)
		{
			Debug.LogError(name + " does not contain a Pickup Collision Component (Collider)");
		}
		if (transform.FindChild("Model") == null)
		{
			Debug.LogError(name + " must contain a child object named \"Model\"");
		}
		if (meshComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Mesh Filter Component (MeshFilter)");
		}
		if (meshRendComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Renderable Mesh Component (MeshRenderer)");
		}
		if (modelCollisionComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Collision Component (Collider)");
		}
		if (materialComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Material Component (Material)");
		}
	}

	public void OnPickup(Transform actor)
	{
		// Disable physics properties
		physicsComp.isKinematic = true;
		physicsComp.detectCollisions = false;
		physicsComp.velocity = Vector3.zero;
		// Disable the Pickup components
		pickupCollisionComp.enabled = false;
		// Child object to actor
		transform.SetParent(actor);
		// Reposition weapon
		transform.localPosition = new Vector3(0, 0, 0);
		transform.localRotation = Quaternion.identity;
	}

	public void OnDrop()
	{
		// Re-enable Physics properties
		physicsComp.isKinematic = false;
		physicsComp.detectCollisions = true;
		transform.SetParent(null);
		// Re-enable the Pickup components
		StartCoroutine(ReenableCollisionAfterDelay(1.0f));
		// "Throw" weapon forward
		physicsComp.AddForce(transform.forward * 8, ForceMode.Impulse);
	}

	// Handle pickup collision
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			var clone = other.GetComponent<WeaponManager>();
			if (clone != null) clone.PickUpAmmo(this.ammunition);
		}
	}

	// Coroutine that delays the reactivation of the pickup physics component by (float) seconds number of seconds
	private IEnumerator ReenableCollisionAfterDelay(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		pickupCollisionComp.enabled = true;
	}
}

public static class AmmunitionInvItemExtensions
{
	public static AmmunitionInvItem GetByType(this List<AmmunitionInvItem> ammos, AmmunitionType type)
	{
		foreach (var ammo in ammos)
		{
			if (ammo.ammunition.ammunitionType == type)
			{
				return ammo;
			}
		}
		return null;
	}
}