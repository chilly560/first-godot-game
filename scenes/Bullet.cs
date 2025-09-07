using Godot;
using System; 

public partial class Bullet : Area2D
{
	private int speed = 750;

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
	 */
    public override void _PhysicsProcess(double delta)
	{
		Position += -1 * Transform.Y * speed * (float)delta;
	}


	/**
	 * Called when the bullet collides with another body
	 */
	public void OnBodyEnteredBullet(Node body)
	{
		if (body is Enemy enemy)
		{
			GD.Print("Bullet hit enemy");
			enemy.QueueFree();
			this.QueueFree();
		}
		else GD.Print("Miss");
	}
}
