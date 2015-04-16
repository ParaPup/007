using System.Collections.Generic;

// WeaponBase and List<WeaponBase> class extensions
namespace WeaponBaseExtensions
{
    public static class Extensions
    {
        // Gets the next weapon after currentWeapon in the List.
        // Returns null if currentWeapon is not present in the List.
        public static WeaponBase WeaponAfter(this List<WeaponBase> weapons, WeaponBase currentWeapon)
        {
            var weaponsSize = weapons.Count;
            for (var i = 0; i < weaponsSize; i++)
            {
                if (currentWeapon.Equals(weapons[i]))
                {
                    return i + 1 < weaponsSize ? weapons[i + 1] : weapons[0];
                }
            }
            // currentWeapon does not exist in the list
            return null;
        }

        // Gets the weapon before currentWeapon in the List.
        // Returns null if currentWeapon is not present in the List.
        public static WeaponBase WeaponBefore(this List<WeaponBase> weapons, WeaponBase currentWeapon)
        {
            var weaponsSize = weapons.Count;
            for (var i = 0; i < weaponsSize; i++)
            {
                if (currentWeapon.Equals(weapons[i]))
                {
                    return i > 0 ? weapons[0] : weapons[weaponsSize - 1];
                }
            }
            // currentWeapon does not exist in the list
            return null;
        }

        // Gets the weapon object in the List of the same type as weaponToSearchFor.
        // Returns null if weaponToSearchFor's type of weapon is not present in the list.
        public static WeaponBase GetWeapon(this List<WeaponBase> weapons, WeaponBase weaponToSearchFor)
        {
            foreach (var w in weapons)
            {
                if (w.Equals(weaponToSearchFor))
                {
                    return w;
                }
            }
            // weaponToSearchFor's weapon type does not exist in the list
            return null;
        }
    }
}
