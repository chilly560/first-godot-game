using Godot;
using System;

public partial class GameOver : Node2D
{
	private Label scoreValue, waveValue;
	private GameData gameData;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameData = GameData.Get();
		scoreValue = GetNode<Label>("./ScoreValue");
		waveValue = GetNode<Label>("./WaveValue");
		scoreValue.Text = gameData.Score.ToString();
		waveValue.Text = gameData.WaveNumber.ToString();
	}
}
