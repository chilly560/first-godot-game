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
	public partial class EnemySpawner2 : Area2D
	{

		public RayCast2D leftCollisionRay;
		public RayCast2D rightCollisionRay;
		private Timer spawnTimer;
		private const int SPEED = 40;
		private PackedScene enemyScene = (PackedScene)ResourceLoader.Load("res://scenes/enemy.tscn");
		public int dir = 1;
		/// <summary>
		/// Defines additional physics modification for this IDynamic2DPhysicsObject
		/// </summary>
		private Action<EnemySpawner2> physicsModifier;
		/// <summary>
		/// Defines physics behavior for this IDynamic2DPhysicsObject
		/// </summary>
		private Action<EnemySpawner2, double> physicsOverhauler;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			leftCollisionRay = GetNode<RayCast2D>("./RayCast2D2");
			rightCollisionRay = GetNode<RayCast2D>("./RayCast2D");
			SetPhysicsOverhauler(SpawnerPhysicsOverhaulers.DefaultPhysics);
			spawnTimer = GetNode<Timer>("./SpawnTimer");
			spawnTimer.WaitTime = 2f;
			spawnTimer.Start();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _PhysicsProcess(double delta)
		{
			if (physicsOverhauler != null)
			{
				physicsOverhauler.Invoke(this, delta);

				if (physicsModifier != null)
					physicsModifier.Invoke(this);
			}
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

		public void SetPhysicsModifier(Action<EnemySpawner2> del)
		{
			physicsModifier = del;
		}

		public void SetPhysicsOverhauler(Action<EnemySpawner2, double> del)
		{
			physicsOverhauler = del;
		}
	}
}