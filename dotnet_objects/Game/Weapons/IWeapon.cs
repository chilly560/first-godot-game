using Godot;
using System;

namespace Game.Weapons
{
	/// <summary>
	/// Represents a weapon in the game, providing methods for managing damage, speed, ammo, and shooting behavior.
	/// </summary>
	public interface IWeapon
	{
		/// <summary>
		/// Adds ammo to this weapon.
		/// </summary>
		/// <param name="ammo">The amount of ammo to add.</param>
		public void AddAmmo(int ammo);

		/// <summary>
		/// Reloads the weapon, refilling its ammo or resetting its state as needed.
		/// </summary>
		public void Reload();

		/// <summary>
		/// Fires the weapon, consuming ammo and creating a bullet.
		/// </summary>
		public void Shoot(Vector2 weaponPosition);

		public void SetParent(Node2D parent);
	}
}
