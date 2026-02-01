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
		
		private Random random;

		public override void _Ready()
		{
			gameData = GameData.Get();
			random = new Random();
			gameData.WaveNumber++;
			gameData.WaveDestroyed += WaveDestroyedSignalHandler;
			enemyActivationTimer = GetNode<EnemyActivationTimer>("EnemyActivationTimer");
			enemyActivationTimer.Start(FREQUENCY);
			currentWave = new Wave();
			currentWave.InstantiateWaveEntitites(GetParent<GameRoot>());
		}

	    private void WaveDestroyedSignalHandler()
		{
			// TODO: This SignalHandler should pause the Bogey spawner and wait for the 
			// player to finish clearing enemies - after which a new wave should be spawned.
			// 
			// Don't forget - you're planning multiple types of waves to be spawnable!
			gameData.PauseSpawning = true;
			currentWave = null;
			GD.Print($"Residual Entities remaining: {gameData.Entities}");
		}
		public override void _ExitTree()
		{
			// Disconnect signals BEFORE calling base._ExitTree()
			gameData.WaveDestroyed -= WaveDestroyedSignalHandler;
			base._ExitTree();
		}
		public void OnEnemyActivationTimerTimeout()
		{
			if (currentWave != null && currentWave.GetCount() > 0)
			{
				currentWave.ActivateEnemy(true);
				enemyActivationTimer.Start(FREQUENCY);
			} else if (gameData.Entities == 0)
			{
				//GD.Print("Wave cleared! Spawning new wave...");
				// Temporarily just spawning new default wave, can add logic for different wave types later.
				currentWave = null;
				gameData.WaveNumber++;
				int pattern = random.Next(0,4);
				switch (pattern)
				{
					case 0:
						currentWave = new Wave(gameData.WaveNumber, WavePattern.AGGRESSIVE);
						break;
					case 1:
						currentWave = new Wave(gameData.WaveNumber, WavePattern.AGGRESSIVE);
						break;
					// temp unless more patterns are added
					default:
						currentWave = new Wave(gameData.WaveNumber, WavePattern.DEFAULT);
						break;
				}
				currentWave.InstantiateWaveEntitites(GetParent<GameRoot>());
				enemyActivationTimer.Start(FREQUENCY);
				gameData.PauseSpawning = false;
			}
		}
	}
}
