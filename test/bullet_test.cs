namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game.Enemies;

    [TestSuite]
    public class BulletTest
    {
        // ----- Bullet -----

        [TestCase]
        [RequireGodotRuntime]
        public void SetStats_AssignsProvidedValues()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet.SetStats(50, 300, 2.5f);

                AssertThat(bullet.Damage).IsEqual(50);
                AssertThat(bullet.speed).IsEqual(300f);
                AssertThat(bullet.Range).IsEqual(2.5f);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void SetStats_WithoutRange_DefaultsRangeToOne()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet.SetStats(50, 300);

                AssertThat(bullet.Range).IsEqual(1f);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Ready_DoesNotThrow()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet._Ready();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Process_WithoutFreeRotate_DoesNotChangeRotation()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet._Process(0.1);

                AssertThat(bullet.Rotation).IsEqual(0f);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void AllowFreeRotate_CausesRotationToIncreaseOnProcess()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet.AllowFreeRotate();
                bullet._Process(0.1);

                AssertThat(bullet.Rotation).IsEqual(0.1f);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithoutOverhauler_MovesInDefaultDirection()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet.SetStats(10, 750);
                bullet.Position = Vector2.Zero;

                bullet._PhysicsProcess(1.0);

                // Default movement: Position += -1 * Transform.Y * speed * delta.
                // Transform.Y is (0, 1) for an unrotated Node2D, so this moves straight up.
                AssertThat(bullet.Position).IsEqual(new Vector2(0, -750));
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithOverhauler_InvokesOverhaulerInsteadOfDefaultMovement()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet.Position = Vector2.Zero;
                bool overhaulerInvoked = false;
                bullet.SetPhysicsOverhauler((b, delta) => overhaulerInvoked = true);

                bullet._PhysicsProcess(1.0);

                AssertThat(overhaulerInvoked).IsEqual(true);
                AssertThat(bullet.Position).IsEqual(Vector2.Zero);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithModifier_InvokesModifierAfterMovement()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bool modifierInvoked = false;
                bullet.SetPhysicsOverhauler((b, delta) => { });
                bullet.SetPhysicsModifier(b => modifierInvoked = true);

                bullet._PhysicsProcess(1.0);

                AssertThat(modifierInvoked).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
            }
        }

#pragma warning disable CS0618 // ShootBullet is deprecated but still exercised by WeaponFactory's Shotgun path.
        [TestCase]
        [RequireGodotRuntime]
        public void ShootBullet_StartsChildBulletTimerWithConfiguredRange()
        {
            // Timer.Start() is a no-op (logged engine error, no exception) unless the timer is
            // actually inside the live SceneTree, so the bullet has to be added to the real tree
            // for IsStopped() to reflect a running timer.
            SceneTree tree = (SceneTree)Engine.GetMainLoop();
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                bullet.SetStats(10, 500, 2.5f);
                BulletTimer timer = new BulletTimer();
                timer.Name = "BulletTimer";
                bullet.AddChild(timer);
                tree.Root.AddChild(bullet);

                bullet.ShootBullet();

                AssertThat(timer.WaitTime).IsEqual(2.5);
                AssertThat(timer.IsStopped()).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.GetParent()?.RemoveChild(bullet);
                bullet?.Free();
            }
        }
#pragma warning restore CS0618

        [TestCase]
        [RequireGodotRuntime]
        public void OnAreaEnteredBullet_WithNonEnemyNode_DoesNotThrow()
        {
            Bullet bullet = null;
            Node2D node = null;
            try
            {
                bullet = new Bullet();
                node = new Node2D();
                bullet.OnAreaEnteredBullet(node);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
                node?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnBodyEnteredBullet_WithNonPlayerNode_DoesNotThrow()
        {
            Bullet bullet = null;
            Node2D node = null;
            try
            {
                bullet = new Bullet();
                node = new Node2D();
                bullet.OnBodyEnteredBullet(node);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                bullet?.Free();
                node?.Free();
            }
        }

        // ----- BulletTimer -----

        [TestCase]
        [RequireGodotRuntime]
        public void OnTimerTimeout_WithBulletParent_QueuesBulletForDeletion()
        {
            Bullet bullet = null;
            try
            {
                bullet = new Bullet();
                BulletTimer timer = new BulletTimer();
                bullet.AddChild(timer);

                timer.OnTimerTimeout();

                AssertThat(bullet.IsQueuedForDeletion()).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                if (bullet is not null && !bullet.IsQueuedForDeletion())
                    bullet.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnTimerTimeout_WithNonBulletParent_ThrowsInvalidCastException()
        {
            Node2D parent = null;
            bool exceptionThrown = false;
            try
            {
                parent = new Node2D();
                BulletTimer timer = new BulletTimer();
                parent.AddChild(timer);

                timer.OnTimerTimeout();
            }
            catch (InvalidCastException)
            {
                exceptionThrown = true;
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
            finally
            {
                parent?.Free();
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        // ----- EnemyBullet -----

        [TestCase]
        [RequireGodotRuntime]
        public void EnemyBullet_Ready_DoesNotThrow()
        {
            EnemyBullet enemyBullet = null;
            try
            {
                enemyBullet = new EnemyBullet();
                enemyBullet._Ready();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemyBullet?.Free();
            }
        }

        // Unlike the base Bullet, EnemyBullet's override only reacts to Player bodies, so an
        // Enemy passed to it should be safely ignored rather than damaged.
        [TestCase]
        [RequireGodotRuntime]
        public void EnemyBullet_OnAreaEnteredBullet_WithEnemyNode_DoesNotThrowOrDamage()
        {
            EnemyBullet enemyBullet = null;
            Drone enemy = null;
            try
            {
                enemyBullet = new EnemyBullet();
                enemy = new Drone();
                enemyBullet.OnAreaEnteredBullet(enemy);

                AssertThat(enemyBullet.IsQueuedForDeletion()).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemyBullet?.Free();
                enemy?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void EnemyBullet_OnAreaEnteredBullet_WithUnrelatedNode_DoesNotThrow()
        {
            EnemyBullet enemyBullet = null;
            Node2D node = null;
            try
            {
                enemyBullet = new EnemyBullet();
                node = new Node2D();
                enemyBullet.OnAreaEnteredBullet(node);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemyBullet?.Free();
                node?.Free();
            }
        }
    }
}
