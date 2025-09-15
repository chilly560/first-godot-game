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
                BulletType = BulletClassification.Standard;
                Damage = 10;
                Speed = 700;
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
                BulletType = BulletClassification.Heavy;
                Damage = 15;
                Speed = 500;
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
                Bullet bullet = (Bullet)bulletScene.Instantiate();
                Bullet bullet2 = (Bullet)bulletScene.Instantiate();
                Bullet bullet3 = (Bullet)bulletScene.Instantiate();
                bullet2.Rotate(DEG_30_IN_RAD);
                bullet3.Rotate(-DEG_30_IN_RAD);
                bullet.GlobalPosition = weaponPosition;
                bullet2.GlobalPosition = weaponPosition;
                bullet3.GlobalPosition = weaponPosition;
                root.AddChild(bullet);
                root.AddChild(bullet2);
                root.AddChild(bullet3);
                bullet.SetTimer(.01f);
                bullet2.SetTimer(.01f);
                bullet3.SetTimer(.01f);      
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
