using Game.Drops;
using Game.Enemies;
using Game.Weapons;
using Godot;
using System;

public partial class ShotgunDrop : Drop
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collectable = WeaponFactory.CreateWeapon(
			WeaponType.Shotgun,
			GetParent<GameRoot>()
		);
	}
}
