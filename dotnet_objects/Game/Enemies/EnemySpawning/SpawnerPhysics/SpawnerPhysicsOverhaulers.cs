using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Enemies.EnemySpawning.SpawnerPhysics
{
    /// <summary>
    /// A collection of physics overhaulers defining physics behavior for Spawner objects.
    /// 
    /// Default behavior is null, in which a Spawner will not move.
    /// </summary>
    public class SpawnerPhysicsOverhaulers
    {
        /// <summary>
        /// Movement speed
        /// </summary>
        private const int DEFAULT_SPEED = 40;
        /// <summary>
        /// Default physics behavior for mothership-like spawners (moves left to right).
        /// Originally for the EnemySpawner2 class used in testing. I've moved it here in to use with 
        /// the mothership idea later.
        /// </summary>
        /// <param name="spawner"></param>
        /// <param name="delta"></param>
        public static void MothershipDefaultPhyysics(Spawner spawner, double delta)
        {
            if (spawner is EnemySpawner2 enemySpawner)
            {
                if (enemySpawner.rightCollisionRay.IsColliding())
                {
                    enemySpawner.dir = -1;
                }
                else if (enemySpawner.leftCollisionRay.IsColliding())
                {
                    enemySpawner.dir = 1;
                }

                enemySpawner.Position += enemySpawner.Transform.X * DEFAULT_SPEED * (float)delta * enemySpawner.dir;
            }
        }
    }
}