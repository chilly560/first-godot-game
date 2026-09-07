namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game.Weapons;

    public class WeaponTest
    {
        private const int MAX_SHOTGUN_AMMO = 25;

        [TestCase]
        [RequireGodotRuntime]
        public void Pistol_GetAmmo_ReturnsInfinite()
        {
            try
            {
                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, null);
                AssertThat(weapon.GetAmmo()).IsEqual(-1);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }

        [TestCase]
        public void Pistol_GetMaxAmmo_ReturnsInfinite()
        {
            try
            {
                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, null);
                AssertThat(weapon.GetMaxAmmo()).IsEqual(-1);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }

        [TestCase]
        public void Shotgun_GetAmmo_ReturnsValueWithinValidRange()
        {
            try
            {
                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Shotgun, null);
                AssertThat(weapon.GetAmmo()).IsGreater(0);
                AssertThat(weapon.GetAmmo()).IsLessEqual(MAX_SHOTGUN_AMMO);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }

        [TestCase]
        public void Shotgun_GetMaxAmmo_ReturnsMaxShotgunAmmo()
        {
            try
            {
                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Shotgun, null);
                AssertThat(weapon.GetMaxAmmo()).IsEqual(MAX_SHOTGUN_AMMO);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }

        [TestCase]
        public void Weapon_AddAmmo_IncreasesAmmoByExpectedAmount()
        {
            try
            {
                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Shotgun, null);
                int before = weapon.GetAmmo();
                weapon.AddAmmo(5);
                AssertThat(weapon.GetAmmo()).IsEqual(before + 5);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Weapon_SetAmmoMax_RestoresAmmoToMax()
        {
            try
            {
                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Shotgun/*, null*/, null);
                int max = weapon.GetMaxAmmo();
                weapon.AddAmmo(-weapon.GetAmmo());
                weapon.SetAmmoMax();
                AssertThat(weapon.GetAmmo()).IsEqual(max);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }
    }
}
