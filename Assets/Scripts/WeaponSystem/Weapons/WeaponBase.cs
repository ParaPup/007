using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public int ammoCount;                       // Total ammunition.
    public int costPerShot;                     // Ammunition expense per shot.
    public int ammoInMag;                       // Ammo in the magazine.
    public int magCapacity;                     // Magazine capacity.
    public string weaponName;                   // Name of weapon.
    public float cooldownBetweenTrigger;        // Cooldown time between shots from the trigger (in seconds).
    public float reloadTime;                    // Time to reload (in seconds).
    
    public AudioClip emptyClipSound;            // Empty Clip sound clip.
    public AudioClip reloadSound;               // Reloading sound clip.
    public AudioClip fireSound;                 // Weapon fire sound clip.

    public GameObject weaponAmmunitionType;     // Ammunition prefab (must contain an ammunition component) that
                                                // the weapon accepts as valid ammunition.
    
    protected AudioSource audioEmitter = null;    // Audio emitter on Weapon object to emit firing (etc) sounds.
    protected float lastShotTime = 0;           // Time last shot was fired.
    private GameObject owner = null;            // Reference to Actor that is in posession of the weapon.
    private bool reloading = false;             // Is the Actor currently reloading?
    private float reloadStartTime = 0;          // Time reload began.

    public virtual void Reset()
    {
        // Default member values
        ammoCount = 10;
        costPerShot = 1;
        ammoInMag = 5;
        magCapacity = 5;
        cooldownBetweenTrigger = 1.0f;
        reloadTime = 3.0f;
        weaponName = "";
        audioEmitter = GetComponent<AudioSource>();
        if (audioEmitter == null)
        {
            Debug.LogError("Weapon object does not contain an Audio Emitter (AudioSource)");
        }

        Init();
    }

    protected bool CanFire()
    {
        var currTime = Time.time;
        // Trigger cooldown
        if (currTime - lastShotTime > cooldownBetweenTrigger)
        {
            // Reloading?
            if (reloading && currTime - reloadStartTime > reloadTime)
            {
                reloading = false;
                lastShotTime = currTime;
                return true;
            }
        }
        return false;
    }

    public void Reload()
    {
        if (ammoCount > 0)
        {
            if (ammoInMag != magCapacity)
            {
                // Reload!
                reloading = true;
                reloadStartTime = Time.time;
                if (reloadSound != null)
                {
                    audioEmitter.PlayOneShot(reloadSound, 0.9f);
                }
            }
        }
        
    }

    protected void ReduceAmmo(int numToReduceBy)
    {
        if (ammoCount > 0)
        {
            if (ammoInMag > 0)
            {
                ammoInMag--;
                ammoCount--;
            }
            else
            {
                Reload();
            }
        }
        else
        {
            if (emptyClipSound != null)
            {
                audioEmitter.PlayOneShot(emptyClipSound, 0.9f);
            }
        }
    }

    protected abstract void Init();

    public abstract void Fire();

    public virtual void OnPickup(GameObject newOwner)
    {
        if (newOwner != null)
        {
            owner = newOwner;
            owner.GetComponent<WeaponManager>().PickUpWeapon(this);
        }
        else
        {
            Debug.LogError("No owning GameObject specified! Nobody to Pick-up!");
        }
    }

    public virtual void OnDrop()
    {
        if (owner != null)
        {
            owner.GetComponent<WeaponManager>().DropWeapon(this);
        }
        else
        {
            Debug.LogError("No owning GameObject!");
        }
    }
}