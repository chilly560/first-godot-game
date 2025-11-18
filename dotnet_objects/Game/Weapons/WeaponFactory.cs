using Godot;
using System;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using Game.Enemies;
using Game.Drops;
using System.ComponentModel;

namespace Game.Weapons
{
    // Factory class to create weapons
    public static class WeaponFactory
    {
        private class CollectorCastException : Exception
        {
            public CollectorCastException(string message) : base(message) { }
        }

        private const float DEG_30_IN_RAD = 0.523598776f; 

        private const int MAX_SHOTGUN_AMMO = 25;

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
                Bullet bullet = (Bullet)bulletScene.Instantiate();
                bullet.GlobalPosition = weaponPosition;
                // TODO: Overhaul the Timer system used in Bullet.ShootBullet
                // to be universal to all bullet types. 
                root.AddChild(bullet);
                SetBulletPhysics(bullet, parent is Player);
            }
        }

        private class Shotgun : Weapon
        {
            private bool randomBulletSpread = false;

            public Shotgun(Node2D parent) : base(parent)
            {
                this.SetBulletType(BulletClassification.Heavy);
                Ammo = -1; // Infinite ammo
                MaxAmmo = -1; // Infinite ammo
            }

            public Shotgun(Node2D parent, int Ammo, int MaxAmmo) : base (parent)
            {
                SetBulletType(BulletClassification.Heavy);
                this.Ammo = Ammo;
                this.MaxAmmo = MaxAmmo;
            }

            public void EnableRandomSpread()
            {
                randomBulletSpread = true;
            }

            public void DisableRandomSpread()
            {
                randomBulletSpread = false;
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
                    GD.Print("Ammo:" + Ammo);
                    if (Ammo < 1)
                        return;

                    Ammo--;

                    for (int i = 0; i < 5; i++)
                    {
                        Bullet newBullet = (Bullet)bulletScene.Instantiate();
                        newBullet.SetStats(this.Damage, 400);
                        bullets.Add(newBullet);
                    }

                    bullets[0].Rotate(DEG_30_IN_RAD * -.5f);
                    bullets[0].GlobalPosition = weaponPosition;
                    root.AddChild(bullets[0]);

                    bullets[1].Rotate(DEG_30_IN_RAD * -.25f);
                    bullets[1].GlobalPosition = weaponPosition;
                    root.AddChild(bullets[1]);

                    bullets[2].GlobalPosition = weaponPosition;
                    root.AddChild(bullets[2]);

                    bullets[3].Rotate(DEG_30_IN_RAD * .25f);
                    bullets[3].GlobalPosition = weaponPosition;
                    root.AddChild(bullets[3]);

                    bullets[4].Rotate(DEG_30_IN_RAD * .5f);
                    bullets[4].GlobalPosition = weaponPosition;
                    root.AddChild(bullets[4]);

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

        public static IWeapon CreateWeapon(WeaponType weaponType, ICollector parent)
        // Gross VVV 
        // TODO: Refactor
        {
            if (parent is null)
                throw new ArgumentException("ERROR: PARENT NODE IS NULL.");

            switch (weaponType)
            {
                case WeaponType.Pistol:
                    try
                    {
                        Player concreteParent = (Player)parent;
                        return new Pistol(concreteParent);
                    } catch (InvalidCastException) {
                        try
                        {
                            Enemy concreteParent = (Enemy)parent;
                            return new Pistol(concreteParent);   
                        } catch (InvalidCastException)
                        {
                            try
                            {
                                Drop concreteParent = (Drop)parent;
                                return new Pistol(concreteParent);
                            } catch (InvalidCastException)
                            {
                                try
                                {
                                    GameRoot concreteParent = (GameRoot)parent;
                                    return new Pistol(concreteParent);
                                }
                                catch (InvalidCastException)
                                {
                                    throw new CollectorCastException("ERROR: PARENT NODE NOT OF TYPE PLAYER OR ENEMY OR DROP.");
                                }                            
                            }
                        }
                    } 
                case WeaponType.Shotgun:
                    try
                    {
                        Player concreteParent = (Player)parent;
                        return new Shotgun(concreteParent, (new Random()).Next(1, MAX_SHOTGUN_AMMO + 1), MAX_SHOTGUN_AMMO);
                    } catch (InvalidCastException) {
                        try
                        {
                            Enemy concreteParent = (Enemy)parent;
                            return new Shotgun(concreteParent);   
                        } catch (InvalidCastException)
                        {
                            try
                            {
                                Drop concreteParent = (Drop)parent;
                                return new Shotgun(concreteParent, (new Random()).Next(1, MAX_SHOTGUN_AMMO + 1), MAX_SHOTGUN_AMMO);
                            } catch (InvalidCastException)
                            {
                                try
                                {
                                    GameRoot concreteParent = (GameRoot)parent;
                                    return new Shotgun(concreteParent, (new Random()).Next(1, MAX_SHOTGUN_AMMO + 1), MAX_SHOTGUN_AMMO);
                                }
                                catch (InvalidCastException)
                                {
                                    throw new CollectorCastException("ERROR: PARENT NODE NOT OF TYPE PLAYER OR ENEMY OR DROP.");
                                }                               
                            }
                        }
                    } 
                    // dup logic here
                default:
                    throw new ArgumentException($"Weapon type '{weaponType}' is not recognized.");
            }
        }
    }
}
