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
        public static void DefaultPhysics(EnemySpawner2 spawner, double delta)
        {
            if(spawner.rightCollisionRay.IsColliding())
            {
                spawner.dir = -1;
            }
            else if(spawner.leftCollisionRay.IsColliding())
            {
                spawner.dir = 1;
            }

            spawner.Position += spawner.Transform.X * DEFAULT_SPEED * (float)delta * spawner.dir;
        }
    }
}