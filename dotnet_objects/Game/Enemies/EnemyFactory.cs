using Godot;
using System;
using System.Collections.Generic;

namespace Game.Enemies
{
    // Factory class to create enemies
    public static class EnemyFactory
    {
        /// <summary>
        /// Next unique enemy ID
        /// </summary>
        private static int nextId = 1; // Static counter for unique IDs
        /// <summary>
        /// PackedScene for Drone enemy
        /// </summary>
        private static PackedScene droneScene = GD.Load<PackedScene>("res://scenes/drone.tscn");
        /// <summary>
        /// PackedScene for Bogey enemy
        /// </summary>
        private static PackedScene bogeyScene = GD.Load<PackedScene>("res://scenes/Bogey.tscn");
        /// <summary>
        /// Internal helper method to set ID and position for a newly created enemy
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Enemy defaultSetup(Enemy enemy, Vector2 position)
        {
            enemy.SetID(nextId++);
            enemy.Position = position;
            return enemy;
        } 
        /// <summary>
        /// Creates an enemy of the specified type at the given position
        /// </summary>
        /// <param name="enemyType">An EnemyClasssificiation representing the type of Enemy to create</param>
        /// <param name="position">A Vector2 (typically a Position or GlobalPosition) representing 
        ///     the location this enemy should spawn in
        /// </param>
        /// <returns>A new instance of an Enemy</returns>
        /// <exception cref="ArgumentException">If the EnemyClassification is invalid</exception>
        public static Enemy CreateEnemy(EnemyClassification enemyType, Vector2 position)
        {
            switch (enemyType)
            {
                case EnemyClassification.DRONE: 
                    return defaultSetup((Drone)droneScene.Instantiate(), position);
                case EnemyClassification.BOGEY:
                    return defaultSetup((Bogey)bogeyScene.Instantiate(), position);
                default:
                    throw new ArgumentException("Unknown enemy type");
            }
        }
    }
}