using System;
using Godot;

namespace Game.Enemies
{
    public partial class WaveBogey : Bogey, Bobber
    {
        private Healthbar healthbar;
        private Timer waveBogeyTimer;
        public float BobDelta { get ; set ; } = 0;
        public bool Down { get ; set ; } = false;
        public Vector2 PreviousPosition { get; set; }

        private Timer showHealthbarTimer;
        public override void _Ready()
        {
            healthbar = GetNode<Healthbar>("./Healthbar");
            healthbar.SetHealth(100);
            healthbar.Visible = false; // Hide healthbar by default
            showHealthbarTimer = GetNode<Timer>("./ShowWavebogeyHealthbarTimer");
            showHealthbarTimer.WaitTime = 2.0;
            showHealthbarTimer.OneShot = true;
            base._Ready();
            waveBogeyTimer = GetNode<Timer>("./WaveBogeyTimer");
            waveBogeyTimer.WaitTime = new Random().Next(1,6);
            waveBogeyTimer.Start();
            
        }
        public void OnWaveBogeyTimerTimeout()
        {
            Shoot();
            waveBogeyTimer.WaitTime = 5f;
            waveBogeyTimer.Start();
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

        private void OnWavebogeyShowHealthbarTimerTimeout()
        {
            healthbar.Visible = false;
        }
    }
}