using Godot;
using System;

public partial class WeaponScene : Marker2D
{

	private IWeapon currentWeapon;

	[Export]
	public IWeapon CurrentWeapon;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Shoot()
	{
		if (currentWeapon != null)
			currentWeapon.Shoot();
		else
			GD.PrintErr("No weapon equipped");
	}
}
