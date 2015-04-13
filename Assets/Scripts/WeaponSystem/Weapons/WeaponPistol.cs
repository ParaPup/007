using UnityEngine;
using System.Collections;

public class WeaponPistol : WeaponBase {

    protected override void Init()
    {
        weaponName = "Pistol";
        cooldownBetweenShots = 0.3f;
        fireMode = FireMode.SemiAuto;
    }

    protected override void OnFire()
    {
        Debug.Log("Pistol: BANG");
    }

    protected override void OnEmpty()
    {
        Debug.Log("Pistol: CLICK");
    }
}
