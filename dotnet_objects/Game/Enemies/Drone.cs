using Godot;
using System;

namespace Game.Enemies
{
	public partial class Drone : Enemy
	{
		public override void _Process(double delta)
		{
			Position += Transform.Y * 50 * (float)delta;
		}
	}
}