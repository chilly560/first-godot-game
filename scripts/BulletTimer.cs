using Godot;
using System;

public partial class BulletTimer : Timer
{
	public void OnTimerTimeout()
	{
		GetParent<Bullet>().QueueFree();
	}
}
