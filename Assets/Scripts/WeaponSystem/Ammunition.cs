using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmunitionType
{
    PistolAmmo,
    RifleAmmo,
    ShotgunCartridge,
    HandGrenade,
    Rocket,
    RemoteMine,
    ProximityMine,
    TimedMine,
    ThrowingKnife,
    GrenadeRound,
    MagnumBullet,
    GoldenBullet,
    Bug,
    GoldeneyeKey,
    Plastique,
    WatchLaserBattery,
    WatchMagnetAttract,
    TankShell
}

[Serializable]
public class AmmunitionInvItem
{
    [SerializeField]
    public Ammunition ammunition = new Ammunition();
    [SerializeField]
    public int max;
    [SerializeField]
    public GameObject ammunitionPrefab;

    public AmmunitionInvItem(AmmunitionType ammunitionType, int count, int max)
    {
        ammunition.ammunitionType = ammunitionType;
        this.max = max;
        ammunition.ammoCount = count <= max ? count : 0;
    }

    public void AddRounds(Ammunition ammo)
    {
        if (ammo.ammunitionType == ammunition.ammunitionType)
        {
            var potentialAmmo = ammunition.ammoCount + ammo.ammoCount;
            ammunition.ammoCount = potentialAmmo <= max ? potentialAmmo : max;
        }
    }
}

[Serializable]
public class Ammunition
{
    [SerializeField]
    public AmmunitionType ammunitionType;
    [SerializeField]
    public int ammoCount;
    [SerializeField]
    public bool usesClip;


    // Hacky Unity drawer fix. TODO implement property drawer
    [SerializeField]
    private int roundsPerClip;
    [SerializeField]
    private int roundsInClip;

    public Ammunition(AmmunitionType ammunitionType, int ammoCount, int roundsPerClip, int roundsInClip, bool usesClip)
    {
        this.ammunitionType = ammunitionType;
        this.ammoCount = ammoCount;
        RoundsPerClip = roundsPerClip;
        RoundsInClip = roundsInClip;
        this.usesClip = usesClip;
    }

    public Ammunition(Ammunition ammunition)
        : this(ammunition.ammunitionType, ammunition.ammoCount, ammunition.RoundsPerClip, ammunition.RoundsInClip, ammunition.usesClip)
    {
    }

    public Ammunition()
    {

    }

    [SerializeField]
    public int RoundsPerClip
    {
        get { return roundsPerClip; }
        set
        {
            if (usesClip)
                roundsPerClip = value;
            else
                roundsInClip = -1;
        }
    }

    //[SerializeField]
    public int RoundsInClip
    {
        get { return roundsInClip; }
        set
        {
            if (usesClip)
            {
                if (value <= RoundsPerClip)
                    roundsInClip = value;
            }
            else
                roundsInClip = -1;
        }
    }

    public bool Reload()
    {
        if (usesClip)
        {
            if (ammoCount > 0 && RoundsInClip != RoundsPerClip)
            {
                RoundsInClip = RoundsPerClip;
                return true;
            }
        }
        return false;
    }

    public bool DispenseRound()
    {
        if (ammoCount > 0)
        {
            if (usesClip)
            {
                if (RoundsInClip == 0) return false;
                RoundsInClip--;
            }
            ammoCount--;
            return true;
        }
        return false;
    }
}

public static class AmmunitionInvItemExtensions
{
    public static AmmunitionInvItem GetByType(this List<AmmunitionInvItem> ammos, AmmunitionType type)
    {
        foreach (var ammo in ammos)
        {
            if (ammo.ammunition.ammunitionType == type)
            {
                return ammo;
            }
        }
        return null;
    }
}
