using System;

namespace Game.Enemies
{
    public partial class WaveBogey : Bogey
    {
        private Godot.Timer waveBogeyTimer;

        public override void _Ready()
        {
            base._Ready();
            waveBogeyTimer = GetNode<Godot.Timer>("./WaveBogeyTimer");
            waveBogeyTimer.WaitTime = new Random().Next(0,6);
            waveBogeyTimer.Start();
        }
        public void OnWaveBogeyTimerTimeout()
        {
            Shoot();
            waveBogeyTimer.WaitTime = 5f;
            waveBogeyTimer.Start();
        }
    }
}