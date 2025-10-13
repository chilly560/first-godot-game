using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Drops
{
    public abstract class AbstractDropFactory
    {
        protected static class DropPhysicsModifiers{}

        protected static class DropPhysicsOverhaulers
        {
            private const int DEFAULT_SPEED = 300;

            public static void DefaultPhysics(Drop d, double delta)
            {
                d.Position += 1 * d.Transform.Y * DEFAULT_SPEED * (float)delta;
            }
        }

        private const int DEFAULT_SPEED = 300;

        public abstract Drop MakeDrop(int type);
    }
}