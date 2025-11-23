using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Drops
{
    public abstract class AbstractDropFactory
    {
        public const int WEAPON = 0;
        public const int STATUS = 1;

        protected int factoryType;

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

        public int GetFactoryType()
        {
            return factoryType;
        }

        protected void SetFactoryType(int type)
        {
            if (type != WEAPON && type != STATUS)
                throw new ArgumentException("Invalid factory type constant");
        }
        
        public abstract Drop MakeDrop(int type);
    }
}