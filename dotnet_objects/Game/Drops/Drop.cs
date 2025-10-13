using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Weapons;
using Godot;
using Game.StatusModifier;

namespace Game.Drops
{
    public abstract partial class Drop : Node2D, IDrop
    {
        protected ICollectable collectable;

        /// <summary>
        /// Signals the player to pick up this drop
        /// </summary>
        /// <param name="player"></param>
        /// <param name="attribute"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void AddAttribute(Player player, Node2D attribute)
        {
            if (attribute is null)
                throw new ArgumentException("ERROR: ATTRIBUTE CANNOT BE NULL");
            else if (
                attribute is not IWeapon ||
                attribute is not IEnemy ||
                attribute is not IStatusModifier
            )
                throw new ArgumentException("ERROR: ATTRIBUTE NOT OF TYPE IWEAPON, IENEMY, OR ISTATUSMODIFIER");

            player.Collect(collectable);
            collectable.SetParent(player);
        }

        /// <summary>
        /// Event handler for body entering drop area.
        /// </summary>
        /// <param name="body"></param>
        public virtual void OnBodyEnteredDrop(CharacterBody2D body)
        {
            if (body is Player player)
            {
                AddAttribute(player, (Node2D)collectable);
            }
            QueueFree();
        }
    }
}