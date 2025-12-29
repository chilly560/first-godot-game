using Godot;
using System;
using Game.Enemies;
using Game;
using Game.Enemies.EnemySpawning.SpawnerPhysics;

/*
TODO: Move spawner back within arena bounds
*/

namespace Game.Enemies.EnemySpawning
{
	/// <summary>
    /// An enemy spawner that moves horizontally and spawns enemies at intervals.
	/// 
	/// Unironically might repurpose this to be a "mothership" boss lmao
    /// </summary>
	public partial class EnemySpawner2 : Spawner
	{
		/// <summary>
        /// Raycast to detect collisions on the left side.
        /// </summary>
		public RayCast2D leftCollisionRay;
		/// <summary>
        /// Raycast to detect collisions on the right side.
        /// </summary>
		public RayCast2D rightCollisionRay;
		/// <summary>
        /// Timer to control enemy spawning intervals.
        /// </summary>
		private Timer spawnTimer;
		/// <summary>
        /// Defines default movement speed for the spawner.
        /// </summary>
		private const int SPEED = 40;
		/// <summary>
        /// Reference to the enemy scene to be spawned.
		/// 
		/// TODO: Going to try removing this, as I have an EnemyFactory now.
        /// </summary>
		private PackedScene enemyScene = (PackedScene)ResourceLoader.Load("res://scenes/enemy.tscn");
		/// <summary>
        /// Direction of movement: 1 for right, -1 for left.
        /// </summary>
		public int dir = 1;
		private GameData gameData;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			gameData = GameData.Get();
			leftCollisionRay = GetNode<RayCast2D>("./RayCast2D2");
			rightCollisionRay = GetNode<RayCast2D>("./RayCast2D");
			SetPhysicsOverhauler(SpawnerPhysicsOverhaulers.MothershipDefaultPhyysics);
			spawnTimer = GetNode<Timer>("./SpawnTimer");
			spawnTimer.WaitTime = 2f;
			spawnTimer.Start();
		}
		/// <summary>
        /// Creates an enemy at the spawner's global position when the timer timeouts.
        /// </summary>
		public void OnSpawnTimerTimeout()
		{
			if ((!gameData.PauseSpawning) && (gameData.Entities < gameData.EntityCap))
			{				
				GD.Print("---------------------");
				GD.Print("INCREASING ENTITIES");
				GD.Print($"Entities ({gameData.Entities})++");
				gameData.Entities++;
				GD.Print($"Entities ({gameData.Entities})");
				GD.Print($"SPAWNING ENEMY");
				SpawnBogey(GlobalPosition);
				GD.Print($"Entities after spawn: {gameData.Entities}");
				GD.Print("---------------------");
			}

			spawnTimer.Start();
		}
	}
}