using Game.Drops;
using Game.Weapons;

public partial class ShotgunDrop : Drop
{
	public override void _Ready()
	{
		collectable = WeaponFactory.CreateWeapon(
			WeaponType.Shotgun,
			GetParent<GameRoot>()
		);
	}
}
