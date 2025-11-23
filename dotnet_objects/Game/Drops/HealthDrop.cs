using Game.Drops;
using Game.StatusModifier;
using Godot;
using System;

namespace Game.Drops 
{
    public partial class HealthDrop : Drop
    {
        [Export]
        private int HealAmount { get; set; } = 25;

        public override void _Ready()
        {
            // initialize the collectable (health modifier) and default physics behavior
            collectable = new Game.StatusModifier.HealthModifier(HealAmount);
            // ensure the collectable knows its parent if necessary
            collectable.SetParent(this);
        }

        public override void _Process(double delta)
        {
            if (physicsOverhauler is null)
                throw new NullReferenceException("Instance of HealthDrop does not have defined physics");

            physicsOverhauler.Invoke(this, delta);

            if (physicsModifier is not null)
                physicsModifier.Invoke(this);
        }

        public override void AddAttribute(Player player, ICollectable attribute)
        {
            if (attribute is IStatusModifier sm)
            {
                sm.ApplyModifier(player);
            }
            else
            {
                base.AddAttribute(player, attribute);
            }
        }
    }
}