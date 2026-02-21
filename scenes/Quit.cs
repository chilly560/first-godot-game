using Godot;
using System;

public partial class Quit : TouchScreenButton
{
	/// <summary>
	/// Handles the quit button press event.
	/// </summary>
	public void OnButtonPressedQuit()
	{
		CallDeferred(nameof(DeferredQuit));
	}
  
	/// <summary>
	/// Necessary to defer quit code to avoid undefined behavior during shutdown.
	/// </summary>
	private void DeferredQuit()
	{
		GetTree().Quit(0);
	}
}
