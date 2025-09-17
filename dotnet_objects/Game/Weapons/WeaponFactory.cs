using Godot;
using System;
using System.Timers;
using System.Collections.Generic;

namespace Game.Weapons
{
    // Factory class to create weapons
    public static class WeaponFactory
    {

        private const float DEG_30_IN_RAD = 0.523598776f; 

        private static Player player1 = null; 

        private class Pistol : Weapon
        {
            public Pistol(Player player) : base(player)
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

            public Shotgun(Player player) : base(player)
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
                    newBullet.SetStats(this.Damage, this.Speed, .75f);
                    bullets.Add(newBullet);
                }

                foreach (Bullet b in bullets)
                {
                    b.Rotate(RandomSpread());
                    b.GlobalPosition = weaponPosition;
                    root.AddChild(b);
                }

                foreach (Bullet b in bullets)
                    b.ShootBullet();

            }
        }
        public static IWeapon CreateWeapon(WeaponType weaponType)
        {
            if (player1 == null)
                throw new ArgumentNullException("Player reference is null. Cannot create weapon without player context.");

            switch (weaponType)
            {
                case WeaponType.Pistol:
                    return new Pistol(player1);
                case WeaponType.Shotgun:
                    return new Shotgun(player1);
                default:
                    throw new ArgumentException($"Weapon type '{weaponType}' is not recognized.");
            }
        }

        public static IWeapon CreateWeapon(WeaponType weaponType, Player player)
        {
            player1 = player;
            if (player == null)
                throw new ArgumentNullException("ERROR: PLAYER OBJ NULL");    
            return CreateWeapon(weaponType);
        }

        public static void SetPlayer(Player player)
        {
            player1 = player;
        }
    }
}
