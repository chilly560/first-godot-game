using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;

namespace Game.Enemies
{
    /// <summary>
    /// Class representing an enemy wave formation
    /// </summary>
    public class Wave
    {
        /// <summary>
        /// Class representing a 2D matrix of enemies for enemy wave formations.
        /// 
        /// Enscapsulates logic for managing enemy formations in a grid layout.
        /// </summary>
        private class EnemyMatrix
        {
            /// <summary>
            /// 2D array representing the enemy formation.
            /// </summary>
            private Enemy[,] matrix;
            /// <summary>
            /// Constructor initializes the enemy matrix.
            /// </summary>
            public EnemyMatrix()
            {
                matrix = new Enemy[6,13];
            }       
            /// <summary>
            /// Constructor with predefined matrix.
            /// </summary>
            /// <param name="matrix"></param>
            public EnemyMatrix(Enemy[,] matrix)
            {
                this.matrix = matrix;
            }
            /// <summary>
            /// Sets the enemy at the specified coordinates.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="enemy"></param>
            public void PopulateMatrix(int x, int y, Enemy enemy)
            {
                if (x >= 0 && x < 6 && y >= 0 && y < 13)
                {
                    matrix[x, y] = enemy;
                }
            }  
        }
        /// <summary>
        /// Builder class for constructing EnemyMatrix instances with predefined patterns.
        /// </summary>
        private class EnemyMatrixBuilder
        {
            /// <summary>
            /// Builds a default enemy matrix formation with all positions filled with Drones.
            /// </summary>
            /// <returns></returns>
            public EnemyMatrix BuildDefaultMatrix()
            {
                Enemy[,] defaultMatrix = new Enemy[6, 13];
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        // TODO: Tweak actual Vector2 positions later
                        defaultMatrix[i, j] = EnemyFactory.CreateEnemy(EnemyClassification.DRONE, new Godot.Vector2(i * 10, j * 10));
                    }
                }
                return new EnemyMatrix(defaultMatrix);
            }
        }

        private EnemyMatrix eMatrix;
    }
}