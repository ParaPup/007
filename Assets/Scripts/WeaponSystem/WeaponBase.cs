using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName;                       // Name of weapon.
    public float cooldownBetweenTrigger;            // Cooldown time between shots from the trigger (in seconds).
    public float reloadTime;                        // Time to reload (in seconds).
    public float damagePerShot;

    public bool usesParticleForShot = true;         // Does the weapon use a particle system to represent a bullet shot?


    public Ammunition defaultAmmo = new Ammunition();
    [ReadOnly]
    public Ammunition ammo;

    // TODO usesAmmo bool for unarmed/unlimitd weapons

    public AudioClip emptyClipSound;                // Empty Clip sound clip.
    public AudioClip reloadSound;                   // Reloading sound clip.
    public AudioClip fireSound;                     // Weapon fire sound clip.
    
    protected float lastShotTime = 0;               // Time last shot was fired.
    private bool reloading = false;                 // Is the Actor currently reloading?
    private float reloadStartTime = 0;              // Time reload began.

    // Required object components. Visible in inspector for debugging purposes
    [SerializeField] [ReadOnly] protected AudioSource audioEmitter;
    [SerializeField] [ReadOnly] protected Collider pickupCollisionComp;
    [SerializeField] [ReadOnly] protected Rigidbody physicsComp;
    [SerializeField] [ReadOnly] protected MeshFilter meshComp;
    [SerializeField] [ReadOnly] protected MeshRenderer meshRendComp;
    [SerializeField] [ReadOnly] protected Collider modelCollisionComp;
    [SerializeField] [ReadOnly] protected Material materialComp;
    [SerializeField] [ReadOnly] protected GameObject shotParticle;

    public void Awake()
    {
        ammo = defaultAmmo;
        physicsComp = GetComponent<Rigidbody>();
        pickupCollisionComp = GetComponent<Collider>();
        if (usesParticleForShot && GetComponent<ParticleSystem>() != null)
            shotParticle = GetComponent<ParticleSystem>().gameObject;
        Transform model = transform.FindChild("Model");
        if (model != null)
        {
            audioEmitter = model.GetComponent<AudioSource>();
            meshComp = model.GetComponent<MeshFilter>();
            meshRendComp = model.GetComponent<MeshRenderer>();
            modelCollisionComp = model.GetComponent<Collider>();
            if (meshRendComp != null) materialComp = meshRendComp.material;
        }
        ValidateImperatives();
    }

    public void Start()
    {
        ValidateImperatives();
    }

    public virtual void Reset()
    {
        // Sensible defaults
        ammo.ammoCount = 10;
		ammo.usesClip = true;
		ammo.RoundsPerClip = 5;
	    ammo.RoundsInClip = 5;
        damagePerShot = 20.0f;
        cooldownBetweenTrigger = 1.0f;
        reloadTime = 3.0f;
        weaponName = "";
        Init();
    }

    // Used to validate all specified imperitives. Called as a part of the MonoBehaviour Awake() sequence.
    private void  ValidateImperatives()
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
        if (audioEmitter == null)
        {
            Debug.LogError(name + "'s Model does not contain an Audio Emitter (AudioSource)");
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
        if (usesParticleForShot && (shotParticle == null || shotParticle.GetComponent<ParticleSystem>() == null))
        {
            Debug.LogError(name + "'s Weapon uses a particle system that is not defined");
        }
	}

	// Caled when the weapon is possessed by an actor
	public virtual void OnPickup(Transform actor, AmmunitionInvItem ammo)
	{
        
        // Specify ammo ref
	    ammo.AddRounds(this.ammo);
        this.ammo = ammo.ammunition;
		// Disable Physics properties
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

	// Called when the actor holding the weapon loses possession of the weapon
	public virtual void OnDrop()
	{
        // Remove ammo ref
        ammo = new Ammunition();
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
			if (clone != null) clone.PickUpWeapon(this);
		}
	}

	// Coroutine that delays the reactivation of the pickup physics component by (float) seconds number of seconds
	private IEnumerator ReenableCollisionAfterDelay(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		pickupCollisionComp.enabled = true;
	}

    // Checks wether the weapon has any active cooldowns preventing firing.
    // Examples: cooldownBetweenTrigger and reloadTime
    protected bool CanFire()
    {
        var currTime = Time.time;
        if (currTime - lastShotTime > cooldownBetweenTrigger)
        {
            // We have no trigger cooldown
            if (!reloading)
            {
                // We are reloading
                lastShotTime = Time.time;
                return true;
            }
            else if (currTime - reloadStartTime > reloadTime)
            {
                // We have finished reloading
                reloading = false;
                lastShotTime = Time.time;
                return true;
            }
        }
        return false;
    }

    // Attempt to reload the weapon
    public virtual void Reload()
    {
	    if (ammo.Reload())
	    {
		    reloading = true;
		    reloadStartTime = Time.time;
			// Audible feedback on reload
			if (reloadSound != null)
			{
				audioEmitter.PlayOneShot(reloadSound, 0.9f);
			}
	    }
    }

    // Ray cast a single ray from the camera with direction
    protected void FireSingleShotRay(Vector3 direction)
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera != null)
        {
            RaycastHit rayHit;
            // Query raycast for hit
            if (Physics.Raycast(camera.transform.position, direction, out rayHit, 1000))
            {
                if (rayHit.collider.gameObject.tag == "enemy")
                {
                    rayHit.collider.gameObject.SendMessage("hit", damagePerShot);
                }
            }
            Debug.DrawRay(camera.transform.position, direction * 100, Color.yellow, 10.0f);
            if (usesParticleForShot)
            {
                //shotParticle.transform.rotation = direction;
                shotParticle.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    // Ray cast multiple rays from the camera with direction.
    protected void FireMultipleShotRays(Vector3 direction, int numShots, float directionDelta)
    {
        Vector3 shotDir, crossDir;
        for (int i = 0; i < numShots; i++)
        {
            shotDir = direction;
            crossDir = Vector3.Cross(shotDir, Vector3.up);
            crossDir = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), shotDir) * crossDir;
            crossDir = Random.Range(0.0f, directionDelta) * crossDir;
            shotDir += crossDir;

            FireSingleShotRay(shotDir);
        }
    }

    // Weapon specialisation specific member initialisation method.
    // weaponName MUST be specified here
    protected abstract void Init();

    // Weapon specialisation specific fire method
    protected abstract void OnFire();

    // Weapon specialisation specific end fire method
    protected abstract void OnEndFire();

    // Called when the player begins the fire sequence (Fire key is pressed down)
    public virtual void StartFire()
    {
        if (CanFire())
        {
            if (ammo.ammoCount > 0)
            {
                if (ammo.DispenseRound())
                {
                    OnFire();
                }
                else Reload();
            }
            else
            {
                if (emptyClipSound != null)
                    audioEmitter.PlayOneShot(emptyClipSound, 0.9f);
            }
        }
    }

    // Called when the player ends the fire sequence (Fire key is released)
    public virtual void EndFire()
    {
        OnEndFire();
    }

    // System.Object.Equals override. Checks the weaponName's of the compared WeaponBase objects for equality.
    public override bool Equals(object o)
    {
        if (ReferenceEquals(null, o)) return false;
        if (ReferenceEquals(this, o)) return true;
        if (o.GetType() != GetType()) return false;
        return Equals((WeaponBase) o);
    }

    private bool Equals(WeaponBase other)
    {
        return string.Equals(weaponName, other.weaponName);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = base.GetHashCode();
            hashCode = (hashCode * 397) ^ (weaponName != null ? weaponName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ cooldownBetweenTrigger.GetHashCode();
            hashCode = (hashCode * 397) ^ reloadTime.GetHashCode();
            hashCode = (hashCode * 397) ^ (ammo != null ? ammo.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (emptyClipSound != null ? emptyClipSound.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (reloadSound != null ? reloadSound.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (fireSound != null ? fireSound.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ lastShotTime.GetHashCode();
            hashCode = (hashCode * 397) ^ reloading.GetHashCode();
            hashCode = (hashCode * 397) ^ reloadStartTime.GetHashCode();
            hashCode = (hashCode * 397) ^ (audioEmitter != null ? audioEmitter.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (pickupCollisionComp != null ? pickupCollisionComp.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (physicsComp != null ? physicsComp.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (meshComp != null ? meshComp.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (meshRendComp != null ? meshRendComp.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (modelCollisionComp != null ? modelCollisionComp.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (materialComp != null ? materialComp.GetHashCode() : 0);
            return hashCode;
        }
    }
}

public static class WeaponBaseListExtensions
{
	// Gets the next weapon after currentWeapon in the List.
	// Returns null if currentWeapon is not present in the List.
	public static WeaponBase WeaponAfter(this List<WeaponBase> weapons, WeaponBase currentWeapon)
	{
		var weaponsSize = weapons.Count;
		for (var i = 0; i < weaponsSize; i++)
		{
			if (currentWeapon.Equals(weapons[i]))
			{
				return i + 1 < weaponsSize ? weapons[i + 1] : weapons[0];
			}
		}
		// currentWeapon does not exist in the list
		return null;
	}

	// Gets the weapon before currentWeapon in the List.
	// Returns null if currentWeapon is not present in the List.
	public static WeaponBase WeaponBefore(this List<WeaponBase> weapons, WeaponBase currentWeapon)
	{
		var weaponsSize = weapons.Count;
		for (var i = 0; i < weaponsSize; i++)
		{
			if (currentWeapon.Equals(weapons[i]))
			{
				return i > 0 ? weapons[0] : weapons[weaponsSize - 1];
			}
		}
		// currentWeapon does not exist in the list
		return null;
	}

	// Gets the weapon object in the List of the same AmmunitionType as weaponToSearchFor.
	// Returns null if weaponToSearchFor's AmmunitionType of weapon is not present in the list.
	public static WeaponBase GetWeapon(this List<WeaponBase> weapons, WeaponBase weaponToSearchFor)
	{
		foreach (var weapon in weapons)
		{
			if (weapon.Equals(weaponToSearchFor))
			{
				return weapon;
			}
		}
		// weaponToSearchFor's weapon AmmunitionType does not exist in the list
		return null;
	}
}
