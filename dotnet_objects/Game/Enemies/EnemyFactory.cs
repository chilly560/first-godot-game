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
        /// Special type of drone for use in waves.
        /// </summary>
        private static PackedScene waveDroneScene = GD.Load<PackedScene>("res://scenes/wavedrone.tscn");
        /// <summary>
        /// A collection of physics modifiers for enemies
        /// </summary>
        private class EnemyPhysicsModifiers
        {
        }
        /// <summary>
        /// A collection of physics overhauls for enemies
        /// </summary>
        private class EnemyPhysicsOverhaulers
        {
        }
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
                case EnemyClassification.WAVE_DRONE:
                    return defaultSetup((WaveDrone)waveDroneScene.Instantiate(), position);
                default:
                    throw new ArgumentException("Unknown enemy type");
            }
        }
        /// <summary>
        /// Creates an enemy of the specified type at the given position with optional physics overhauls and modifiers
        /// </summary>
        /// <param name="enemyType">An EnemyClasssificiation representing the type of Enemy to create</param>
        /// <param name="position">A Vector2 (typically a Position or GlobalPosition) representing 
        ///     the location this enemy should spawn in
        /// </param>
        /// <param name="physicsOverhauler">Delegate method containing main physics behavior</param>
        /// <param name="physicsModifier">Delegate method containing secondary physics behavior</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Enemy CreateEnemy(EnemyClassification enemyType, Vector2 position, Action<Enemy, double> physicsOverhauler = null, Action<Enemy> physicsModifier = null)
        {
            switch (enemyType)
            {
                case EnemyClassification.DRONE:
                    Enemy e = defaultSetup((Drone)droneScene.Instantiate(), position);
                    e.SetPhysicsOverhauler(physicsOverhauler);
                    e.SetPhysicsModifier(physicsModifier);
                    return e;
                case EnemyClassification.BOGEY:
                    Enemy e2 = defaultSetup((Bogey)bogeyScene.Instantiate(), position);
                    e2.SetPhysicsOverhauler(physicsOverhauler);
                    e2.SetPhysicsModifier(physicsModifier);
                    return e2;
                default:
                    throw new ArgumentException("Unknown enemy type"); 
            }
        }
    }
}