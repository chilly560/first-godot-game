using Godot;
using System;
using System.Collections.Generic;

namespace Game.Enemies
{
    // Factory class to create enemies
    public static class EnemyFactory
    {
        private static int nextId = 1; // Static counter for unique IDs

        private static PackedScene droneScene = GD.Load<PackedScene>("res://scenes/Drone.tscn");

        public static Enemy CreateEnemy(EnemyClassification enemyType, Vector2 position)
        {
            switch (enemyType)
            {
                case EnemyClassification.DRONE:
                    Drone enemy = (Drone)droneScene.Instantiate();
                    enemy.SetID(nextId++);
                    return enemy;
                default:
                    throw new ArgumentException("Unknown enemy type");
            }
        }
    }
}