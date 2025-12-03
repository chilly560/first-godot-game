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
	private int Speed { get; set; } = 400;

	private GameData gameData;

	private bool isAlive;

	private Label hp, secondaryAmmo, score;

	// Remove
	private PackedScene bulletScene;

	/// <summary>
    /// WeaponScene playerWeapon contains a List<IWeapon> weaponCollection
	/// responsible for storing the player's collection of weapons.
    /// </summary>
	private WeaponScene weaponScene;

	private Timer autofireTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		weaponScene = GetNode<WeaponScene>("AnimatedSprite2D/WeaponScene");
		gameData = GetNode<GameData>("%GameData");
		gameData.UpdateAmmoLabel += OnUpdateAmmoLabel;
		gameData.UpdateScoreLabel += OnUpdateScoreLabel;
		autofireTimer = GetNode<Timer>("./AutofireTimer");
		hp = GetNode<Label>("Camera2D/HUD/HPValue");
		secondaryAmmo = GetNode<Label>("Camera2D/HUD/AmmoValue");
		score = GetNode<Label>("Camera2D/HUD/ScoreValue");
		isAlive = true;
		autofireTimer.WaitTime = 2;
		autofireTimer.Start();
	}

	/** Handles input from the player.
	*/
	public void GetInput()
	{
		if (Input.IsActionJustPressed("click"))
		{
			weaponScene.AltShoot();
		} 
		else
		{
			Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
			Velocity = inputDirection * Speed;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
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
			CallDeferred(nameof(DeferredToGameOverScene));
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

		this.hp.Text = gameData.GetHP().ToString();
		this.isAlive = true;
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
    }	
}
