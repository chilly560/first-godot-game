using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Enemies
{
    public interface IEnemy
    {
        public void TakeDamage(int amount);

        public int GetID();

        public void OnBodyEnteredEnemy(Node body);
    }
}