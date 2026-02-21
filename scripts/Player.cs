using System;
using System.Collections.Generic;
using Game;
using Game.Drops;
using Game.StatusModifier;
using Game.Weapons;
using Godot;

public partial class Player : CharacterBody2D, ICollector
{
	[Export]
	private int Speed { get; set; } = 600;
	private GameData gameData;
	private bool isAlive;
	private Label hp, secondaryAmmo, score, wave;
	/// <summary>
    /// WeaponScene playerWeapon contains a List<IWeapon> weaponCollection
	/// responsible for storing the player's collection of weapons.
    /// </summary>
	private WeaponScene weaponScene;
	private Timer autofireTimer;
	private Sprite2D sprite;
	private Timer showHealthbarTimer;
	private Timer deathDelayTimer;
	private AnimatedSprite2D deathExplosion;
	private AnimatedSprite2D trail;
	// Called when the node enters the scene tree for the first time.
	private AudioStreamPlayer2D shootSound, powerupSound, deathSound;
	/// <summary>
	/// Maps touch index to current position for handling multi-touch input. 
	/// 
	/// Used to determine if a tap or drag occurred based on the movement and duration of each touch.
	/// </summary>
	private Dictionary<long, Vector2> activeTouches = new Dictionary<long, Vector2>(); // Maps touch index to current position
	/// <summary>
	/// Pixels a finger can move and still count as a tap
	/// </summary>
	private const float TAP_MAX_MOVEMENT = 10f; 
	/// <summary>
	/// Maximum duration for a touch to be considered a tap.
	/// </summary>
	private const float TAP_MAX_DURATION = 0.2f; 
	/// <summary>
	/// Maps touch index to the starting position and time of the touch for tap detection.
	/// </summary>
	private Dictionary<long, (Vector2 startPos, double startTime)> touchStartInfo = new Dictionary<long, (Vector2 startPos, double startTime)>();
	public override void _Ready()
	{
		weaponScene = GetNode<WeaponScene>("Sprite2D/WeaponScene");
		gameData = GameData.Get();
		gameData.Player = this;
		trail = GetNode<AnimatedSprite2D>("./AnimatedTrail2D");
		autofireTimer = GetNode<Timer>("./AutofireTimer");
		hp = GetNode<Label>("../Camera2D/HUD/HPValue");
		secondaryAmmo = GetNode<Label>("../Camera2D/HUD/AmmoValue");
		score = GetNode<Label>("../Camera2D/HUD/ScoreValue");
		wave = GetNode<Label>("../Camera2D/HUD/WaveValue");
		sprite = GetNode<Sprite2D>("./Sprite2D");
		deathDelayTimer = GetNode<Timer>("./PlayerDeathDelayTimer");
		deathDelayTimer.WaitTime = 2.5;
		deathDelayTimer.OneShot = true;
		deathExplosion = GetNode<AnimatedSprite2D>("./ExplodeAnimation");
		deathExplosion.Visible = false;
		isAlive = true;
		autofireTimer.WaitTime = 2;
		autofireTimer.Start();
		gameData.WaveDestroyed += UpdateWaveLabel;
		shootSound = GetNode<AudioStreamPlayer2D>("./Shoot");
		powerupSound = GetNode<AudioStreamPlayer2D>("./PowerUp");
		deathSound = GetNode<AudioStreamPlayer2D>("./Explosion");
		gameData.UpdateAmmoLabel += OnUpdateAmmoLabel;
		gameData.UpdateScoreLabel += OnUpdateScoreLabel;
		gameData.WaveBonus += OnUpdateScoreLabel;
	}
    public override void _ExitTree()
    {
        gameData.UpdateAmmoLabel -= OnUpdateAmmoLabel;
        gameData.UpdateScoreLabel -= OnUpdateScoreLabel;
        gameData.WaveBonus -= OnUpdateScoreLabel;
        gameData.WaveDestroyed -= UpdateWaveLabel;
        base._ExitTree();
    }
	
