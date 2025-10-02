using Godot;
using System;

public partial class BogeyDelayTimer : Timer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartTimer(float time)
	{
		this.WaitTime = time;
		this.Start();
	}

	public void OnBogeyDelayTimerTimeout()
	{
		
	}
}
