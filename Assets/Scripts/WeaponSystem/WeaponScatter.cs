using UnityEngine;

public class WeaponScatter : WeaponBase
{
    public int numberOfPelletsPerShot = 1;  // Number of pellets in a single shot
    public float shotSpread = 0.1f;         // Pellet spread (direction angle delta)

    // Sensible defaults
    protected override void Init()
    {
        weaponName = "Scattergun";

        cooldownBetweenTrigger = 0.15f;
        damagePerShot = 15.0f;
        reloadTime = 1.5f;

        defaultAmmo.ammunitionType = AmmunitionType.ShotgunCartridge;
        defaultAmmo.usesClip = true;
        defaultAmmo.RoundsPerClip = 2;
        defaultAmmo.RoundsInClip = 2;
    }

    protected override void OnFire()
    {
        FireMultipleShotRays(transform.TransformDirection(Vector3.forward), numberOfPelletsPerShot, shotSpread);
    }

    protected override void OnEndFire()
    {
    }
}
