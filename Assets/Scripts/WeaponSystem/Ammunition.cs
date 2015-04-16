using UnityEngine;
using System.Collections;

// Ammunition types available
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
    WatchLasterBattery,
    WatchMagnetAttract,
    TankShell
}

public class Ammunition : MonoBehaviour
{
    public AmmunitionType ammunitionType;
    public int numberOfAmmo;
    // TODO display max number of rounds in ammo type in inspector

}
