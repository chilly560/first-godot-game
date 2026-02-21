using Godot;

public partial class Restart : TouchScreenButton
{
	public void OnButtonPressedRestart()
	{
		//GameData.Get().Flush();
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}
}
