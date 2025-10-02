using Godot;
using System;
using Game.Weapons;
using Game.Enemies;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

public partial class Bullet : Area2D
{
	public float speed { get; set; } = 750;

	public int Damage { get; set; } = 20;

	public float Range { get; set; } = 1;

	public BulletTimer BulletTimer;

	private bool freeRotate = false;
	private Action<Bullet> bulletPhysicsModifier;
	public void AllowFreeRotate()
	{
		freeRotate = true;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletPhysicsModifier = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (freeRotate)
			Rotation += 0.1f;
	}

	/** Called every physics frame. 'delta' is the elapsed time since the previous frame.
	* Used for bullet movement, regardless of framerate
	@param delta Time elapsed since the previous frame
	*/
	public override void _PhysicsProcess(double delta)
	{
		Position += -1 * Transform.Y * speed * (float)delta;
		if (bulletPhysicsModifier != null)
			bulletPhysicsModifier(this);
		//speed = speed * 0.9525f;
	}

	public void ShootBullet()
	{
		this.BulletTimer = GetNode<BulletTimer>("./BulletTimer");
		this.BulletTimer.WaitTime = this.Range;
		this.BulletTimer.Start();
	}

	/**
	* Called when the bullet collides with another body
	*/
	public void OnAreaEnteredBullet(Node body)
	{
		GD.Print("Bullet collided with something");

		// Makes no sense but I have to fully qualify the Enemy class name here
		// or it doesn't recognize it.
		// I'm sure there's a reason but I don't know what it is yet.
		if (body is Game.Enemies.Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			this.QueueFree();
			GD.Print("Hit");
		}
		else if (body is Player)
			GD.Print("Bug");
	}

	/**
	* Sets the stats of the bullet
	* @param damage The damage the bullet will deal
	* @param speed The speed of the bullet
	* @param range Timer time
	*/
	public void SetStats(int damage, int speed, float range = 1)
	{
		this.Damage = damage;
		this.speed = speed;
		this.Range = range;
	}


	/// <summary>
	/// Sets a custom physics modifier for the bullet.
	/// </summary>
	/// <param name="del">A delegate that defines the custom physics behavior for the bullet. 
	/// The delegate takes a <see cref="Bullet"/> instance as input and returns an object representing the result of the physics modification.</param>
	public void SetPhysics(Action<Bullet> del)
	{
		this.bulletPhysicsModifier = del;
	}
}