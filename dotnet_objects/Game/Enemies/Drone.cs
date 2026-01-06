using Godot;
using System;

namespace Game.Enemies
{	
	/// <summary>
    /// A very basic enemy that more or less does nothing
    /// </summary>
	public partial class Drone : Enemy
	{

        public override void _Ready()
        {
			dropChance = .1f;
            base._Ready();
        }
		protected override void SetWorth()
		{
			worth = 5;
		}
	}
}