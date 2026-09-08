namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game.Weapons;

    [TestSuite]
    public class WeaponSceneTest
    {
        private class FakeWeapon : IWeapon
        {
            public int Ammo;
            public int MaxAmmo;
            public bool AmmoSetToMax;
            public bool ShootCalled;
            public Vector2 LastShootPosition;

            public FakeWeapon(int ammo, int maxAmmo)
            {
                Ammo = ammo;
                MaxAmmo = maxAmmo;
            }

            public void AddAmmo(int ammo) => Ammo += ammo;
            public int GetAmmo() => Ammo;
            public int GetMaxAmmo() => MaxAmmo;
            public void SetAmmoMax()
            {
                AmmoSetToMax = true;
                Ammo = MaxAmmo;
            }
            public void Reload() { }
            public void Shoot(Vector2 weaponPosition)
            {
                ShootCalled = true;
                LastShootPosition = weaponPosition;
            }
            public void SetParent(Node2D parent) { }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Shoot_InvokesCurrentWeaponShootAtGlobalPosition()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                FakeWeapon weapon = new FakeWeapon(10, 25);
                weaponScene.SetWeapon(weapon);
                weaponScene.Position = new Vector2(5, 5);

                weaponScene.Shoot();

                AssertThat(weapon.ShootCalled).IsEqual(true);
                AssertThat(weapon.LastShootPosition).IsEqual(new Vector2(5, 5));
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AltShoot_WithNoSecondaryWeapon_DoesNotThrow()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                weaponScene.AltShoot();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AltShoot_WithSecondaryWeapon_InvokesShootAtGlobalPosition()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                FakeWeapon secondary = new FakeWeapon(5, 25);
                weaponScene.AddNewWeapon(secondary);
                weaponScene.Position = new Vector2(10, 20);

                weaponScene.AltShoot();

                AssertThat(secondary.ShootCalled).IsEqual(true);
                AssertThat(secondary.LastShootPosition).IsEqual(new Vector2(10, 20));
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AddNewWeapon_WhenNoSecondaryWeapon_DoesNotThrow()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                weaponScene.AddNewWeapon(new FakeWeapon(5, 25));
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AddNewWeapon_CombinedAmmoUnderMax_AddsAmmoToSecondary()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                FakeWeapon first = new FakeWeapon(5, 25);
                weaponScene.AddNewWeapon(first);

                FakeWeapon second = new FakeWeapon(3, 999);
                weaponScene.AddNewWeapon(second);

                AssertThat(first.Ammo).IsEqual(8);
                AssertThat(first.AmmoSetToMax).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AddNewWeapon_CombinedAmmoExceedsMax_SetsSecondaryAmmoToMax()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                FakeWeapon first = new FakeWeapon(20, 25);
                weaponScene.AddNewWeapon(first);

                FakeWeapon second = new FakeWeapon(10, 999);
                weaponScene.AddNewWeapon(second);

                AssertThat(first.AmmoSetToMax).IsEqual(true);
                AssertThat(first.Ammo).IsEqual(25);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void GetWeapon_WhenNoWeaponSet_ReturnsNull()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                AssertThat(weaponScene.GetWeapon()).IsNull();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void GetSecondaryWeapon_WhenNoSecondaryWeaponSet_ReturnsNull()
        {
            WeaponScene weaponScene = null;
            try
            {
                weaponScene = new WeaponScene();
                AssertThat(weaponScene.GetSecondaryWeapon()).IsNull();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                weaponScene?.Free();
            }
        }
    }
}
