using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    private List<WeaponBase> weaponInventory = new List<WeaponBase>();

    public void Reset()
    {
        weaponInventory.Clear();
    }

    public void PickUpWeapon(WeaponBase weapon)
    {
        bool alreadyContains = false;

        foreach (var weap in weaponInventory)
        {
            if (weap.weaponName == weapon.weaponName)
            {
                alreadyContains = true;
            }
        }

        if (!alreadyContains)
        {
            weaponInventory.Add(weapon);
        }
    }

    public void DropWeapon(WeaponBase weapon)
    {
        bool contains = false;

        foreach (var weap in weaponInventory)
        {
            if (weap.weaponName == weapon.weaponName)
            {
                weaponInventory.Remove(weapon);
                contains = true;
            }
        }

        if (contains)
        {
            Debug.Log("Weapon removed from agent");
        }
        else
        {
            Debug.Log("Agent doesnt contain weapon " + weapon.GetType());
        }
    }
	
}
