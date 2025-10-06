using Game.Enemies;
using Game.Weapons;
using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponScene : Marker2D
{

	private List<IWeapon> weaponCollection;

	private IWeapon currentWeapon;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Node2D localRoot = GetParent<Node2D>().GetParent<Node2D>();

		if (localRoot is Player)
			this.currentWeapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, (Player)localRoot);

		else if (localRoot is Enemy)
			this.currentWeapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, (Enemy)localRoot);

		else
			throw new ArgumentException("ERROR: PLAYER OBJ NOT OF TYPE PLAYER OR ENEMY");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetWeapon(IWeapon weapon)
	{
		this.currentWeapon = weapon;
	}

	public void AddNewWeapon(IWeapon weapon)
	{
		this.weaponCollection.Add(weapon);
	}

	public void Shoot()
	{
		this.currentWeapon.Shoot(this.GlobalPosition);
	}
}
