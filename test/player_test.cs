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

        // The remaining tests document that these members depend on _Ready() having
        // wired up GameData and the scene's child nodes (labels, sprite, weaponScene,
        // etc). Without a full scene tree they fail fast with a NullReferenceException
        // rather than silently misbehaving.
        [TestCase]
        [RequireGodotRuntime]
        public void Heal_WithPositiveAmount_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.Heal(10);
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void TakeDamage_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.TakeDamage(10);
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void GetHP_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.GetHP();
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void GetScore_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.GetScore();
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnUpdateScoreLabel_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.OnUpdateScoreLabel(5);
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnUpdateAmmoLabel_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.OnUpdateAmmoLabel(1);
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnAnimationFinished_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.OnAnimationFinished();
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnAutofireTimerTimeout_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.OnAutofireTimerTimeout();
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
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

        [TestCase]
        [RequireGodotRuntime]
        public void Input_TouchReleased_ThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                InputEventScreenTouch touch = new InputEventScreenTouch
                {
                    Index = 0,
                    Position = new Vector2(10, 10),
                    Pressed = false
                };
                player._Input(touch);
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
                player?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Input_DragEvent_UpdatesPositionThenThrowsNullReferenceException_WhenNotInitialized()
        {
            Player player = null;
            bool exceptionThrown = false;
            try
            {
                player = new Player();
                player.Position = Vector2.Zero;
                InputEventScreenDrag drag = new InputEventScreenDrag
                {
                    Index = 0,
                    Relative = new Vector2(10, 0)
                };
                player._Input(drag);
            }
            catch (NullReferenceException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            AssertThat(exceptionThrown).IsEqual(true);
            // OnDrag() updates Position before it reaches the sprite dereference that throws.
            AssertThat(player.Position).IsEqual(new Vector2(6, 0));
            player?.Free();
        }
    }
}
