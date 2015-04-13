using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public int ammoCount;                   // Total ammunition.
    public int costPerShot;                 // Ammunition expense per shot.
    public string weaponName;               // Name of weapon.
    public float cooldownBetweenShots;      // Cooldown time between shots (in seconds).

    private float lastShotTime = 0;         // Time last shot was fired.
    private GameObject owner = null;        // Reference to Actor that is in posession of the weapon.

    public enum FireMode
    {
        SemiAuto,
        Auto,
        OneShot,
        Melee
    }

    public FireMode fireMode;

    public virtual void Reset()
    {
        // Default member values
        ammoCount = 10;
        costPerShot = 1;
        cooldownBetweenShots = 0.1f;
        weaponName = "";

        Init();
    }

    protected abstract void Init();

    public bool Fire()
    {
        bool success = false;
        switch (fireMode)
        {
            case FireMode.SemiAuto:
                // Check if the time since the last shot is greater than the cooldown time
                if (Time.time - lastShotTime > cooldownBetweenShots)
                {
                    // Check for ammunition
                    if (ammoCount > 0)
                    {
                        // Fire!
                        ammoCount -= costPerShot;
                        OnFire();
                        lastShotTime = Time.time;
                        success = true;
                    }
                    else
                    {
                        // Empty!
                        OnEmpty();
                    }
                }
                break;

            case FireMode.Auto:

                break;

            case FireMode.OneShot:

                break;

            case FireMode.Melee:
                OnFire();
                break;

            default:
                Debug.LogError("FireMode not valid!");
                break;
        }
        return success;
    }

    protected abstract void OnFire();

    protected abstract void OnEmpty();

    public virtual void OnPickup(GameObject newOwner)
    {
        if (newOwner != null)
        {
            owner = newOwner;
            owner.GetComponent<WeaponManager>().PickUp(this);
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
            owner.GetComponent<WeaponManager>().Drop(this);
        }
        else
        {
            Debug.LogError("No owning GameObject!");
        }
    }
}