using UnityEngine;

public class WeaponSemiAuto : WeaponBase
{
    protected override void Init()
    {
        weaponName = "Semi-Auto Firearm";
        cooldownBetweenTrigger = 0.5f;
    }

    public override void StartFire()
    {
        if (CanFire())
        {
            if (ammoCount > 0)
            {
                if (ammoInMag > 0)
                {
                    // Fire
                    ammoInMag--;
                    ammoCount--;
                    if (fireSound != null)
                    {
                        audioEmitter.PlayOneShot(fireSound, 0.9f);
                    }
                    Debug.Log("Pistol: BANG");
                }
                else
                {
                    // Reload
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
    }

    public override void EndFire()
    {
    }
}
