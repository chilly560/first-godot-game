using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Weapons;
using Game.StatusModifier;
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
                attribute is not IEnemy &&
                attribute is not HealthModifier
            ) throw new ArgumentException("ERROR: ATTRIBUTE NOT OF TYPE IWEAPON, IENEMY, OR ISTATUSMODIFIER");

            player.Collect(collectable);
            collectable.SetParent(player);
        }

        public override void _Process(double delta)
        {
            if (physicsOverhauler is null)
                throw new NullReferenceException("Instance of ShotgunDrop does not have defined physics");

            physicsOverhauler.Invoke(this, delta);
            
            if (physicsModifier is not null)
                physicsModifier.Invoke(this);
        }

        /// <summary>
        /// Event handler for body entering drop area.
        /// </summary>
        /// <param name="body"></param>
        public virtual void OnBodyEnteredDrop(Node2D node)
        {
            if (node is Player player)
            {
                AddAttribute(player, collectable);
            }
            
            if (node is not OneWayBlocker)
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