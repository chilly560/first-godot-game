namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game;
    using Game.Weapons;

    [TestSuite]
    public class WeaponFactoryTest
    {
        private class MockCollector : ICollector { }

        [TestCase]
        public void CreateWeapon_NullParent_ThrowsArgumentException()
        {
            bool exceptionThrown = false;
            try
            {
                WeaponFactory.CreateWeapon(WeaponType.Pistol, null);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        public void CreateWeapon_UnrecognizedCollector_Pistol_ThrowsException()
        {
            bool exceptionThrown = false;
            try
            {
                WeaponFactory.CreateWeapon(WeaponType.Pistol, new MockCollector());
            }
            catch (Exception e) when (e is not ArgumentException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        public void CreateWeapon_UnrecognizedCollector_Shotgun_ThrowsException()
        {
            bool exceptionThrown = false;
            try
            {
                WeaponFactory.CreateWeapon(WeaponType.Shotgun, new MockCollector());
            }
            catch (Exception e) when (e is not ArgumentException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        public void CreateWeapon_UnrecognizedWeaponType_ThrowsArgumentException()
        {
            bool exceptionThrown = false;
            try
            {
                WeaponFactory.CreateWeapon((WeaponType)99, new MockCollector());
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }
    }
}
