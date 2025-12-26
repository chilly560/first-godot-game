using Godot;
using System;
using System.Collections.Generic;
using Game.Enemies;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

/*
TODO: Fix Entity cap and spawn pausing.
*/

/// <summary>
/// Originally meant to store game data about the current scene (hence the name 'GameData'),
/// This class has been retrofitted to function as the main Event-BUS for the game.
/// </summary>
public partial class GameData : Node
{
	/// <summary>
	/// Maximum HP for the player.
	/// </summary>
	private const int MAX_HP = 100;

	/// <summary>
	/// Current HP for the player.
	/// </summary>
	private int HP;

	/// <summary>
	/// Reference to the player node.
	/// </summary>
	private Player player;

	/// <summary>
	/// Reference to the player's weapon scene node.
	/// </summary>
	private WeaponScene weaponScene;

	/// <summary>
	/// Reference to the enemy parent class node.
	/// </summary>
	private Enemy enemyParentClassNode;

	private static GameData self;

	/// <summary>
	/// Dictionary of active enemies in the scene.
	/// Key: enemy ID
	/// Value: enemy instance
	/// </summary>
	private Dictionary<int, Enemy> enemies;
	/// <summary>
	/// Number of active non-wave enemies.
	/// </summary>
	public int Entities { get; set; } = 0;
	/// <summary>
	/// Maximum number of non-wave enemies allowed to be spawned.
	/// </summary>
	public int EntityCap { get; private set; } = 8;
	/// <summary>
	/// Indicates whether enemy spawning is paused. Should only be invoked when a wave is cleared.
	/// </summary>
	public bool PauseSpawning { get; set; } = false;

	public int WaveNumber { get; set; } = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self = this;
		HP = MAX_HP;

		enemies = new Dictionary<int, Enemy>();
		player = GetNode<Player>("../Player");
		weaponScene = GetNode<WeaponScene>("../Player/AnimatedSprite2D/WeaponScene");
		weaponScene.UpdateAmmoHUD += OnUpdateHUDEventHandler;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Get the number of active enemies.
	/// </summary>
	/// <returns></returns>
	public int GetNumberOfEnemies()
	{
		return enemies.Count;
	}

	/// <summary>
	/// Add enemy to the list of active enemies.
	/// </summary>
	/// <param name="enemy"></param>
	public void AddEnemy(Enemy enemy)
	{
		enemies.Add(enemy.GetID(), enemy);
	}

	/// <summary>
	/// Remove enemy from the list of active enemies.
	/// </summary>
	/// <param name="enemyId"></param>
	public void RemoveEnemy(int enemyId)
	{
		enemies.Remove(enemyId);
	}

	/// <summary>
	/// Get player's current HP.
	/// </summary>
	/// <returns></returns>
	public int GetHP()
	{
		return HP;
	}

	/// <summary>
	/// Increase player's HP by the given amount. Amount must be positive.
	/// </summary>
	public void Heal(int amount)
	{
		if (amount <= 0)
			return;

		if (amount + HP > MAX_HP)
			HP = MAX_HP;

		else HP += amount;
	}

	/// <summary>
	/// Get player's position.
	/// </summary>
	/// <returns></returns>
	public Vector2 GetPlayerXY()
	{
		return player.Position;
	}

	/// <summary>
	/// Get player's X position.
	/// </summary>
	/// <returns></returns>
	public float GetPlayerX()
	{
		return player.Position.X;
	}

	/// <summary>
	/// Get player's Y position.
	/// </summary>
	/// <returns></returns>
	public float GetPlayerY()
	{
		return player.Position.Y;
	}

	public Vector2 GetPlayerPosition()
	{
		return player.Position;
	}

	public Vector2 GetPlayerGlobalPosition()
	{
		return player.GlobalPosition;
	}
	/// <summary>
	/// Decrease player's HP by the given amount. Amount must be positive.
	/// </summary>
	/// <param name="amount"></param>
	/// <returns>True if player dies</returns>
	public bool CauseDamage(int amount)
	{
		HP -= amount;
		return (HP > 0);
	}

