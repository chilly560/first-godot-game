using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game;
using Godot;

namespace Game.Enemies.EnemySpawning
{
    /// <summary>
    /// A base class for enemy spawners that implement dynamic 2D physics behavior.
    /// </summary>
    public abstract partial class Spawner : Area2D, IDynamic2DPhysicsObject<Spawner>
    {
        /// <summary>
        /// An optional physics modifier that is applied after the overhauler at runtime.
        /// </summary>
        private Action<Spawner> physicsModifier;
        /// <summary>
        /// The physics overhauler that defines this IDynamic2DPhysicsObject's physics behavior.
        /// </summary>
        private Action<Spawner, double> physicsOverhauler;

    	// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _PhysicsProcess(double delta)
		{
			if (physicsOverhauler != null)
			{
				physicsOverhauler.Invoke(this, delta);

				if (physicsModifier != null)
					physicsModifier.Invoke(this);
			}
		}
        /// <summary>
        /// Sets the physics modifier for this IDynamic2DPhysicsObject
        /// </summary>
        /// <param name="del">Delegate function</param>
        public void SetPhysicsModifier(Action<Spawner> del)
        {
            physicsModifier = del;
        }
        /// <summary>
        /// Sets the physics overhauler for this IDynamic2DPhysicsObject
        /// </summary>
        /// <param name="del">Delegate function</param>
        public void SetPhysicsOverhauler(Action<Spawner, double> del)
        {
            physicsOverhauler = del;
        }
        /// <summary>
        /// Spawns a bogey enemy at the specified global position.
        /// </summary>
        /// <param name="spawnerGlobalPosition"></param>
        public void SpawnBogey(Vector2 spawnerGlobalPosition)
        {
            GetParent().AddChild(
                EnemyFactory.CreateEnemy(
                    //EnemyClassification.DRONE,
                    EnemyClassification.BOGEY,
                    spawnerGlobalPosition
                )
            );
        }
    }
}