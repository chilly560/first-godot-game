namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game;
    using Game.Drops;
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
        [RequireGodotRuntime]
        public void CreateWeapon_UnrecognizedWeaponType_ThrowsArgumentException()
        {
            bool exceptionThrown = false;
            try
            {
                WeaponFactory.CreateWeapon((WeaponType)99, CreateAndAttachGameRoot());
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

        // Weapon's base constructor resolves "GameRoot" via parent.GetTree().Root.GetNode("GameRoot"),
        // so the parent must actually be in the live scene tree. Using the parent itself as that
        // "GameRoot" node avoids needing a second node or a full game scene.
        private static GameRoot CreateAndAttachGameRoot()
        {
            SceneTree tree = (SceneTree)Engine.GetMainLoop();
            GameRoot gameRoot = new GameRoot();
            gameRoot.Name = "GameRoot";
            tree.Root.AddChild(gameRoot);
            return gameRoot;
        }

        // A concrete, non-mock ICollector for the weapon to actually belong to. It's parented
        // under the GameRoot so it shares the same live scene tree (and thus the same
        // GetTree().Root.GetNode("GameRoot") resolution) without being the GameRoot itself.
        private static Drop CreateAndAttachCollector(GameRoot gameRoot)
        {
            PackedScene scene = GD.Load<PackedScene>("res://scenes/ShotgunDrop.tscn");
            Drop collector = scene.Instantiate() as ShotgunDrop;
            gameRoot.AddChild(collector);
            return collector;
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateWeapon_Pistol_WithValidParent_CreatesWeaponWithInfiniteAmmo()
        {
            GameRoot gameRoot = null;
            try
            {
                gameRoot = CreateAndAttachGameRoot();
                Drop collector = CreateAndAttachCollector(gameRoot);

                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, collector);

                AssertThat(weapon).IsNotNull();
                AssertThat(weapon).IsInstanceOf<Weapon>();
                AssertThat(weapon.GetAmmo()).IsEqual(-1);
                AssertThat(weapon.GetMaxAmmo()).IsEqual(-1);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameRoot?.GetParent()?.RemoveChild(gameRoot);
                gameRoot?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateWeapon_Shotgun_WithValidParent_CreatesWeaponWithFiniteAmmo()
        {
            GameRoot gameRoot = null;
            try
            {
                gameRoot = CreateAndAttachGameRoot();
                Drop collector = CreateAndAttachCollector(gameRoot);

                IWeapon weapon = WeaponFactory.CreateWeapon(WeaponType.Shotgun, collector);

                AssertThat(weapon).IsNotNull();
                AssertThat(weapon).IsInstanceOf<Weapon>();
                AssertThat(weapon.GetMaxAmmo()).IsEqual(25);
                AssertThat(weapon.GetAmmo()).IsGreater(0);
                AssertThat(weapon.GetAmmo()).IsLessEqual(25);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameRoot?.GetParent()?.RemoveChild(gameRoot);
                gameRoot?.Free();
            }
        }
    }
}
