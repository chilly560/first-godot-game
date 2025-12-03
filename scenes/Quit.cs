using Godot;
using System;

public partial class Quit : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
 
	/// <summary>
    /// Handles the quit button press event.
    /// </summary>
	public void OnButtonPressedQuit()
	{
		CallDeferred(nameof(DeferredQuit));
	}
  
	/// <summary>
    /// Necessary to deferr quit code to avoid undefined behavior during shutdown.
    /// </summary>
	private void DeferredQuit()
    {
        GetTree().Quit(0);
    }
}
