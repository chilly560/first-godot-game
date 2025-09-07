using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 400;

	private GameData gameData;

	private bool isAlive;

	private Label hp;

	private PackedScene bulletScene;

	private Marker2D bulletSpawnPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.bulletScene = (PackedScene)ResourceLoader.Load("res://scenes/Bullet.tscn");
		this.bulletSpawnPosition = GetNode<Marker2D>("AnimatedSprite2D/BulletSpawnPosition");
		this.gameData = GetNode<GameData>("%GameData");
		this.hp = GetNode<Label>("Camera2D/HUD/HPValue");
		this.isAlive = true;
		GD.Print("[LOG] Spawned Player");
		GD.Print(gameData.ToString());
	}

	private void Shoot()
	{
		Node2D bullet = (Node2D)bulletScene.Instantiate();
		bullet.GlobalPosition = bulletSpawnPosition.GlobalPosition;
		GetParent().AddChild(bullet);
	}

	public void GetInput()
	{	
		if (Input.IsActionJustPressed("click"))
			Shoot();
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
