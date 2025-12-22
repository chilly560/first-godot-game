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
			throw new NotImplementedException("EnemyDestroyedSignalHandler succesfully invoked!");
		}

		public void OnEnemyActivationTimerTimeout()
		{
			currentWave.ActivateEnemy(true);
			enemyActivationTimer.Start(FREQUENCY);
		}
	}
}
