using Godot;
using System;
using Game.Enemies;
using Game;
using Game.Enemies.EnemySpawning.SpawnerPhysics;

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

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
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
			SpawnBogey(GlobalPosition);
			spawnTimer.Start();
		}
	}
}