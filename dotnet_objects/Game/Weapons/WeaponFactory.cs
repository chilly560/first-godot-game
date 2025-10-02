using Godot;
using System;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using Game.Enemies;

namespace Game.Weapons
{
    // Factory class to create weapons
    public static class WeaponFactory
    {

        private const float DEG_30_IN_RAD = 0.523598776f; 

        private static Node2D parent = null; 

        private class Pistol : Weapon
        {
            public Pistol(Node2D parent) : base(parent)
            {
                this.SetBulletType(BulletClassification.Standard);
                Ammo = -1; // Infinite ammo
                MaxAmmo = -1; // Infinite ammo
            }

            public override void Reload()
            {
                // Pistol does not need to reload
                GD.Print("[TEMP] Pistol does not need to reload.");
            }

            public override void Shoot(Vector2 weaponPosition)
            {
                Node2D bullet = (Node2D)bulletScene.Instantiate();
                bullet.GlobalPosition = weaponPosition;
                root.AddChild(bullet);
            }
        }

        private class Shotgun : Weapon
        {

            public Shotgun(Node2D parent) : base(parent)
            {
                this.SetBulletType(BulletClassification.Heavy);
                Ammo = -1; // Infinite ammo
                MaxAmmo = -1; // Infinite ammo
            }

            public override void Reload()
            {
                // Pistol does not need to reload
                GD.Print("[TEMP] Pistol does not need to reload.");
            }

            private float RandomSpread()
            {
                Random rand = new Random();
                float baseFloat = (float)rand.NextDouble() * DEG_30_IN_RAD;
                return rand.NextDouble() < 0.5 ? -baseFloat : baseFloat;
            }

            private int RandomBulletCount()
            {
                Random rand = new Random();
                return rand.Next(5, 9); 
            }

            public override void Shoot(Vector2 weaponPosition)
            {
                List<Bullet> bullets = new List<Bullet>();
                int i = RandomBulletCount();

                for (; i > 0; i--) 
                {
                    Bullet newBullet = (Bullet)bulletScene.Instantiate();

                    // For variable speed on heavy bullets
                    Random random = new Random();
                    newBullet.SetStats(this.Damage, random.Next(400, 600), .3f);
                    bullets.Add(newBullet);
                }

                foreach (Bullet b in bullets)
                {
                    b.Rotate(RandomSpread());
                    b.GlobalPosition = weaponPosition;
                    root.AddChild(b);
                }

                foreach (Bullet b in bullets) 
                {
                    b.SetPhysics(this.BulletMod.DefaultShotgunMod);
                    b.ShootBullet();
                }
            }
        }
        public static IWeapon CreateWeapon(WeaponType weaponType)
        {
            if (parent == null)
                throw new ArgumentNullException("Player reference is null. Cannot create weapon without player context.");

            switch (weaponType)
            {
                case WeaponType.Pistol:
                    return new Pistol(parent);
                case WeaponType.Shotgun:
                    return new Shotgun(parent);
                default:
                    throw new ArgumentException($"Weapon type '{weaponType}' is not recognized.");
            }
        }

        public static IWeapon CreateWeapon(WeaponType weaponType, Node2D parent)
        {
            if (parent == null || (parent is not Player && parent is not Enemy))
                throw new ArgumentException("ERROR: PLAYER OBJ NOT OF TYPE PLAYER OR ENEMY");

            WeaponFactory.parent = parent;

            return CreateWeapon(weaponType);
        }

        public static void SetPlayer(Player player)
        {
            parent = player;
        }
    }
}
