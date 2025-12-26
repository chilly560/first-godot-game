using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/*
TODO: Figure out why Wave isn't being garbage collected properly,
- Check signals to make sure they are not keeping references alive
*/

namespace Game.Enemies.EnemySpawning
{
	/// <summary>
	/// Spawns waves of enemies based on patterns
	/// </summary>
	public partial class WaveSpawner : Spawner
	{
		private GameData gameData;

		private Wave currentWave;

		private EnemyActivationTimer enemyActivationTimer;

		private const int FREQUENCY = 3;
		
		public override void _Ready()
		{
			gameData = GameData.Get();
			gameData.WaveNumber++;
			gameData.WaveDestroyed += EnemyDestroyedSignalHandler;
			enemyActivationTimer = GetNode<EnemyActivationTimer>("EnemyActivationTimer");
			enemyActivationTimer.Start(FREQUENCY);
			currentWave = new Wave();
			currentWave.InstantiateWaveEntitites(GetParent<GameRoot>());
		}

	    private void EnemyDestroyedSignalHandler()
		{
			// TODO: This SignalHandler should pause the Bogey spawner and wait for the 
			// player to finish clearing enemies - after which a new wave should be spawned.
			// 
			// Don't forget - you're planning multiple types of waves to be spawnable!
			gameData.PauseSpawning = true;
			currentWave = null;
			GD.Print($"Residual Entities remaining: {gameData.Entities}");
			if (gameData.Entities == 0)
			    gameData.PauseSpawning = false;
		}
		public void OnEnemyActivationTimerTimeout()
		{
			if (currentWave != null && currentWave.GetCount() > 0)
			{
				currentWave.ActivateEnemy(true);
				enemyActivationTimer.Start(FREQUENCY);
			} else if (gameData.Entities == 0)
			{
				GD.Print("Wave cleared! Spawning new wave...");
				// Temporarily just spawning new default wave, can add logic for different wave types later.
				currentWave = null;
				//GC.Collect();
				//GC.WaitForPendingFinalizers();
				//GC.Collect();
				gameData.WaveNumber++;
				currentWave = new Wave(gameData.WaveNumber);
				currentWave.InstantiateWaveEntitites(GetParent<GameRoot>());
				enemyActivationTimer.Start(FREQUENCY);
			}
		}
	}
}
