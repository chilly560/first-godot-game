using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

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
			throw new NotImplementedException("EnemyDestroyedSignalHandler succesfully invoked!");
		}
		public void OnEnemyActivationTimerTimeout()
		{
			currentWave.ActivateEnemy(true);
			enemyActivationTimer.Start(FREQUENCY);
		}
	}
}
