using System;
using Godot;

namespace Game.Enemies
{
    public partial class WaveBogey : Bogey, Bobber
    {
        private Timer waveBogeyTimer;
        public float BobDelta { get ; set ; } = 0;
        public bool Down { get ; set ; } = false;
        public Vector2 PreviousPosition { get; set; }
        public override void _Ready()
        {
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
    }
}