using Godot;
using System;

public partial class Restart : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/** Called when the restart button is pressed
	 * Changes the scene to the main game scene
	 */
	public void OnButtonPressedRestart()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}
}
