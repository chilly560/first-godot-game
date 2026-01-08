using Godot;
using System;

namespace Game.Enemies
{	
	/// <summary>
    /// A very basic enemy that more or less does nothing
    /// </summary>
	public partial class Drone : Enemy
	{
		protected Sprite2D sprite;

        public override void _Ready()
        {
			dropChance = .1f;
			sprite = GetNode<Sprite2D>("./Sprite2D");
            base._Ready();
        }
		protected override void SetWorth()
		{
			worth = 5;
		}
	}
}