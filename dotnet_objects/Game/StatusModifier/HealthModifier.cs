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

        public void ApplyModifier(Player player)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            player.Heal(amount);
        }

        public void SetParent(Node2D parent)
        {
            this.parent = parent;
        }
    }
}
