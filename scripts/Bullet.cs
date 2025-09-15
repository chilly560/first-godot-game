using Godot;
using System;

//public partial class Bullet : RigidBody2D
public partial class Bullet : Area2D
{
	private int speed { get; set;  } = 750;

	public int Damage { get; set; } = 20;

	public BulletTimer BulletTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/** Called every physics frame. 'delta' is the elapsed time since the previous frame.
	 * Used for bullet movement, regardless of framerate
	 @param delta Time elapsed since the previous frame
	 */
	public override void _PhysicsProcess(double delta)
	{
		Position += -1 * Transform.Y * speed * (float)delta;
	}

	public void SetTimer(float time)
	{
		this.BulletTimer = GetNode<BulletTimer>("BulletTimer");
		this.BulletTimer.WaitTime = time;
		this.BulletTimer.Start();
	}

	/**
	 * Called when the bullet collides with another body
	 */
	public void OnAreaEnteredBullet(Node body)
	{
		GD.Print("Bullet collided with something");
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			this.QueueFree();
			GD.Print("Hit");
		}
		else if (body is Player)
			GD.Print("Bug");
	}
}
