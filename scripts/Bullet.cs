using Godot;
using System;
using Game.Weapons;
using Game.Enemies;
using Game;

/// <summary>
/// Represents a bullet projectile used by weapons. Supports configurable
/// speed, damage and range, and allows custom physics behavior via
/// delegates.
/// </summary>
public partial class Bullet : Area2D, IDynamic2DPhysicsObject<Bullet>
{
	/// <summary>
	/// Movement speed of the bullet in pixels per second.
	/// </summary>
	public float speed { get; set; } = 750;

	/// <summary>
	/// Damage applied to an entity when this bullet hits it.
	/// </summary>
	public int Damage { get; set; } = 20;

	/// <summary>
	/// Lifetime or range of the bullet in seconds. Used by timers.
	/// </summary>
	public float Range { get; set; } = 1;

	/// <summary>
	/// Optional timer node used for bullet lifetime management.
	/// </summary>
	public BulletTimer BulletTimer;

	private bool freeRotate = false;

	private Action<Bullet> bulletPhysicsModifier;

	private Action<Bullet, double> bulletPhysicsOverhauler;

	/// <summary>
	/// Enables continuous rotation for the bullet (visual effect).
	/// </summary>
	public void AllowFreeRotate()
	{
		freeRotate = true;
	}

	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		bulletPhysicsModifier = null;
		bulletPhysicsOverhauler = null;
	}

	/// <summary>
	/// Called every frame. Used for non-physics visual updates.
	/// </summary>
	/// <param name="delta">Time elapsed since last frame, in seconds.</param>
	public override void _Process(double delta)
	{
		if (freeRotate)
			Rotation += 0.1f;
	}

	/// <summary>
	/// Called every physics frame. Moves the bullet according to the
	/// configured physics delegates or the default straight-line motion.
	/// </summary>
	/// <param name="delta">Time elapsed since the previous physics frame, in seconds.</param>
	public override void _PhysicsProcess(double delta)
	{
		if (bulletPhysicsOverhauler == null)
			Position += -1 * Transform.Y * speed * (float)delta;
		else
			bulletPhysicsOverhauler(this, delta);

		bulletPhysicsModifier?.Invoke(this);
	}

	/// <summary>
	/// Deprecated. Legacy method used by the shotgun implementation to
	/// start an internal lifetime timer for the bullet.
	/// </summary>
	[Obsolete("ShootBullet is deprecated due to only being compatible with shotgun.")]
	public void ShootBullet()
	{
		this.BulletTimer = GetNode<BulletTimer>("./BulletTimer");
		this.BulletTimer.WaitTime = this.Range;
		this.BulletTimer.Start();
	}

	/// <summary>
	/// Called when this bullet's area collides with another area.
	/// Applies damage to <see cref="Game.Enemies.Enemy"/> instances and
	/// frees the bullet.
	/// </summary>
	/// <param name="body">The node that entered this bullet's area.</param>
	public virtual void OnAreaEnteredBullet(Node body)
	{
		if (body is Game.Enemies.Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			QueueFree();
		}
	}

	/// <summary>
	/// Called when this bullet collides with a physics body.
	/// Applies damage to <see cref="Player"/> instances and frees the bullet.
	/// </summary>
	/// <param name="body">The node that entered this bullet's body collision.</param>
	public void OnBodyEnteredBullet(Node body)
	{
		//Debug
		GD.Print((body as Node2D).Position);
		GD.Print(this.Position);
		
		if (body is Player player)
		{
			player.TakeDamage(Damage);
			QueueFree();
		}
	}

	/// <summary>
	/// Sets the bullet's fundamental stats.
	/// </summary>
	/// <param name="damage">Damage dealt on hit.</param>
	/// <param name="speed">Movement speed (pixels per second).</param>
	/// <param name="range">Optional lifetime/range in seconds. Default is 1.</param>
	public void SetStats(int damage, int speed, float range = 1)
	{
		Damage = damage;
		this.speed = speed;
		Range = range;
	}

	/// <summary>
	/// Assigns a delegate that will be invoked each physics frame after
	/// the bullet movement step. Use to apply custom per-frame physics changes.
	/// </summary>
	/// <param name="del">Delegate that receives the current <see cref="Bullet"/> instance.</param>
	public void SetPhysicsModifier(Action<Bullet> del)
	{
		this.bulletPhysicsModifier = del;
	}
    
	/// <summary>
	/// Assigns a delegate that fully controls the bullet's physics update.
	/// If set, it is called instead of the default movement logic.
	/// </summary>
	/// <param name="del">Delegate receiving the current <see cref="Bullet"/> and the physics <c>delta</c>.</param>
	public void SetPhysicsOverhauler(Action<Bullet, double> del)
	{
		this.bulletPhysicsOverhauler = del;
	}
}