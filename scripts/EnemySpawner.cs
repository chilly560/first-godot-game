using Godot;
using System;

public partial class EnemySpawner : Marker2D
{

	private RayCast2D leftCollisionRay;
	private RayCast2D rightCollisionRay;

	private const int SPEED = 40;

	private int dir = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.leftCollisionRay = GetNode<RayCast2D>("./RayCast2D2");
		this.rightCollisionRay = GetNode<RayCast2D>("./RayCast2D");
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
}
