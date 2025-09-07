using Godot;
using System;

public partial class Enemy : Area2D
{
	private GameData gameData;
	
	private int enemyid;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.gameData = GetNode<GameData>("%GameData");
		this.enemyid = this.gameData.GetNumberOfEnemies();
		this.gameData.AddEnemy(this);
		GD.Print(this.gameData.GetNumberOfEnemies());
		GD.Print("[LOG] Spawned Enemy");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// TODO: not working, needs investigation
	public void OnArea2DBodyEntered(Node body)
	{
		if (body is Player player) {
			GD.Print("Looks like it's working :D");
			player.TakeDamage(50);
		}
		else GD.Print("Wtf bro");
	}
	
	public int GetID()
	{
		return enemyid;
	}
}
