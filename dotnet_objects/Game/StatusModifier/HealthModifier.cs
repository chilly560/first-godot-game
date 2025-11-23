using System;
using Godot;

namespace Game.StatusModifier
{
    public class HealthModifier : IStatusModifier
    {
        private int amount;
        private Node2D parent;

        public HealthModifier(int amount)
        {
            this.amount = amount;
        }

        public int GetHealAmount()
        {
            return amount;
        }
        public void SetParent(Node2D parent)
        {
            this.parent = parent;
        }
    }
}
