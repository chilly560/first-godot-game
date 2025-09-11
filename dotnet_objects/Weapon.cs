using System;
using Godot;

public abstract class Weapon : IWeapon
{

    protected enum Bullet
    {
        Standard,
        Heavy,
        FiftyCal,
        RayGun
    }
    protected int Damage, Speed, Ammo, MaxAmmo;

    protected Bullet BulletType;


    public void addAmmo(int ammo)
    {
        this.Ammo += ammo;
    }

    public abstract void Reload();

    public void AddAmmo(int ammo)
    {
        this.Ammo += ammo;
    }

    public abstract void Shoot();
}