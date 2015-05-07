using UnityEngine;

public class WeaponSemiAuto : WeaponBase
{
    // Sensible defaults
    protected override void Init()
    {
        weaponName = "Semi-Auto Firearm";

        reloadTime = 1.5f;
        damagePerShot = 25.0f;
        cooldownBetweenTrigger = 0.25f;

        defaultAmmo.ammunitionType = AmmunitionType.PistolAmmo;
        defaultAmmo.usesClip = true;
        defaultAmmo.RoundsPerClip = 5;
        defaultAmmo.RoundsInClip = 5;
    }

    protected override void OnFire()
    {
        FireSingleShotRay(transform.TransformDirection(Vector3.forward));
    }

    protected override void OnEndFire()
    {
    }
}
