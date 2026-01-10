using Godot;
using System;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using Game.Enemies;
using Game.Drops;
using System.ComponentModel;
using NewGameProject.scripts;

namespace Game.Weapons
{
    /// <summary>
    /// Factory class for creating weapons.
    /// </summary>
    public static class WeaponFactory
    {
        /// <summary>
        /// Custom exception for when the parent node is not of a valid type.
        /// </summary>
        private class CollectorCastException : Exception
        {
            public CollectorCastException(string message) : base(message) { }
        }
        /// <summary>
        /// 30 Degrees in Radians.
        /// </summary>
        private const float DEG_30_IN_RAD = 0.523598776f; 
        /// <summary>
        /// Maximum amount of ammo for a shotgun.
        /// </summary>
        private const int MAX_SHOTGUN_AMMO = 25;
        /// <summary>
        /// Pistol class that extends the Weapon class.
        /// </summary>
        private class Pistol : Weapon
        {
            /// <summary>
            /// Creates a pistol with infinite ammo.
            /// </summary>
            /// <param name="parent"></param>
            public Pistol(Node2D parent) : base(parent)
            {
                SetBulletType(BulletClassification.Standard);
                Ammo = -1; // Infinite ammo
                MaxAmmo = -1; // Infinite ammo
            }
            /// <summary>
            /// Does nothing because the pistol does not need to reload. For potential cases where a pistol or child class needs to reload.
            /// </summary>
            public override void Reload()
            {
                // Pistol does not need to reload
            }
            /// <summary>
            /// Shoots a standard bullet from the weapon's position.
            /// </summary>
            /// <param name="weaponPosition"></param>
            public override void Shoot(Vector2 weaponPosition)
            {
                if (parent is Player)
                {
                    Bullet bullet = (Bullet)bulletScene.Instantiate();
                    bullet.GlobalPosition = weaponPosition;
                    bullet.SetStats(Damage, Speed);
                    root.AddChild(bullet);
                    SetBulletPhysics(bullet, parent is Player);   
                } else
                {
                    EnemyBullet bullet = (EnemyBullet)enemyBulletScene.Instantiate();
                    bullet.GlobalPosition = weaponPosition;
                    bullet.SetStats(Damage, Speed);
                    root.AddChild(bullet);
                    SetBulletPhysics(bullet, parent is Player);   
                }
                
            }
        }
        /// <summary>
        /// Shotgun class that extends the Weapon class.
        /// </summary>
        private class Shotgun : Weapon
        {   
            /// <summary>
            /// Enables or disables random spread mode for the shotgun.
            /// </summary>
            private bool randomBulletSpread = false;
            /// <summary>
            /// Creates a shotgun with infinite ammo.
            /// </summary>
            /// <param name="parent"></param>
            public Shotgun(Node2D parent) : base(parent)
            {
                this.SetBulletType(BulletClassification.Heavy);
                Ammo = -1; // Infinite ammo
                MaxAmmo = -1; // Infinite ammo
            }
            /// <summary>
            /// Creates a shotgun with a finite amount of ammo.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="Ammo"></param>
            /// <param name="MaxAmmo"></param>
            public Shotgun(Node2D parent, int Ammo, int MaxAmmo) : base (parent)
            {
                SetBulletType(BulletClassification.Heavy);
                this.Ammo = Ammo;
                this.MaxAmmo = MaxAmmo;
            }
            /// <summary>
            /// Enables the random spread mode for the shotgun.
            /// </summary>
            public void EnableRandomSpread()
            {
                randomBulletSpread = true;
            }
            /// <summary>
            /// Disables the random spread mode for the shotgun.
            /// </summary>
            public void DisableRandomSpread()
            {
                randomBulletSpread = false;
            }
            /// <summary>
            /// Reloads the weapon if it has a finite amount of ammo.
            /// </summary>
            public override void Reload()
            {
                // Pistol does not need to reload
            }
            /// <summary>
            /// Randomly generates a spread angle between -15 and 15 degrees.
            /// </summary>
            /// <returns></returns>
            private float RandomSpread()
            {
                Random rand = new Random();
                float baseFloat = (float)rand.NextDouble() * DEG_30_IN_RAD;
                return rand.NextDouble() < 0.5 ? -baseFloat : baseFloat;
            }
            /// <summary>
            /// Randomly generates a number of bullets to shoot between 5 and 8.
            /// </summary>
            /// <returns></returns>
            private int RandomBulletCount()
            {
                Random rand = new Random();
                return rand.Next(5, 9);
            }
            /// <summary>
            /// Shoots a shotgun bullet with a spread pattern. If randomBulletSpread is enabled, the bullet count and spread angle are randomized.
            /// </summary>
            /// <param name="weaponPosition">Position to shoot from</param>
            /// <exception cref="Exception"></exception>
            public override void Shoot(Vector2 weaponPosition)
            {
                List<Bullet> bullets = new List<Bullet>();
                if (randomBulletSpread)
                {
                    if (Ammo != -1)
                        throw new Exception("ERROR: RANDOM MODE MUST HAVE INFINITE AMMO");

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

                    bool isPlayer = parent is Player;

                    foreach (Bullet b in bullets)
                    {
                        this.SetBulletPhysics(b, isPlayer);
                        b.SetPhysicsModifier(BulletMod.DefaultShotgunMod);
                        b.ShootBullet();
                    }
                } else
                {
                    if (Ammo < 1)
                        return;

                    Ammo--;

                    for (int i = 0; i < 5; i++)
                    {
                        Bullet newBullet = (Bullet)bulletScene.Instantiate();
                        newBullet.SetStats(this.Damage, 400);
                        bullets.Add(newBullet);
                    }

                    float[] spreadAngles = { -.5f, -.25f, 0f, .25f, .5f };
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].Rotate(DEG_30_IN_RAD * spreadAngles[i]);
                        bullets[i].GlobalPosition = weaponPosition;
                        root.AddChild(bullets[i]);
                    }

