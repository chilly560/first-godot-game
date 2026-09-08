namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game.Enemies;

    [TestSuite]
    public class GameDataTest
    {
        private const int MAX_HP = 100;

        /// <summary>
        /// Builds the node structure Flush() expects, since it resolves the player healthbar
        /// through the hardcoded path "../GameRoot/Camera2D/HUD/Healthbar".
        ///
        /// The fixture is deliberately left OUT of the live SceneTree: entering the tree would
        /// fire GameData._Ready(), which assigns the private static singleton that the rest of
        /// the game reads through GameData.Get(). Flush() performs the same field
        /// initialization without clobbering that global for other suites.
        /// </summary>
        private static GameData CreateGameDataFixture()
        {
            Node2D fixtureRoot = new Node2D();

            GameData gameData = new GameData();
            gameData.Name = "GameData";
            fixtureRoot.AddChild(gameData);

            Node2D gameRoot = new Node2D();
            gameRoot.Name = "GameRoot";
            fixtureRoot.AddChild(gameRoot);

            Node2D camera = new Node2D();
            camera.Name = "Camera2D";
            gameRoot.AddChild(camera);

            Node2D hud = new Node2D();
            hud.Name = "HUD";
            camera.AddChild(hud);

            Healthbar healthbar = new Healthbar();
            healthbar.Name = "Healthbar";
            hud.AddChild(healthbar);

            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "ProgressBar";
            healthbar.AddChild(progressBar);

            Timer timer = new Timer();
            timer.Name = "Timer";
            healthbar.AddChild(timer);

            // Healthbar caches its children in _Ready(), which only fires inside the live
            // tree, so call it directly to give SetHealth() a usable timer/progress bar.
            healthbar._Ready();
            gameData.Flush();

            return gameData;
        }

        /// <summary>
        /// Frees the whole fixture; the root owns every node built above.
        /// </summary>
        private static void FreeFixture(GameData gameData)
        {
            gameData?.GetParent()?.Free();
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Flush_ResetsHpScoreAndCounters()
        {
            GameData gameData = null;
            try
            {
                gameData = CreateGameDataFixture();
                gameData.CauseDamage(40);
                gameData.Score = 50;
                gameData.Entities = 3;
                gameData.WaveNumber = 7;
                gameData.PauseSpawning = true;

                gameData.Flush();

                AssertThat(gameData.GetHP()).IsEqual(MAX_HP);
                AssertThat(gameData.Score).IsEqual(0);
                AssertThat(gameData.Entities).IsEqual(0);
                AssertThat(gameData.WaveNumber).IsEqual(0);
                AssertThat(gameData.PauseSpawning).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CauseDamage_ReducesHpAndReportsPlayerStillAlive()
        {
            GameData gameData = null;
            try
            {
                gameData = CreateGameDataFixture();

                bool alive = gameData.CauseDamage(30);

                AssertThat(gameData.GetHP()).IsEqual(70);
                AssertThat(alive).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CauseDamage_ExceedingRemainingHp_ClampsToZeroAndReportsDead()
        {
            GameData gameData = null;
            try
            {
                gameData = CreateGameDataFixture();

                bool alive = gameData.CauseDamage(MAX_HP + 50);

                AssertThat(gameData.GetHP()).IsEqual(0);
                AssertThat(alive).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void CauseDamage_NegativeAmount_ThrowsArgumentException()
        {
            GameData gameData = null;
            bool exceptionThrown = false;
            try
            {
                gameData = CreateGameDataFixture();
                gameData.CauseDamage(-1);
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
                FreeFixture(gameData);
            }
            AssertThat(exceptionThrown).IsEqual(true);
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Heal_IncreasesHpByAmount()
        {
            GameData gameData = null;
            try
            {
                gameData = CreateGameDataFixture();
                gameData.CauseDamage(50);

                gameData.Heal(20);

                AssertThat(gameData.GetHP()).IsEqual(70);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Heal_BeyondMaxHp_ClampsToMaxHp()
        {
            GameData gameData = null;
            try
            {
                gameData = CreateGameDataFixture();
                gameData.CauseDamage(10);

                gameData.Heal(50);

                AssertThat(gameData.GetHP()).IsEqual(MAX_HP);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void Heal_WithNonPositiveAmount_LeavesHpUnchanged()
        {
            GameData gameData = null;
            try
            {
                gameData = CreateGameDataFixture();
                gameData.CauseDamage(40);

                gameData.Heal(0);
                gameData.Heal(-10);

                AssertThat(gameData.GetHP()).IsEqual(60);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
            }
        }

        // ----- Enemy registry -----

        [TestCase]
        [RequireGodotRuntime]
        public void AddEnemy_AndRemoveEnemy_TrackEnemiesById()
        {
            GameData gameData = null;
            Drone first = null, second = null;
            try
            {
                gameData = CreateGameDataFixture();
                first = new Drone();
                first.SetID(1);
                second = new Drone();
                second.SetID(2);

                gameData.AddEnemy(first);
                gameData.AddEnemy(second);
                AssertThat(gameData.GetNumberOfEnemies()).IsEqual(2);

                gameData.RemoveEnemy(1);
                AssertThat(gameData.GetNumberOfEnemies()).IsEqual(1);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                FreeFixture(gameData);
                first?.Free();
                second?.Free();
            }
        }

        // ----- Signal bus -----

        [TestCase]
        [RequireGodotRuntime]
        public void OnUpdateHUDEventHandler_EmitsUpdateAmmoLabelWithAmount()
        {
            GameData gameData = null;
            try
            {
                gameData = new GameData();
                int received = 0;
                gameData.UpdateAmmoLabel += plusMinus => received = plusMinus;

                gameData.OnUpdateHUDEventHandler(7);

                AssertThat(received).IsEqual(7);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameData?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnUpdateScoreEventHandler_EmitsUpdateScoreLabelWithAmount()
        {
            GameData gameData = null;
            try
            {
                gameData = new GameData();
                int received = 0;
                gameData.UpdateScoreLabel += plusMinus => received = plusMinus;

                gameData.OnUpdateScoreEventHandler(25);

                AssertThat(received).IsEqual(25);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameData?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void OnSignalWaveEnemyDestroyedEventHandler_ForwardsFormationCoords()
        {
            GameData gameData = null;
            try
            {
                gameData = new GameData();
                int receivedX = -1, receivedY = -1;
                bool receivedActivated = false;
                gameData.RemoveEnemyXYFromFormation += (x, y, activated) =>
                {
                    receivedX = x;
                    receivedY = y;
                    receivedActivated = activated;
                };

                gameData.OnSignalWaveEnemyDestroyedEventHandler(2, 3, true);

                AssertThat(receivedX).IsEqual(2);
                AssertThat(receivedY).IsEqual(3);
                AssertThat(receivedActivated).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameData?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void EmitWaveDestroyedEventHandlerSignal_EmitsWaveDestroyed()
        {
            GameData gameData = null;
            try
            {
                gameData = new GameData();
                bool fired = false;
                gameData.WaveDestroyed += () => fired = true;

                gameData.EmitWaveDestroyedEventHandlerSignal();

                AssertThat(fired).IsEqual(true);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameData?.Free();
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void EmitWaveBonusEventHandlerSignal_EmitsWaveBonusWithAmount()
        {
            GameData gameData = null;
            try
            {
                gameData = new GameData();
                int received = 0;
                gameData.WaveBonus += bonus => received = bonus;

                gameData.EmitWaveBonusEventHandlerSignal(500);

                AssertThat(received).IsEqual(500);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                gameData?.Free();
            }
        }
    }
}
