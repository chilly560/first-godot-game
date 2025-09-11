using Godot;
using System;

public partial class Enemy : Area2D
{
	private GameData gameData;

	private int enemyid;

	private int hp;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.gameData = GetNode<GameData>("%GameData");
		this.enemyid = this.gameData.GetNumberOfEnemies();
		this.gameData.AddEnemy(this);
		this.hp = 100;
		GD.Print(this.gameData.GetNumberOfEnemies());
		GD.Print("[LOG] Spawned Enemy");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/** Called when another body enters the enemy's area
	 * If the body is the player, deal damage to the player
	 @param body The body that entered the enemy's area
	 */
	public void OnBodyEnteredEnemy(Node body)
	{
		GD.Print("Enemy collided with something");
		if (body is Player player)
		{
			GD.Print("Player entered body");
			player.TakeDamage(50);
		}
		else if (body is Bullet bullet)
		{
			GD.Print("Enemy Hit");
		}
	}

	public int GetID()
	{
		return enemyid;
	}
	
	public void TakeDamage(int amount)
	{
		GD.Print("Taking Damage...");
		this.hp -= amount;
		GD.Print("[LOG] Enemy took " + amount + " damage, remaining HP: " + this.hp);
		if (this.hp <= 0)
		{
			//this.gameData.RemoveEnemy(this.enemyid);
			this.QueueFree();
		}
	}
}
