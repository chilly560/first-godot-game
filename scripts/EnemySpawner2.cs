using Godot;
using System;
using Game.Enemies;

public partial class EnemySpawner2 : Area2D
{

	private RayCast2D leftCollisionRay;
	private RayCast2D rightCollisionRay;
	private Timer spawnTimer;
	private const int SPEED = 40;
	private PackedScene enemyScene = (PackedScene)ResourceLoader.Load("res://scenes/Enemy.tscn");
	private int dir = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.leftCollisionRay = GetNode<RayCast2D>("./RayCast2D2");
		this.rightCollisionRay = GetNode<RayCast2D>("./RayCast2D");
		this.spawnTimer = GetNode<Timer>("./SpawnTimer");
		this.spawnTimer.WaitTime = 2f;
		this.spawnTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// For some reason, the raycasts trigger from the base of the arrow?
		if (this.rightCollisionRay.IsColliding())
			dir = -1;
		else if (this.leftCollisionRay.IsColliding())
			dir = 1;

		Position += Transform.X * SPEED * (float)delta * dir;
	}

	public void OnSpawnTimerTimeout()
	{
		GD.Print("Spawning enemy");
		GetParent().AddChild(
			EnemyFactory.CreateEnemy(
				EnemyClassification.DRONE,
				this.GlobalPosition
			)
		);
		this.spawnTimer.Start();
	}
}
