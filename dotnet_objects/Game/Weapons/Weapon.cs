using System;
using System.Runtime.CompilerServices;
using Game.Enemies;
using Godot;
using Game;
namespace Game.Weapons
{
    /// <summary>
    /// Base class for all weapons in the game.
    /// </summary>
    public abstract class Weapon : IWeapon
    {
        /// <summary>
        /// For testing purposes
        /// </summary>
        private const int HAXXOR_DAMAGE = 999;
        /// <summary>
        /// Standard damage for a bullet
        /// </summary>
        private const int STANDARD_DAMAGE = 20;
        /// <summary>
        /// Standard speed for a bullet
        /// </summary>
        private const int STANDARD_SPEED = 750;
        /// <summary>
        /// Damage for a heavy bullet used in Shotgun
        /// </summary>
        private const int HEAVY_DAMAGE = 15;
        /// <summary>
        /// Speed is not defined for heavy bullets as they can have variable speed.
        /// 
        /// Speed is typically defined in the BulletPhysicsOverhauler selected for the bullet,
        /// or on the BulletPhysicsModifier.
        /// </summary>
        private const int HEAVY_SPEED = -1;

        private const int FIFTY_CAL_DAMAGE = 50;

        private const int FIFTY_CAL_SPEED = 1000;

        private const int RAY_GUN_DAMAGE = 5;

        private const int RAY_GUN_SPEED = 1500;

        private const int EXPLOSIVE_DAMAGE = 40;

        private const int EXPLOSIVE_SPEED = 400;

        protected int Damage, Speed, Ammo, MaxAmmo;
        /// <summary>
        /// Type of Bullet used when selecting Damage, Speed, Ammo, and MaxAmmo
        /// </summary>
        protected BulletClassification BulletType;
        /// <summary>
        /// The Godot Scene for a Bullet
        /// </summary>
        protected PackedScene bulletScene;
        /// <summary>
        /// The root node of the game
        /// </summary>
        protected Node2D root;
        /// <summary>
        /// Secondary physics behavior that can be added to a bullet at runtime
        /// </summary>
        protected BulletPhysicsModifiers BulletMod;
        /// <summary>
        /// Core physics behavior of a bullet that can be changed at runtime
        /// </summary>
        protected BulletPhysicsOverhaulers BulletOverhaul;
        /// <summary>
        /// The scene the bullet is instantiated in
        /// </summary>
        protected Node2D parent;
        /// <summary>
        /// Constructor for the Weapon class
        /// </summary>
        /// <param name="parent">Node/Scene to instantiate the bullet in</param>
        public Weapon(Node2D parent)
        {
            BulletMod = new BulletPhysicsModifiers();
            BulletOverhaul = new BulletPhysicsOverhaulers();
            SetParent(parent);
            root = parent.GetTree().Root.GetNode("GameRoot") as Node2D;
            this.bulletScene = (PackedScene)GD.Load("res://scenes/bullet.tscn");
        }
        /// <summary>
        /// Removes ammo from the weapon's ammo count if the weapon does not have infinite ammo
        /// 
        /// May remove due to 'AddAmmo'
        /// </summary>
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
                b.speed = b.speed * 1.05f;
            }
        }

        /// <summary>
        ///  This class is for completely overhauling the physics of a bullet at runtime
        /// </summary>
        protected class BulletPhysicsOverhaulers
        {
            /// <summary>
            /// A slower bullet for bogey enemy types
            /// </summary>
            private const float BOGEY_BULLET_SPEED_MOD = 0.8f;
            /// <summary>
            /// Default physics behavior for a bullet
            /// </summary>
            /// <param name="b">Bullet to set the phyics of (usually 'this' when calling from the Bullet class itself)</param>
            /// <param name="delta">Time between frames in seconds</param>
            public void DefaultPhysics(Bullet b, double delta)
            {
                b.Position += -1 * b.Transform.Y * b.speed * (float)delta;
            }
            /// <summary>
            /// Default physics behavior for a bullet when it is fired by an enemy
            /// </summary>
            /// <param name="b">Bullet to set the phyics of (usually 'this' when calling from the Bullet class itself)</param>
            /// <param name="delta">Time between frames in seconds</param>
            public void EnemyDefaultPhysics(Bullet b, double delta)
            {
                b.Position += 1 * b.Transform.Y * b.speed * (float)delta;
            }
            /// <summary>
            /// Default physics behavior for a bullet when it is fired by a bogey enemy type
            /// </summary>
            /// <param name="b">Bullet to set the phyics of (usually 'this' when calling from the Bullet class itself)</param>
            /// <param name="delta">Time between frames in seconds</param>
            public void BogeyDefaultPhysics(Bullet b, double delta)
            {
                b.Position += 1 * b.Transform.Y * (b.speed * BOGEY_BULLET_SPEED_MOD) * (float)delta;
            }
        }
        /// <summary>  
        /// Sets the type of bullet used by the weapon
        /// </summary>
        /// <param name="type">A BulletClassification determining the type of Bullet this Weapon should use</param>
        /// <exception cref="ArgumentException">If the BulletClassification is not valid</exception>
        protected void SetBulletType(BulletClassification type)
        {
            switch (type)
            {
                case BulletClassification.Standard:
                    Damage = STANDARD_DAMAGE;
                    //this.Damage = HAXXOR_DAMAGE;
                    Speed = STANDARD_SPEED;
                    break;
                case BulletClassification.Heavy:
                    Damage = HEAVY_DAMAGE;
                    Speed = HEAVY_SPEED;
                    break;
                case BulletClassification.FiftyCal:
                    Damage = FIFTY_CAL_DAMAGE;
                    Speed = FIFTY_CAL_SPEED;
                    break;
                case BulletClassification.RayGun:
                    Damage = RAY_GUN_DAMAGE;
                    Speed = RAY_GUN_SPEED;
                    break;
                case BulletClassification.Explosive:
                    Damage = EXPLOSIVE_DAMAGE;
                    Speed = EXPLOSIVE_SPEED;
                    break;
                default:
                    throw new ArgumentException("Invalid bullet classification");
            }
        }
        /// <summary>
        /// Sets the physics behavior of a bullet based on the type of parent (Player or Enemy)
        /// </summary>
        /// <param name="b">Bullet to set phyics of</param>
        /// <param name="isPlayer">Whether the 'Collector' of the weapon is an Enemy or a Player</param>
        /// <exception cref="ArgumentException"></exception>
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

        public int GetAmmo()
        {
            return Ammo;
        }

        public int GetMaxAmmo()
        {
            return MaxAmmo;
        }

        public void SetAmmoMax()
        {
            Ammo = MaxAmmo;
        }

        public abstract void Shoot(Vector2 weaponPosition);
    }
}