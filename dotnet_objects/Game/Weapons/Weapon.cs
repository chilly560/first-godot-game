using System;
using Godot;

namespace Game.Weapons
{
    public abstract class Weapon : IWeapon
    {
        protected enum BulletClassification
        {
            Standard,
            Heavy,
            FiftyCal,
            RayGun,

            Explosive
        }
        protected int Damage, Speed, Ammo, MaxAmmo;

        protected BulletClassification BulletType;

        protected PackedScene bulletScene;

        protected Node2D root;

        public Weapon(Player player)
        {
            root = player.GetTree().Root.GetNode("Game") as Node2D;
            this.bulletScene = (PackedScene)GD.Load("res://scenes/Bullet.tscn");
        }

        public void addAmmo(int ammo)
        {
            this.Ammo += ammo;
        }

        public abstract void Reload();

        public void AddAmmo(int ammo)
        {
            this.Ammo += ammo;
        }

        public abstract void Shoot(Vector2 weaponPosition);

    }
}