using Godot;
using System;
using System.Collections.Generic;
using Game.Enemies;

public partial class GameData : Node
{
	
	private int HP;
	
	private Dictionary<int, Enemy> enemies;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.HP = 100;
		this.enemies = new Dictionary<int, Enemy>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public int GetNumberOfEnemies()
	{
		return enemies.Count;
	}
	
	public void AddEnemy(Enemy enemy) 
	{
		this.enemies.Add(enemy.GetID(), enemy);
	}

	public void RemoveEnemy(int enemyId)
	{
		enemies.Remove(enemyId);
	}
	
	public int GetHP() 
	{
		return this.HP;	
	}
	
	/** Returns true if player dies */
	public bool CauseDamage(int amount)
	{
		this.HP -= amount;
		return (this.HP > 0);
	}
}
