using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Drops
{
    public interface IDrop
    {
        public void AddAttribute(Player player, Node2D attribute);
    }
}