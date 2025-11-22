using Godot;
using System;
using System.Collections.Generic;
using Game.Enemies;

/// <summary>
/// Originally meant to store game data about the current scene (hence the name 'GameData'),
/// This class has been retrofitted to function as the main Event-BUS for the game.
/// </summary>
public partial class GameData : Node
{
	
	private int HP;

	private Player player;

	private WeaponScene weaponScene;

	private Dictionary<int, Enemy> enemies;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.HP = 100;
		this.enemies = new Dictionary<int, Enemy>();
		this.player = GetNode<Player>("../Player");
		weaponScene = GetNode<WeaponScene>("../Player/AnimatedSprite2D/WeaponScene");
		weaponScene.UpdateAmmoHUD += OnUpdateHUDEventHandler;
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
	
	public Vector2 GetPlayerXY()
	{
		return player.Position;
	}

	public float GetPlayerX()
	{
		return player.Position.X;
	}

	public float GetPlayerY()
	{
		return player.Position.Y;
	}

	/** Returns true if player dies */
	public bool CauseDamage(int amount)
	{
		this.HP -= amount;
		return (this.HP > 0);
	}

	/// <summary>
    /// Signal for updating the UI counter for ammo. 
	/// 
	/// Target: ../scenes/Player/Camera2D/HUD/AmmoValue
    /// </summary>
	[Signal]
	public delegate void UpdateAmmoLabelEventHandler(int plusMinus);

	public void OnUpdateHUDEventHandler(int plusMinus)
    {
		GD.Print("UPDATEAMMOSIGNAL RECEIVED");
        EmitSignal(SignalName.UpdateAmmoLabel, plusMinus);
    }
}
