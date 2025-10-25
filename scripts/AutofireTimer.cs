using Godot;
using System;

public partial class AutofireTimer : Timer
{
	[Signal]
	public delegate void AutofireTimeoutEventHandler();

	public void OnAutofireTimerTimeout()
	{
		EmitSignal(SignalName.AutofireTimeout);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		WaitTime = 0.3f;
    }
	
}