	/// <summary>
	/// Override of the _Input method to handle touch input for both taps and drags.
	/// 
	/// Detects taps and drags based on the movement of the input and the duration of the input (See TAP_MAX_MOVEMENT and TAP_MAX_DURATION). 
	/// If an input is less than 10 pixels across and persists for less than .2 seconds, it is a tap and the weapon is fired.
	/// 
	/// Otherwise, the player's position is updated based on the drag input.
	/// </summary>
	/// <param name="event">InputEvent for evaluation. Only processed if it is alos an InputEventScreenTouch</param>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Pressed)
			{
				activeTouches[touchEvent.Index] = touchEvent.Position;
				touchStartInfo[touchEvent.Index] = (touchEvent.Position, Time.GetTicksMsec() / 1000.0);
			}
			else
			{
				if (touchStartInfo.TryGetValue(touchEvent.Index, out var info))
				{
					double duration = (Time.GetTicksMsec() / 1000.0) - info.startTime;
					float distance = touchEvent.Position.DistanceTo(info.startPos);

					if (duration <= TAP_MAX_DURATION && distance <= TAP_MAX_MOVEMENT)
					{
						OnTap();
					}

					touchStartInfo.Remove(touchEvent.Index);
				}

				activeTouches.Remove(touchEvent.Index);
			}
		}
		else if (@event is InputEventScreenDrag dragEvent)
		{
			if (activeTouches.ContainsKey(dragEvent.Index))
			{
				activeTouches[dragEvent.Index] = dragEvent.Position;
			}

			OnDrag(dragEvent.Relative);
		}

		base._Input(@event);
	}

	private void OnDrag(Vector2 delta)
	{
		Position += delta * .6f;

		if (delta.X < 0)
			sprite.Texture = gameData.TextureCache.Player.Left;
		else if (delta.X > 0)
			sprite.Texture = gameData.TextureCache.Player.Right;
		else
			sprite.Texture = gameData.TextureCache.Player.Center;
	}

	private void OnTap()
	{
		weaponScene.AltShoot();
	}
	/** Handles input from the player.
	*/
	public void GetInput()
	{
		if (Input.IsActionJustPressed("click"))
		{
			weaponScene.AltShoot();
		}
		
		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (inputDirection.X < 0)
		{
			sprite.Texture = gameData.TextureCache.Player.Left;
		}
		else if (inputDirection.X > 0)
		{
			sprite.Texture = gameData.TextureCache.Player.Right;
		}
		else if (inputDirection.X == 0)
		{
			sprite.Texture = gameData.TextureCache.Player.Center;
		} else {
			throw new Exception("Unreachable code in Player.GetInput()");
		}
		Velocity = inputDirection * Speed;
	}
	public override void _PhysicsProcess(double delta)
	{
		//GetInput();
		MoveAndSlide();
	}
	/** Applies damage to the player and checks if they are still alive.
	 * If the player dies, it changes the scene to the game over screen.

	 @param damage The amount of damage to apply to the player.
	 */
	public void TakeDamage(int damage)
	{
		isAlive = gameData.CauseDamage(damage);
		this.hp.Text = gameData.GetHP().ToString();
		int hp = gameData.GetHP();
		if (!this.isAlive)
		{
			autofireTimer.Stop();
			SetPhysicsProcess(false);
			sprite.Visible = false;
			trail.Visible = false;
			if (deathDelayTimer.IsStopped())
			{
				deathExplosion.Visible = true;
				deathSound.Play();
				deathExplosion.Play();
				deathDelayTimer.Start();
			}
		}
	}
	/// <summary>
	/// Heal the player by the specified amount and update HUD.
	/// </summary>
	public void Heal(int amount)
	{
		if (amount <= 0)
			return;
			
		gameData.Heal(amount);

		hp.Text = gameData.GetHP().ToString();
		isAlive = true;
	}
	/// <summary>
    /// Required to change scene after player death to avoid undefined/undesired behavior.
    /// </summary>
	private void DeferredToGameOverScene()
	{
		GetTree().ChangeSceneToFile("res://scenes/game_over.tscn");
	}
	private void Autofire()
	{
		weaponScene.Shoot();
		shootSound.Play();
		autofireTimer.Start();
	}
	public void Collect(ICollectable collectable)
	{
		if (collectable is IWeapon w)
			weaponScene.AddNewWeapon(w);
			
		else if (collectable is IStatusModifier sm)
        {
            if (sm is HealthModifier hm)
            {
                Heal(hm.GetHealAmount());
            }
        }
		else throw new ArgumentException("ERROR: collectable IS NOT WEAPONSCENE");

		powerupSound.Play();
	}
	public void OnAutofireTimerTimeout()
	{
		Autofire();
		autofireTimer.WaitTime = .25;
		autofireTimer.Start();
	}
	public void OnUpdateAmmoLabel(int plusMinus)
    {
		int ammoVal = weaponScene.GetSecondaryWeapon().GetAmmo();
		int maxAmmo = weaponScene.GetSecondaryWeapon().GetMaxAmmo();
		int newTextValue = ammoVal;
		if (newTextValue > maxAmmo)
        	secondaryAmmo.Text = maxAmmo.ToString();
		else if (newTextValue < 0)
			secondaryAmmo.Text = "0";
		else secondaryAmmo.Text = newTextValue.ToString();
    }
	public void OnUpdateScoreLabel(int plusMinus)
    {
        int scoreVal = int.Parse(score.Text);
		scoreVal += plusMinus;
		score.Text = scoreVal.ToString();
		gameData.UpdateScore(plusMinus);
    }	
	public void OnAnimationFinished()
	{
		deathExplosion.Visible = false;
	}
	public int GetHP()
	{
		return gameData.GetHP();
	}
	/// <summary>
	/// Returns player score as a String.
	/// </summary>
	/// <returns>A String instead of an int (for display purposes)</returns>
	public String GetScore()
	{
		return score.Text;
	}
	private void UpdateWaveLabel()
	{
		int i;
		if (int.TryParse(wave.Text, out i))
		{
			wave.Text = (++i).ToString();
		}
	}
	public void OnPlayerDeathDelayTimerTimeout()
	{
		CallDeferred(nameof(DeferredToGameOverScene));
	}
}
