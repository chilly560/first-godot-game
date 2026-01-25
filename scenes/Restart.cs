using Godot;
using System;

public partial class Restart : Button
{
	public void OnButtonPressedRestart()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}
}
