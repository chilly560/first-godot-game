using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Enemies.EnemySpawning.SpawnerPhysics
{
    public class SpawnerPhysicsOverhaulers
    {
        private const int DEFAULT_SPEED = 40;

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