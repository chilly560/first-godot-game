using Godot;
using System;

public partial class BulletTimer : Timer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Timer ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnTimerTimeout()
	{
		GetParent<Bullet>().QueueFree();
		GD.Print("Timer timeout, bullet freed");
	}
}
