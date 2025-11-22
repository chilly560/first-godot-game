using Game.Enemies;
using Game.Weapons;
using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponScene : Marker2D
{

	private List<IWeapon> weaponCollection;

	private IWeapon currentWeapon;

	private IWeapon secondaryWeapon;

	/// <summary>
    /// Signals the Event-BUS (GameData.cs) to update the ammo count on the HUD
    /// </summary>
    /// <param name="plusMinus">Amount (+ - integer) to add to the display</param>
	[Signal]
	public delegate void UpdateAmmoHUDEventHandler(int plusMinus);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Node2D localRoot = GetParent<Node2D>().GetParent<Node2D>();

		if (localRoot is Player)
			currentWeapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, (Player)localRoot);

		else if (localRoot is Enemy)
			currentWeapon = WeaponFactory.CreateWeapon(WeaponType.Pistol, (Enemy)localRoot);

		else throw new ArgumentException("ERROR: PLAYER OBJ NOT OF TYPE PLAYER OR ENEMY");

		weaponCollection = [currentWeapon];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
    }

	public void SetWeapon(IWeapon weapon)
	{
		currentWeapon = weapon;
	}

	public void AddNewWeapon(IWeapon weapon)
	{
		weaponCollection.Add(weapon);
		if (secondaryWeapon is null)
			secondaryWeapon = weapon;
		else
        {
			secondaryWeapon.AddAmmo(((Weapon)weapon).GetAmmo());
        }

		EmitSignal(SignalName.UpdateAmmoHUD, ( (Weapon) weapon ).GetAmmo());
	}

	public void Shoot()
	{
		currentWeapon.Shoot(this.GlobalPosition);
	}

	public Weapon GetWeapon()
    {
        return (Weapon) currentWeapon;
    }

	public Weapon GetSecondaryWeapon()
    {
        return (Weapon) secondaryWeapon;
    }

	public void AltShoot()
	{
		if (secondaryWeapon is not null)
		{
			secondaryWeapon.Shoot(this.GlobalPosition);
			EmitSignal(SignalName.UpdateAmmoHUD, -1);
		}
	}
}
