using System;
using Godot;
namespace Game.Weapons
{
    public abstract class Weapon : IWeapon
    {
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
  
        protected void SetBulletType(BulletClassification type)
        {
            switch (type)
            {
                case BulletClassification.Standard:
                    this.Damage = 20;
                    this.Speed = 750;
                    break;
                case BulletClassification.Heavy:
                    this.Damage = 15;
                    // Special case: Heavy bullets have variable speed
                    this.Speed = -1;
                    break;
                case BulletClassification.FiftyCal:
                    this.Damage = 50;
                    this.Speed = 1000;
                    break;
                case BulletClassification.RayGun:
                    this.Damage = 5;
                    this.Speed = 1500;
                    break;
                case BulletClassification.Explosive:
                    this.Damage = 40;
                    this.Speed = 400;
                    break;
                default:
                    throw new ArgumentException("Invalid bullet classification");
            }
        }

        public abstract void Shoot(Vector2 weaponPosition);

    }
}