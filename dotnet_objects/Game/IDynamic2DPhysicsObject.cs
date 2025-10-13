using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game
{
    public interface IDynamic2DPhysicsObject<T> where T : Node2D
    {
        public void SetPhysicsModifier(Action<T> del);

        public void SetPhysicsOverhauler(Action<T, double> del);
    }
}