                    bool isPlayer = parent is Player;

                    foreach (Bullet b in bullets)
                    {
                        SetBulletPhysics(b, isPlayer);
                        b.SetPhysicsModifier(BulletMod.DefaultShotgunMod);
                        b.ShootBullet();
                    }
                }
            }
        }
        /// <summary>
        /// Creates a weapon of the specified type and attaches it to the pare bnt node.
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="parent">Node to spawn the weapon in</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CollectorCastException"></exception>
        public static IWeapon CreateWeapon(WeaponType weaponType, ICollector parent)
        {
            if (parent is null)
                throw new ArgumentException("ERROR: PARENT NODE IS NULL.");

            return weaponType switch
            {
                WeaponType.Pistol => parent switch
                {
                    Player p => new Pistol(p),
                    Enemy e => new Pistol(e),
                    Drop d => new Pistol(d),
                    GameRoot g => new Pistol(g),
                    _ => throw new CollectorCastException("ERROR: PARENT NODE NOT OF TYPE PLAYER OR ENEMY OR DROP.")
                },
                WeaponType.Shotgun => parent switch
                {
                    Player p => new Shotgun(p, (new Random()).Next(1, MAX_SHOTGUN_AMMO + 1), MAX_SHOTGUN_AMMO),
                    Enemy e => new Shotgun(e),
                    Drop d => new Shotgun(d, (new Random()).Next(1, MAX_SHOTGUN_AMMO + 1), MAX_SHOTGUN_AMMO),
                    GameRoot g => new Shotgun(g, (new Random()).Next(1, MAX_SHOTGUN_AMMO + 1), MAX_SHOTGUN_AMMO),
                    _ => throw new CollectorCastException("ERROR: PARENT NODE NOT OF TYPE PLAYER OR ENEMY OR DROP.")
                },
                _ => throw new ArgumentException($"Weapon type '{weaponType}' is not recognized.")
            };
        }
    }
}
