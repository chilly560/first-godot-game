using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game
{
    public interface ICollectable
    {
        public void SetParent(Node2D parent);
    }
}