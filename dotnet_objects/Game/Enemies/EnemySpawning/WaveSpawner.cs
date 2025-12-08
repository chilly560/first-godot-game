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
		private Wave currentWave;

		public override void _Ready()
		{
			GD.Print("Spawning Wave!");
			currentWave = new Wave();
            currentWave.InstantiateWaveEntitites(GetParent<GameRoot>());
		}
	}
}
