using UnityEngine;
using System.Collections;

public class WeaponSemiAuto : WeaponBase
{

    protected override void Init()
    {
        weaponName = "Semi-Auto Firearm";
        cooldownBetweenTrigger = 0.5f;
    }

    public override void Fire()
    {
        if (CanFire())
        {
            ReduceAmmo(costPerShot);
            if (fireSound != null)
            {
                audioEmitter.PlayOneShot(fireSound, 0.9f);
            }
            Debug.Log("Pistol: BANG");
        }
    }
}
