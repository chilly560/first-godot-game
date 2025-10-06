using System;
using System.Runtime.CompilerServices;
using Game.Enemies;
using Godot;
using Game;
namespace Game.Weapons
{
    public abstract class Weapon : IWeapon, ICollectable
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

        protected BulletPhysicsOverhaulers BulletOverhaul;

        protected Node2D parent;

        public Weapon(Node2D parent)
        {
            BulletMod = new BulletPhysicsModifiers();
            BulletOverhaul = new BulletPhysicsOverhaulers();
            SetParent(parent);
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

        // Bullet Physics Modifiers
        /// <summary>
        ///  This class is for adding additional physics modifications to a bullet at runtime
        /// </summary>
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

        // Bullet Physics Overhaulers
        /// <summary>
        ///  This class is for completely overhauling the physics of a bullet at runtime
        /// </summary>
        protected class BulletPhysicsOverhaulers
        {

            private const float BOGEY_BULLET_SPEED_MOD = 0.8f;

            public void DefaultPhysics(Bullet b, double delta)
            {
                b.Position += -1 * b.Transform.Y * b.speed * (float)delta;
            }

            public void EnemyDefaultPhysics(Bullet b, double delta)
            {
                b.Position += 1 * b.Transform.Y * b.speed * (float)delta;
            }

            public void BogeyDefaultPhysics(Bullet b, double delta)
            {
                b.Position += 1 * b.Transform.Y * (b.speed * BOGEY_BULLET_SPEED_MOD) * (float)delta;
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

        protected void SetBulletPhysics(Bullet b, bool isPlayer)
        {
            if (isPlayer)
                b.SetPhysicsOverhauler(BulletOverhaul.DefaultPhysics);
            else if (parent is Bogey)
                b.SetPhysicsOverhauler(BulletOverhaul.BogeyDefaultPhysics);
            else if (parent is Enemy)
                b.SetPhysicsOverhauler(BulletOverhaul.EnemyDefaultPhysics);
            else
                throw new ArgumentException("Invalid parent type for a weapon (should be Player or Enemy)");
        }

        public void SetParent(Node2D parent)
        {
            this.parent = parent;
        }

        public abstract void Shoot(Vector2 weaponPosition);
    }
}