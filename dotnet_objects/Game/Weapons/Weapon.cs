using System;
using System.Runtime.CompilerServices;
using Godot;
namespace Game.Weapons
{
    public abstract class Weapon : IWeapon
    {
        private const int STANDARD_DAMAGE = 20;
        private const int STANDARD_SPEED = 750;

        private const int HEAVY_DAMAGE = 15;

        // Special case: Heavy bullets have variable speed
        // Speed is handled via a bullet modifier
        private const int HEAVY_SPEED = -1; 

        private const int FIFTY_CAL_DAMAGE = 50;
        private const int FIFTY_CAL_SPEED = 1000;

        private const int RAY_GUN_DAMAGE = 5;
        private const int RAY_GUN_SPEED = 1500;

        private const int EXPLOSIVE_DAMAGE = 40;
        private const int EXPLOSIVE_SPEED = 400;

        protected int Damage, Speed, Ammo, MaxAmmo;

        protected BulletClassification BulletType;

        protected PackedScene bulletScene;

        protected Node2D root;
        protected BulletPhysicsModifiers BulletMod;
        public Weapon(Node2D parent)
        {
            BulletMod = new BulletPhysicsModifiers();
            root = parent.GetTree().Root.GetNode("Game") as Node2D;
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

        protected class BulletPhysicsModifiers
        {
            // This method provides the speed falloff for shotgun pellets
            /// <summary>
            /// Provides the speed falloff for shotgun pellets.
            /// Applied as a physics modifier to each pellet.
            /// </summary>
            /// <param name="b">The bullet to modify.</param>
            public void DefaultShotgunMod(Bullet b)
            {
                b.speed = b.speed * 0.9525f;
            }
        }

        protected void SetBulletType(BulletClassification type)
        {
            switch (type)
            {
            case BulletClassification.Standard:
                this.Damage = STANDARD_DAMAGE;
                this.Speed = STANDARD_SPEED;
                break;
            case BulletClassification.Heavy:
                this.Damage = HEAVY_DAMAGE;
                this.Speed = HEAVY_SPEED;
                break;
            case BulletClassification.FiftyCal:
                this.Damage = FIFTY_CAL_DAMAGE;
                this.Speed = FIFTY_CAL_SPEED;
                break;
            case BulletClassification.RayGun:
                this.Damage = RAY_GUN_DAMAGE;
                this.Speed = RAY_GUN_SPEED;
                break;
            case BulletClassification.Explosive:
                this.Damage = EXPLOSIVE_DAMAGE;
                this.Speed = EXPLOSIVE_SPEED;
                break;
            default:
                throw new ArgumentException("Invalid bullet classification");
            }
        }

        public abstract void Shoot(Vector2 weaponPosition);
    }
}