	/// <summary>
	/// Signal for updating the UI counter for ammo. 
	/// 
	/// Target: ../scenes/Player/Camera2D/HUD/AmmoValue
	/// </summary>
	[Signal]
	public delegate void UpdateAmmoLabelEventHandler(int plusMinus);

	/// <summary>
	/// Signal handler for updating the UI counter for ammo.
	/// </summary>
	/// <param name="plusMinus"></param>
	public void OnUpdateHUDEventHandler(int plusMinus)
	{
		EmitSignal(SignalName.UpdateAmmoLabel, plusMinus);
	}

	/// <summary>
	/// Signal for updating the UI counter for score.
	/// </summary>
	/// <param name="plusMinus"></param>
	[Signal]
	public delegate void UpdateScoreLabelEventHandler(int plusMinus);

	/// <summary>
	/// Signal handler for updating the UI counter for score.
	/// </summary>
	/// <param name="plusMinus"></param>
	public void OnUpdateScoreEventHandler(int plusMinus)
	{
		EmitSignal(SignalName.UpdateScoreLabel, plusMinus);
	}
	/// <summary>
	/// Signal for informing a wave that an enemy in formation has been destroyed and must be 
	/// removed.
	/// </summary>
	/// <param name="X">'X' coord of the matrix</param>
	/// <param name="Y">'Y' coord of the matrix</param>
	[Signal]
	public delegate void RemoveEnemyXYFromFormationEventHandler(int X, int Y, bool activated = false);
	/// <summary>
	/// Signal Handler for informing the 'Wave' that an enemy in it's formation has been 
	/// destroyed.
	/// 
	/// Signals the Wave.cs class
	/// </summary>
	public void OnSignalWaveEnemyDestroyedEventHandler(int X, int Y, bool activated = false)
	{
		if (activated)
			GD.Print("OnSignalWaveEnemyDestroyedEventHandler called by Wave.ActivateEnemy");
		else
			GD.Print("OnSignalWaveEnemyDestroyedEventHandler invoked by SignalWaveEnemyDestroyed Emitted by Enemy.TakeDamage");
		EmitSignal(SignalName.RemoveEnemyXYFromFormation, X, Y, activated);
	}
	/// <summary>
	/// Signal to inform the spawner that the current wave has been destroyed. This will 
	/// tell the spawner to wait until any residual Bogey's are destroyed, before spawning the
	/// next wave.
	/// </summary>
	[Signal]
	public delegate void WaveDestroyedEventHandler();
	/// <summary>
	/// Emit the signal for informing the spawner that the current wave has been destroyed.
	/// 
	/// Implemented since 'Wave.cs' is not a godot node.
	/// </summary>
	public void EmitWaveDestroyedEventHandlerSignal()
	{
		GD.Print("Emitting WaveDestroyedEventHandler");
		EmitSignal(SignalName.WaveDestroyed);
	}
	/// <summary>
	/// Signal to award bonus points to the player for defeating a wave.
	/// </summary>
	/// <param name="bonus"></param>
	[Signal]
	public delegate void WaveBonusEventHandler(int bonus);
	/// <summary>
	/// Signal to instruct GameData
	/// </summary>
	/// <param name="bonus">The int representation of bonus points to be awarded</param>
	public void EmitWaveBonusEventHandlerSignal(int bonus)
	{
		EmitSignal(SignalName.WaveBonus, bonus);
	}
	/// <summary>
	/// Get the current instance of GameData.
	/// 
	/// Excluding the typical singleton pattern - this Node is set to 'autoload', therefore it's already
	/// instantiated and just needs to be accessed.
	/// </summary>
	/// <returns></returns>
	public static GameData Get()
	{
		return self;
	}
}
