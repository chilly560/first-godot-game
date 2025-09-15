using Godot;
using System;
using Game.Weapons;

namespace Game.Weapons
{
    // Factory class to create weapons
    public static class WeaponFactory
    {

        private static Player player1 = null; // Temporary, will be passed in properly later

        private class Pistol : Weapon
        {
            public Pistol(Player player) : base(player)
            {
                BulletType = Bullet.Standard;
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
        public static IWeapon CreateWeapon(WeaponType weaponType)
        {
            if (player1 == null)
                throw new ArgumentNullException("Player reference is null. Cannot create weapon without player context.");

            switch (weaponType)
            {
                case WeaponType.Pistol:
                    return new Pistol(player1);
                default:
                    throw new ArgumentException($"Weapon type '{weaponType}' is not recognized.");
            }
        }

        public static IWeapon CreateWeapon(WeaponType weaponType, Player player)
        {
            player1 = player;
            if (player == null)
                throw new ArgumentNullException("ERROR: PLAYER OBJ NULL");    //?? throw new ArgumentNullException("Player reference is null. Cannot create weapon without player context.");
            return CreateWeapon(weaponType);
        }

        public static void SetPlayer(Player player)
        {
            player1 = player;
        }
    }
}
