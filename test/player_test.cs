namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game;
    using Game.StatusModifier;

    [TestSuite]
    public class PlayerTest
    {
        private class FakeCollectable : ICollectable
        {
            public void SetParent(Node2D parent) { }
        }

        // Player.isAlive defaults to false until _Ready() runs, so Collect() should
        // safely no-op regardless of what is passed in.
        [TestCase]
        [RequireGodotRuntime]
        public void Collect_WhenNotAlive_WithNullCollectable_DoesNotThrow()
        {
            Player player = null;
            try
            {
                player = new Player();
                player.Collect(null);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                player?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Collect_WhenNotAlive_WithHealthModifierCollectable_DoesNotThrow()
        {
            Player player = null;
            try
            {
                player = new Player();
                player.Collect(new HealthModifier(10));
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                player?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Heal_WithZeroAmount_DoesNotThrow()
        {
            Player player = null;
            try
            {
                player = new Player();
                player.Heal(0);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                player?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Heal_WithNegativeAmount_DoesNotThrow()
        {
            Player player = null;
            try
            {
                player = new Player();
                player.Heal(-5);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                player?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Input_TouchPressed_DoesNotThrow()
        {
            Player player = null;
            try
            {
                player = new Player();
                InputEventScreenTouch touch = new InputEventScreenTouch
                {
                    Index = 0,
                    Position = new Vector2(10, 10),
                    Pressed = true
                };
                player._Input(touch);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                player?.Free();
            }
        }
    }
}
