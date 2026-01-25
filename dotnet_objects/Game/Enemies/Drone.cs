using Godot;
using System;

namespace Game.Enemies
{	

	/// <summary>
    /// A very basic enemy that more or less does nothing
    /// </summary>
	public partial class Drone : Enemy
	{
		private Healthbar healthbar;

		private Timer showHealthbarTimer;
		
        public override void _Ready()
        {
			dropChance = .1f;
			sprite = GetNode<Sprite2D>("./Sprite2D");
			healthbar = GetNode<Healthbar>("./Healthbar");
			healthbar.SetHealth(100);
			healthbar.Visible = false; // Hide healthbar by default
			showHealthbarTimer = GetNode<Timer>("./ShowHealthbarTimer");
			showHealthbarTimer.WaitTime = 2.0;
			showHealthbarTimer.OneShot = true;
            base._Ready();
        }
		protected override void SetWorth()
		{
			worth = 5;
		}

		public override void TakeDamage(int amount)
		{
			healthbar.SetHealth(hp - amount, true);
			healthbar.Visible = true; // Show healthbar when damage taken
			
			// Restart timer to hide after 2 seconds
			if (!showHealthbarTimer.IsStopped())
			{
				showHealthbarTimer.Stop();
			}
			showHealthbarTimer.Start();
			
			base.TakeDamage(amount);
		}

		public void OnShowDroneHealthbarTimerTimeout()
		{
			healthbar.Visible = false;
		}
	}
}