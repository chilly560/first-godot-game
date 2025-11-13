using System;
using System.Collections.Generic;
using Game;
using Game.Weapons;
using Godot;

public partial class Player : CharacterBody2D, ICollector
{
	[Export]
	private int Speed { get; set; } = 400;

	private GameData gameData;

	private bool isAlive;

	private Label hp;

	// Remove
	private PackedScene bulletScene;

	/// <summary>
    /// WeaponScene playerWeapon contains a List<IWeapon> weaponCollection
	/// responsible for storing the player's collection of weapons.
    /// </summary>
	private WeaponScene weaponInventory;

	private Timer autofireTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		weaponInventory = GetNode<WeaponScene>("AnimatedSprite2D/WeaponScene");
		gameData = GetNode<GameData>("%GameData");
		autofireTimer = GetNode<Timer>("./AutofireTimer");
		hp = GetNode<Label>("Camera2D/HUD/HPValue");
		isAlive = true;
		GD.Print("[LOG] Spawned Player");
		GD.Print(gameData.ToString());
		GD.Print("[LOG] Starting Autofire Timer");
		autofireTimer.WaitTime = 2;
		autofireTimer.Start();
	}

	/** Handles input from the player.
	*/
	public void GetInput()
	{
		if (Input.IsActionJustPressed("click"))
			weaponInventory.AltShoot();
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
		GD.Print(isAlive);
		int hp = gameData.GetHP();
		GD.Print(hp);
		if (!this.isAlive)
		{
			GD.Print("Player dead!");
			GetTree().ChangeSceneToFile("res://scenes/game_over.tscn");
		}
	}

	private void Autofire()
	{
		GD.Print("[LOG] Autofire Triggered");
		weaponInventory.Shoot();
		autofireTimer.Start();
	}

	public void Collect(ICollectable collectable)
	{
		if (collectable is IWeapon w)
			weaponInventory.AddNewWeapon(w);

		else throw new ArgumentException("ERROR: collectable IS NOT WEAPONSCENE");
	}

	public void OnAutofireTimerTimeout()
	{
		GD.Print("[LOG] Autofire");
		Autofire();
		autofireTimer.WaitTime = .05;
		autofireTimer.Start();
	}
}
