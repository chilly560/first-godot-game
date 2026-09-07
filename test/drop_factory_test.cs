namespace GdUnit4.Tests 
{
	using static GdUnit4.Assertions;

	using System;
	using Godot;
	using Game;
	using Game.Drops;
	using Game.Enemies;
	using Game.StatusModifier;
	using Game.Weapons;

	[TestSuite]
	public class DropFactoryTest
	{
		private class MockCollector : ICollector { }

		[TestCase]
		public void TestHelloWorld() {
			AssertThat("Hello World").Equals("Hello World");
		}
		[TestCase]
		public void DropFactoryFactoryConstruct()
		{
			AbstractDropFactory weaponFactory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Weapon);
			AssertThat(weaponFactory).IsNotNull();
			AssertThat(weaponFactory.GetFactoryType()).IsEqual(AbstractDropFactory.WEAPON);

			AbstractDropFactory statusFactory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.StatusModifier);
			AssertThat(statusFactory).IsNotNull();
			AssertThat(statusFactory.GetFactoryType()).IsEqual(AbstractDropFactory.STATUS);

			AbstractDropFactory nullFactory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Null);
			AssertThat(nullFactory).IsNotNull();
		}

		[TestCase]
		public void WeaponDropFactoryConstructs()
		{
			try
			{
				AbstractDropFactory factory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Weapon);
				AssertThat(factory).IsNotNull();
				AssertThat(factory.GetFactoryType()).IsEqual(AbstractDropFactory.WEAPON);
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				AssertThat(false).IsEqual(true);
			}
		}

			
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

		[TestCase]
		public void WeaponDropFactory_Pistol_ThrowsNotImplementedException()
		{
			bool exceptionThrown = false;
			try
			{
				AbstractDropFactory factory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Weapon);
				factory.MakeDrop((int)DropType.Weapon.Pistol);
			}
			catch (NotImplementedException)
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
		[RequireGodotRuntime]
		public void WeaponDropFactory_MakesShotgunDrop()
		{
			try
			{
				AbstractDropFactory factory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Weapon);
				Drop drop = factory.MakeDrop((int)DropType.Weapon.Shotgun);
				AssertThat(drop).IsNotNull();
				AssertThat(drop).IsInstanceOf<ShotgunDrop>();
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				AssertThat(false).IsEqual(true);
			}
		}

		[TestCase]
		[RequireGodotRuntime]
		public void StatusModifierDropFactory_MakesHealthDrop()
		{
			try
			{
				AbstractDropFactory factory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.StatusModifier);
				Drop drop = factory.MakeDrop((int)DropType.StatusModifier.HealthBoost);
				AssertThat(drop).IsNotNull();
				AssertThat(drop).IsInstanceOf<HealthDrop>();
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				AssertThat(false).IsEqual(true);
			}
		}

		[TestCase]
		public void StatusModifierDropFactory_SpeedBoost_ThrowsArgumentException()
		{
			bool exceptionThrown = false;
			try
			{
				AbstractDropFactory factory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.StatusModifier);
				factory.MakeDrop((int)DropType.StatusModifier.SpeedBoost);
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
		[RequireGodotRuntime]
		public void NullDropFactory_MakesNullDrop()
		{
			try
			{
				AbstractDropFactory factory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Null);
				Drop drop = factory.MakeDrop(0);
				AssertThat(drop).IsNull();
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				AssertThat(false).IsEqual(true);
			}
		}
	}

}
