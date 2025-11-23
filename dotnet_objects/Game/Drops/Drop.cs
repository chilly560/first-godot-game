using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Weapons;
using Godot;
using System.ComponentModel;

namespace Game.Drops
{
    public abstract partial class Drop : Node2D, IDrop, IDynamic2DPhysicsObject<Drop>, ICollector
    {
        protected PackedScene node;

        protected ICollectable collectable;

        protected Action<Drop> physicsModifier;

        protected Action<Drop, double> physicsOverhauler;

        /// <summary>
        /// Signals the player to pick up this drop
        /// </summary>
        /// <param name="player"></param>
        /// <param name="attribute"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void AddAttribute(Player player, ICollectable attribute)
        {
            if (attribute is null)
                throw new ArgumentException("ERROR: ATTRIBUTE CANNOT BE NULL");
            else if (
                attribute is not IWeapon &&
                attribute is not IEnemy 
            ) throw new ArgumentException("ERROR: ATTRIBUTE NOT OF TYPE IWEAPON, IENEMY, OR ISTATUSMODIFIER");

            player.Collect(collectable);
            collectable.SetParent(player);
        }

        /// <summary>
        /// Event handler for body entering drop area.
        /// </summary>
        /// <param name="body"></param>
        public virtual void OnBodyEnteredDrop(CharacterBody2D body)
        {
            GD.Print("Collision with Drop and Player detected");
            if (body is Player player)
            {
                AddAttribute(player, collectable);
            }
            CallDeferred("queue_free");
        }

        public void SetPhysicsModifier(Action<Drop> del)
        {
            physicsModifier = del;
        }

        public void SetPhysicsOverhauler(Action<Drop, double> del)
        {
            physicsOverhauler = del;
        }

        public void SetCollectable(ICollectable c)
        {
            collectable = c;
        }
    }
}