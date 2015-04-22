using UnityEngine;

public class WeaponScatter : WeaponBase
{
    public int numberOfPelletsPerShot = 1;
    public float shotSpread = 0.1f;

    protected override void Init()
    {
        weaponName = "Scattergun";
        cooldownBetweenTrigger = 0.15f;
        damagePerShot = 15.0f;
        defaultAmmo.ammunitionType = AmmunitionType.ShotgunCartridge;
        defaultAmmo.usesClip = true;
        defaultAmmo.RoundsPerClip = 2;
        defaultAmmo.RoundsInClip = 2;
        reloadTime = 1.5f;
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
                    FireMultipleShotRays(transform.TransformDirection(Vector3.forward), numberOfPelletsPerShot, shotSpread);
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