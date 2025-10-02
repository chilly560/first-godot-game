using Game.Weapons;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	private int Speed { get; set; } = 400;

	private GameData gameData;

	private bool isAlive;

	private Label hp;

	// Remove
	private PackedScene bulletScene;

	private WeaponScene playerWeapon;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//this.bulletScene = (PackedScene)ResourceLoader.Lad("res://scenes/Bullet.tscn");
		WeaponFactory.SetPlayer(this);
		this.playerWeapon = GetNode<WeaponScene>("AnimatedSprite2D/WeaponScene");
		this.gameData = GetNode<GameData>("%GameData");
		this.hp = GetNode<Label>("Camera2D/HUD/HPValue");
		this.isAlive = true;
		GD.Print("[LOG] Spawned Player");
		GD.Print(gameData.ToString());
	}

	/** Spawns a bullet at the bullet spawn position and adds it to the scene tree.

	Position applied based on BulletSpawnPosition (Marker2D node) in player scene.
	*/
	private void Shoot()
	{
		//Node2D bullet = (Node2D)bulletScene.Instantiate();
		//bullet.GlobalPosition = WeaponPosition.GlobalPosition;
		//GetParent().AddChild(bullet);
	}

	/** Handles input from the player.
	*/
	public void GetInput()
	{
		if (Input.IsActionJustPressed("click"))
			playerWeapon.Shoot();
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
}
