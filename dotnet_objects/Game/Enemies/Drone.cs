using Godot;
using System;

namespace Game.Enemies
{	
	/// <summary>
    /// A very basic enemy that more or less does nothing
    /// </summary>
	public partial class Drone : Enemy
	{
		protected override void SetWorth()
		{
			worth = 5;
		}
	}
}