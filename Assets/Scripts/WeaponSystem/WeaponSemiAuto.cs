using UnityEngine;

public class WeaponSemiAuto : WeaponBase
{
    protected override void Init()
    {
        weaponName = "Semi-Auto Firearm";
        cooldownBetweenTrigger = 0.5f;
        damagePerShot = 15.0f;
    }

    public override void StartFire()
    {
        if (CanFire())
        {
	        if (ammo.ammoCount > 0)
	        {
				if (ammo.DispenseRound())
				{
					if (fireSound != null)
						audioEmitter.PlayOneShot(fireSound, 0.9f);
				    FireSingleShotRay(transform.TransformDirection(Vector3.forward));
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

    public override void EndFire()
    {
    }
}
