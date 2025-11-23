using Game.Drops;
using Game.StatusModifier;
using Godot;
using System;

namespace Game.Drops 
{
    public partial class HealthDrop : Drop
    {
        [Export]
        public int HealAmount = 25;

        public override void _Ready()
        {
            // TODO: Eventually refactor to utilize factory if more status 
            // effects are added
            collectable = new HealthModifier(HealAmount);
            collectable.SetParent(GetParent<GameRoot>());
        }
    }
}