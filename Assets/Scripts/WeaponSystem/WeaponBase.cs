using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public int ammoCapacity;                        // Maximum ammunition. TODO implement
    public int magCapacity;                         // Magazine capacity.
    public int ammoCount;                           // Total ammunition.
    public int ammoInMag;                           // Ammo in the magazine.
    public string weaponName;                       // Name of weapon.
    public float cooldownBetweenTrigger;            // Cooldown time between shots from the trigger (in seconds).
    public float reloadTime;                        // Time to reload (in seconds).
    
    public AudioClip emptyClipSound;                // Empty Clip sound clip.
    public AudioClip reloadSound;                   // Reloading sound clip.
    public AudioClip fireSound;                     // Weapon fire sound clip.

    public AmmunitionType weaponAmmunitionType;     // Ammunition type that
                                                    // the weapon accepts as valid ammunition.
    
    protected float lastShotTime = 0;               // Time last shot was fired.
    //private GameObject owner = null;              // Reference to Actor that is in posession of the weapon.
    private bool reloading = false;                 // Is the Actor currently reloading?
    private float reloadStartTime = 0;              // Time reload began.

    // Required object components. Visible in inspector for debugging purposes
    [SerializeField] [ReadOnly] protected AudioSource audioEmitter;
    [SerializeField] [ReadOnly] protected Collider pickupCollisionComp;
    [SerializeField] [ReadOnly] protected Rigidbody physicsComp;
    [SerializeField] [ReadOnly] protected MeshFilter meshComp ;
    [SerializeField] [ReadOnly] protected MeshRenderer meshRendComp;
    [SerializeField] [ReadOnly] protected Collider modelCollisionComp;
    [SerializeField] [ReadOnly] protected Material materialComp;

    public void Awake()
    {
        physicsComp = GetComponent<Rigidbody>();
        pickupCollisionComp = GetComponent<Collider>();
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

    public virtual void Reset()
    {
        // Sensible defaults
        ammoCount = 10;
        ammoInMag = 5;
        magCapacity = 5;
        cooldownBetweenTrigger = 1.0f;
        reloadTime = 3.0f;
        weaponName = "";
        Init();
    }

    // Use to validate all specified imperitives. Called as a part of the MonoBehaviour Awake() sequence.
    protected virtual void  ValidateImperatives()
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
    protected void Reload()
    {
        if (ammoCount > 0)
        {
            // We are not empty
            if (ammoInMag != magCapacity)
            {
                // Reload the magazine
                reloading = true;
                reloadStartTime = Time.time;
                // Audible feedback on reload
                if (reloadSound != null)
                {
                    audioEmitter.PlayOneShot(reloadSound, 0.9f);
                }
                ammoInMag = ammoCount >= magCapacity ? magCapacity : ammoCount;
            }
        }
        
    }

    // Weapon specialisation specific member initialisation method.
    // weaponName MUST be specified here
    protected abstract void Init();

    // Called when the player begins the fire sequence (Fire key is pressed down)
    public abstract void StartFire();

    // Called when the player ends the fire sequence (Fire key is released)
    public abstract void EndFire();

    // Caled when the weapon is possessed by an actor
    public virtual void OnPickup(Transform actor)
    {
        // Disable Physics properties
        physicsComp.isKinematic = true;
        physicsComp.detectCollisions = false;
        physicsComp.velocity = Vector3.zero;
        // Disable the Pickup components
        pickupCollisionComp.enabled = false;
        // Child object to actor
        transform.SetParent(actor);
        // Reposition weapon
        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;
    }

    // Called when the actor holding the weapon loses possession of the weapon
    public virtual void OnDrop()
    {
        // Re-enable Physics properties
        physicsComp.isKinematic = false;
        physicsComp.detectCollisions = true;
        transform.SetParent(null);
        // Re-enable the Pickup components
        StartCoroutine(ReenableCollisionDelay(1.0f));
        // "Throw" weapon forward
        physicsComp.AddRelativeForce(transform.forward * 8, ForceMode.Impulse);
    }

    // Coroutine that delays the reactivation of the pickup physics component by (float) seconds number of seconds
    private IEnumerator ReenableCollisionDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        pickupCollisionComp.enabled = true;
    }

    // System.Object.Equals override. Checks the weaponName's of the compared WeaponBase objects for equality.
    public override bool Equals(object o)
    {
        if (ReferenceEquals(null, o)) return false;
        if (ReferenceEquals(this, o)) return true;
        if (o.GetType() != this.GetType()) return false;
        return Equals((WeaponBase) o);
    }

    protected bool Equals(WeaponBase other)
    {
        return string.Equals(weaponName, other.weaponName);
    }
}
