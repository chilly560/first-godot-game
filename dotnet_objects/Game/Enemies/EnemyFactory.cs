using Godot;
using System;
using System.Collections.Generic;

namespace Game.Enemies
{
    // Factory class to create enemies
    public static class EnemyFactory
    {
        private static int nextId = 1; // Static counter for unique IDs

        private static PackedScene droneScene = GD.Load<PackedScene>("res://scenes/drone.tscn");
        private static PackedScene bogeyScene = GD.Load<PackedScene>("res://scenes/Bogey.tscn");

        private static Enemy defaultSetup(Enemy enemy, Vector2 position)
        {
            enemy.SetID(nextId++);
            enemy.Position = position;
            return enemy;
        } 
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