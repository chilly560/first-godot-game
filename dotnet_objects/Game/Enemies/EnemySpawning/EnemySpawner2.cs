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

		public RayCast2D leftCollisionRay;
		public RayCast2D rightCollisionRay;
		private Timer spawnTimer;
		private const int SPEED = 40;
		private PackedScene enemyScene = (PackedScene)ResourceLoader.Load("res://scenes/enemy.tscn");
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

		public void OnSpawnTimerTimeout()
		{
			GD.Print("Spawning enemy");
			GetParent().AddChild(
				EnemyFactory.CreateEnemy(
					//EnemyClassification.DRONE,
					EnemyClassification.BOGEY,
					GlobalPosition
				)
			);
			spawnTimer.Start();
		}
	}
}