namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game;
    using Game.Enemies;
    using Game.Enemies.EnemySpawning;
    using Game.Enemies.EnemySpawning.SpawnerPhysics;

    [TestSuite]
    public class EnemyTest
    {
        // Mirrors the private DEFAULT_SPEED constant in SpawnerPhysicsOverhaulers.
        private const int DEFAULT_SPEED = 40;

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_Drone_ReturnsInstanceOfDrone()
        {
            Enemy enemy = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.DRONE, Vector2.Zero);
                AssertThat(enemy).IsNotNull();
                AssertThat(enemy).IsInstanceOf<Drone>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_Bogey_ReturnsInstanceOfBogey()
        {
            Enemy enemy = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.BOGEY, Vector2.Zero);
                AssertThat(enemy).IsNotNull();
                AssertThat(enemy).IsInstanceOf<Bogey>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_WaveDrone_ReturnsInstanceOfWaveDrone()
        {
            Enemy enemy = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.WAVE_DRONE, Vector2.Zero);
                AssertThat(enemy).IsNotNull();
                AssertThat(enemy).IsInstanceOf<WaveDrone>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_WaveBogey_ReturnsInstanceOfWaveBogey()
        {
            Enemy enemy = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.WAVE_BOGEY, Vector2.Zero);
                AssertThat(enemy).IsNotNull();
                AssertThat(enemy).IsInstanceOf<WaveBogey>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_UnrecognizedEnemyClassification_ThrowsArgumentException()
        {
            bool exceptionThrown = false;
            try
            {
                EnemyFactory.CreateEnemy((EnemyClassification)99, Vector2.Zero);
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
        public void CreateEnemy_SetsPositionToGivenVector2()
        {
            Enemy enemy = null;
            try
            {
                Vector2 position = new Vector2(100, 200);
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.DRONE, position);
                AssertThat(enemy.Position).IsEqual(position);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_AssignsUniqueIncrementingID()
        {
            Enemy first = null, second = null;
            try
            {
                first = EnemyFactory.CreateEnemy(EnemyClassification.DRONE, Vector2.Zero);
                second = EnemyFactory.CreateEnemy(EnemyClassification.DRONE, Vector2.Zero);
                AssertThat(second.GetID()).IsGreater(first.GetID());
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                first?.QueueFree();
                second?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_ReturnsInstanceOfIEnemy()
        {
            Enemy enemy = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.BOGEY, Vector2.Zero);
                AssertThat(enemy).IsInstanceOf<IEnemy>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CreateEnemy_ReturnsInstanceOfICollector()
        {
            Enemy enemy = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.WAVE_BOGEY, Vector2.Zero);
                AssertThat(enemy).IsInstanceOf<ICollector>();
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnBodyEnteredEnemy_WithNonPlayerNode_DoesNotThrow()
        {
            Enemy enemy = null;
            Node2D node = null;
            try
            {
                enemy = EnemyFactory.CreateEnemy(EnemyClassification.DRONE, Vector2.Zero);
                node = new Node2D();
                enemy.OnBodyEnteredEnemy(node);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                enemy?.QueueFree();
                node?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithoutOverhauler_DoesNotThrow()
        {
            WaveSpawner spawner = null;
            try
            {
                spawner = new WaveSpawner();
                spawner._PhysicsProcess(0.1);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                spawner?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithOverhauler_InvokesOverhauler()
        {
            WaveSpawner spawner = null;
            try
            {
                spawner = new WaveSpawner();
                bool overhaulerInvoked = false;
                spawner.SetPhysicsOverhauler((s, delta) => overhaulerInvoked = true);
                spawner._PhysicsProcess(0.1);
                AssertThat(overhaulerInvoked).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                spawner?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithOverhaulerAndModifier_InvokesModifier()
        {
            WaveSpawner spawner = null;
            try
            {
                spawner = new WaveSpawner();
                bool modifierInvoked = false;
                spawner.SetPhysicsOverhauler((s, delta) => { });
                spawner.SetPhysicsModifier(s => modifierInvoked = true);
                spawner._PhysicsProcess(0.1);
                AssertThat(modifierInvoked).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                spawner?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void PhysicsProcess_WithModifierButNoOverhauler_DoesNotInvokeModifier()
        {
            WaveSpawner spawner = null;
            try
            {
                spawner = new WaveSpawner();
                bool modifierInvoked = false;
                spawner.SetPhysicsModifier(s => modifierInvoked = true);
                spawner._PhysicsProcess(0.1);
                AssertThat(modifierInvoked).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                spawner?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void SpawnBogey_AddsBogeyToParentAtGivenPosition()
        {
            Node2D root = null;
            try
            {
                root = new Node2D();
                WaveSpawner spawner = new WaveSpawner();
                root.AddChild(spawner);

                Vector2 spawnPosition = new Vector2(150, 75);
                spawner.SpawnBogey(spawnPosition);

                bool bogeyFound = false;
                foreach (Node child in root.GetChildren())
                {
                    if (child is Bogey bogey)
                    {
                        bogeyFound = true;
                        AssertThat(bogey.Position).IsEqual(spawnPosition);
                    }
                }
                AssertThat(bogeyFound).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                root?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void MothershipDefaultPhysics_NoCollision_MovesInCurrentDirection()
        {
            EnemySpawner2 spawner = null;
            try
            {
                spawner = new EnemySpawner2();
                spawner.leftCollisionRay = new RayCast2D();
                spawner.rightCollisionRay = new RayCast2D();
                spawner.dir = 1;
                spawner.Position = Vector2.Zero;

                SpawnerPhysicsOverhaulers.MothershipDefaultPhyysics(spawner, 1.0);

                AssertThat(spawner.dir).IsEqual(1);
                AssertThat(spawner.Position).IsEqual(new Vector2(DEFAULT_SPEED, 0));
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                spawner?.leftCollisionRay?.Free();
                spawner?.rightCollisionRay?.Free();
                spawner?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void MothershipDefaultPhysics_NonEnemySpawner2_DoesNotThrowOrMove()
        {
            WaveSpawner spawner = null;
            try
            {
                spawner = new WaveSpawner();
                spawner.Position = Vector2.Zero;

                SpawnerPhysicsOverhaulers.MothershipDefaultPhyysics(spawner, 1.0);

                AssertThat(spawner.Position).IsEqual(Vector2.Zero);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                spawner?.Free();
            }
        }
    }
}
