using Godot;

public partial class Restart : Button
{
	public void OnButtonPressedRestart()
	{
		//GameData.Get().Flush();
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}
}
