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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (physicsOverhauler is null)
			throw new NullReferenceException("Instance of ShotgunDrop does not have defined physics");

		physicsOverhauler.Invoke(this, delta);
		
		if (physicsModifier is not null)
			physicsModifier.Invoke(this);
    }
}
