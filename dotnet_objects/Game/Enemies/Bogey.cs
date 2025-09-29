using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Enemies
{
    public partial class Bogey : Enemy
    {
        public override void _Ready()
        {
            base._Ready();
            // Create a timer that ticks down every x seconds
            // when timer ends, the bogey moves to the player's x position and shooots
            // This will be done with a godot timer node.
        }
    }
}