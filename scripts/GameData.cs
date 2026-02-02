/*
Asset Credits:
- Music by veeru5656
- SFX by Various Artists from pixabay.com
- Sprites by Various Artists from itch.io
*/
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
	/// <summary>
	/// Maximum HP for the player.
	/// </summary>
	private const int MAX_HP = 100;
	/// <summary>
	/// Current HP for the player.
	/// </summary>
	private int HP;
	/// <summary>
	/// Current player score.
	/// </summary>
	public int Score {get; set;} = 0;
	/// <summary>
	/// Reference to the player node.
	/// </summary>
	public Player Player;
	/// <summary>
	/// Reference to the player's weapon scene node.
	/// </summary>
	public WeaponScene WeaponScene;
	/// <summary>
	/// Reference to the enemy parent class node.
	/// </summary>
	private Enemy enemyParentClassNode;
	/// <summary>
	/// Singleton instance of GameData.
	/// </summary>
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
	/// <summary>
	/// Current iteration of waves.
	/// </summary>
	public int WaveNumber { get; set; } = 1;
	/// <summary>
	/// Player healthbar is contained in gamedata as it is a child of the 'Game' scene,
	/// not the 'Player' scene. This is unique to this healthbar, other healthbars
	/// are children of the Enemy who's health they represent.
	/// </summary>
	private Healthbar playerHealthbar;
	/// <summary>
    /// Cache of alternative SpriteFrames for various game entities. Preloaded as part of GameData to improve performance at runtime.
    /// </summary>
	public TexturesCache TextureCache;
	//public AudioStreamPlayer2D backgroundMusicPlayer;
	/// <summary>
    /// Implements a cache of alternative SpriteFrames for various game entities. 
	/// 
	/// Implemented as readonly and accessed directly. 
	/// 
	/// Options:
	/// - WaveDrone: Left, Right, Center
	/// - Player: Left, Right, Center
	/// - Bogey: Left, Right, Center
    /// </summary>
	public class TexturesCache
    {
		public readonly WaveDroneSpriteCache WaveDrone = new WaveDroneSpriteCache();

		public readonly PlayerSpriteCache Player = new PlayerSpriteCache();

		public readonly BogeySpriteCache Bogey = new BogeySpriteCache();

        public class WaveDroneSpriteCache
		{
			public readonly Texture2D Left = GD.Load<Texture2D>("res://assets/Replacement_Enemy/enemy_2_r_l1.png");
			public readonly Texture2D Right = GD.Load<Texture2D>("res://assets/Replacement_Enemy/enemy_2_r_r2.png");
			public readonly Texture2D Center = GD.Load<Texture2D>("res://assets/Replacement_Enemy/enemy_2_r_m.png");
		}

		public class PlayerSpriteCache
		{
			public readonly Texture2D Left = GD.Load<Texture2D>("res://assets/Player/player_b_l1.png");
			public readonly Texture2D Right = GD.Load<Texture2D>("res://assets/Player/player_b_r2.png");
			public readonly Texture2D Center = GD.Load<Texture2D>("res://assets/Player/player_b_m.png");
		}

		public class BogeySpriteCache
        {
            public readonly Texture2D Left = GD.Load<Texture2D>("res://assets/Replacement_Enemy/enemy_1_r_l2.png");
			public readonly Texture2D Right = GD.Load<Texture2D>("res://assets/Replacement_Enemy/enemy_1_r_r1.png");
			public readonly Texture2D Center = GD.Load<Texture2D>("res://assets/Replacement_Enemy/enemy_1_r_m.png");
        }
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HP = MAX_HP;
		TextureCache = new TexturesCache();
		enemies = new Dictionary<int, Enemy>();
		playerHealthbar = GetNode<Healthbar>("../GameRoot/Camera2D/HUD/Healthbar");
		playerHealthbar.Setup(100);
		self = this;
		//backgroundMusicPlayer = GetNode<AudioStreamPlayer2D>("../GameRoot/Music");
		//backgroundMusicPlayer.Play();
	}
	/// <summary>
	/// Reset all game data fields when restarting the game.
	/// </summary>
	public void Flush()
	{
		HP = MAX_HP;
		enemies = new Dictionary<int, Enemy>();
		playerHealthbar = GetNode<Healthbar>("../GameRoot/Camera2D/HUD/Healthbar");
		playerHealthbar.Setup(100);
		Score = 0;
		WaveNumber = 0;
		Entities = 0;
		PauseSpawning = false;
		
		// Clear all event handler subscriptions to prevent old disposed nodes from being signaled
		//UpdateAmmoLabel;
		//UpdateScoreLabel = null;
		//RemoveEnemyXYFromFormation = null;
		//WaveDestroyed = null;
		//WaveBonus = null;
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
		{
			HP = MAX_HP;	
		}
		else 
		{
			HP += amount;
		}
		playerHealthbar.SetHealth(HP);
	}
	public Player GetPlayer()
	{
		return Player;
	}
	/// <summary>
	/// Get player's X position.
	/// </summary>
	/// <returns>float representing the player's X position</returns>
	public float GetPlayerX()
	{
		return Player.Position.X;
	}
	/// <summary>
	/// Get player's Y position.
	/// </summary>
	/// <returns>float representing the player's Y position</returns>
	public float GetPlayerY()
	{
		return Player.Position.Y;
	}
	public Vector2 GetPlayerGlobalPosition()
	{
		return Player.GlobalPosition;
	}
	/// <summary>
	/// Decrease player's HP by the given amount. Amount must be positive.
	/// </summary>
	/// <param name="amount"></param>
	/// <returns>True if player dies</returns>
	public bool CauseDamage(int amount)
	{
		if (amount < 0)
			throw new ArgumentException("Damage amount must be positive");
		
		if (HP - amount < 0)
			HP = 0;
		else
			HP -= amount;

		playerHealthbar.SetHealth(HP, true);

		return HP > 0;
	}
	/// <summary>
	/// Update the player's score by the given amount. + for adding, - for subtracting.
	/// </summary>
	/// <param name="plusMinus">Amount to add/sub</param>
	public void UpdateScore(int plusMinus)
	{
		Score += plusMinus;
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
		//if (activated)
		//	GD.Print("OnSignalWaveEnemyDestroyedEventHandler called by Wave.ActivateEnemy");
		//else
		//	GD.Print("OnSignalWaveEnemyDestroyedEventHandler invoked by SignalWaveEnemyDestroyed Emitted by Enemy.TakeDamage");
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
	/// 
	/// Secondary Signal is emitted to update the player hud.
	///
	/// Handler located in WaveSpawner.cs
	/// </summary>
	public void EmitWaveDestroyedEventHandlerSignal()
	{
		//GD.Print("Emitting WaveDestroyedEventHandler");
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
		if (self == null)
			throw new Exception("GameData instance is null");
		return self;
	}
}
