namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game;
    using Game.Drops;
    using Game.StatusModifier;

    [TestSuite]
    public class DropTest
    {
        private class MockCollectable : ICollectable
        {
            public void SetParent(Node2D parent) { }
        }

        private static Drop LoadHealthDrop()
        {
            PackedScene scene = GD.Load<PackedScene>("res://scenes/HealthDrop.tscn");
            return scene.Instantiate() as HealthDrop;
        }

        private static Drop LoadShotgunDrop()
        {
            PackedScene scene = GD.Load<PackedScene>("res://scenes/ShotgunDrop.tscn");
            return scene.Instantiate() as ShotgunDrop;
        }

        [TestCase]
        [RequireGodotRuntime]
        public void HealthDrop_IsInstanceOfDrop()
        {
            Drop drop = null;
            try
            {
                drop = LoadHealthDrop();
                AssertThat(drop).IsNotNull();
                AssertThat(drop).IsInstanceOf<IDrop>();
                AssertThat(drop).IsInstanceOf<ICollector>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                drop?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void WeaponDrop_IsInstanceOfDrop()
        {
            Drop drop = null;
            try
            {
                drop = LoadShotgunDrop();
                AssertThat(drop).IsNotNull();
                AssertThat(drop).IsInstanceOf<IDrop>();
                AssertThat(drop).IsInstanceOf<ICollector>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                drop?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Process_WithoutPhysicsOverhauler_ThrowsNullReferenceException()
        {
            bool exceptionThrown = false;
            Drop drop = null;
            try
            {
                drop = LoadHealthDrop();
                drop._Process(0.1);
            }
            catch (NullReferenceException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            finally
            {
                drop?.QueueFree();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Process_WithPhysicsOverhauler_InvokesOverhauler()
        {
            Drop drop = null;
            try
            {
                drop = LoadShotgunDrop();
                bool overhaulerInvoked = false;
                drop.SetPhysicsOverhauler((d, delta) => overhaulerInvoked = true);
                drop._Process(0.1);
                AssertThat(overhaulerInvoked).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                drop?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Process_WithPhysicsModifier_InvokesModifier()
        {
            Drop drop = null;
            try
            {
                drop = LoadHealthDrop();
                bool modifierInvoked = false;
                drop.SetPhysicsOverhauler((d, delta) => { });
                drop.SetPhysicsModifier(d => modifierInvoked = true);
                drop._Process(0.1);
                AssertThat(modifierInvoked).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                drop?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AddAttribute_NullAttribute_ThrowsArgumentException()
        {
            bool exceptionThrown = false;
            Drop drop = null;
            Player player = null;
            try
            {
                drop = LoadShotgunDrop();
                player = new Player();
                drop.AddAttribute(player, null);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            finally
            {
                drop?.QueueFree();
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AddAttribute_UnrecognizedAttributeType_ThrowsArgumentException()
        {
            bool exceptionThrown = false;
            Drop drop = null;
            Player player = null;
            try
            {
                drop = LoadHealthDrop();
                player = new Player();
                drop.AddAttribute(player, new MockCollectable());
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            finally
            {
                drop?.QueueFree();
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AddAttribute_HealthModifierAttribute_DoesNotThrow()
        {
            Drop drop = null;
            Player player = null;
            try
            {
                drop = LoadHealthDrop();
                HealthModifier healthModifier = new HealthModifier(10);
                drop.SetCollectable(healthModifier);
                player = new Player();
                drop.AddAttribute(player, healthModifier);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                drop?.QueueFree();
                player?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnBodyEnteredDrop_WithOneWayBlocker_DoesNotThrow()
        {
            Drop drop = null;
            OneWayBlocker blocker = null;
            try
            {
                drop = LoadHealthDrop();
                blocker = new OneWayBlocker();
                drop.OnBodyEnteredDrop(blocker);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                // OnBodyEnteredDrop does not queue_free the drop for OneWayBlocker bodies.
                drop?.Free();
                blocker?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnBodyEnteredDrop_WithUnrelatedNode_DoesNotThrow()
        {
            Drop drop = null;
            Node2D node = null;
            try
            {
                drop = LoadShotgunDrop();
                node = new Node2D();
                drop.OnBodyEnteredDrop(node);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                // OnBodyEnteredDrop already deferred-frees the drop for non-blocker bodies.
                node?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnBodyEnteredDrop_WithPlayer_ValidCollectable_DoesNotThrow()
        {
            Drop drop = null;
            Player player = null;
            try
            {
                drop = LoadHealthDrop();
                drop.SetCollectable(new HealthModifier(5));
                player = new Player();
                drop.OnBodyEnteredDrop(player);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                // OnBodyEnteredDrop already deferred-frees the drop for non-blocker bodies.
                player?.Free();
            }
        }
    }
}
