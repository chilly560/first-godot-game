using Godot;

public partial class Restart : Button
{
	public void OnButtonPressedRestart()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}
}
