using Game.Weapons;
using Godot;
using System;

public partial class WeaponScene : Marker2D
{

	private IWeapon currentWeapon;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ // Initialize with a default weapon
		Player player = GetParent<AnimatedSprite2D>().GetParent<Player>();
		//this.currentWeapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, player);
		this.currentWeapon = WeaponFactory.CreateWeapon(WeaponType.Shotgun, player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetWeapon(IWeapon weapon)
	{
		this.currentWeapon = weapon;
	}

	public void Shoot()
	{
		this.currentWeapon.Shoot(this.GlobalPosition);
	}
}
