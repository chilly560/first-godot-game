using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Enemies.EnemySpawning;

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
            /// Number of rows in the enemy matrix.
            /// </summary>
            public const int ROWS = 6;
            /// <summary>
            /// Number of columns in the enemy matrix.
            /// </summary>
            public const int COLUMNS = 13;
            /// <summary>
            /// 2D array representing the enemy formation.
            /// </summary>
            private Enemy[,] matrix;
            /// <summary>
            /// Constructor initializes the enemy matrix.
            /// </summary>
            public EnemyMatrix()
            {
                matrix = new Enemy[ROWS, COLUMNS];
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
                if (x >= 0 && x < ROWS && y >= 0 && y < COLUMNS)
                {
                    matrix[x, y] = enemy;
                }
            }  
            /// <summary>
            /// Adds each enemy in the matrix as a child node to the provided GameRoot.
            /// </summary>
            /// <param name="gameRoot"></param>
            public void InstiantiateMatrixEntities(GameRoot gameRoot)
            {
                for (int i = 0; i < ROWS; i++)
                    for (int j = 0; j < COLUMNS; j++)
                        if (matrix[i, j] != null)
                            gameRoot.CallDeferred("add_child", matrix[i, j]);
            }
        }
        /// <summary>
        /// Builder class for constructing EnemyMatrix instances with predefined patterns.
        /// </summary>
        private class EnemyMatrixBuilder
        {
            /// <summary>
            /// Spacing between enemies in the matrix.
            /// </summary>
            private const int SPACING = 10;
            /// <summary>
            /// Builds a default enemy matrix formation with all positions filled with Drones.
            /// </summary>
            /// <returns></returns>
            public static EnemyMatrix BuildDefaultMatrix()
            {
                Enemy[,] defaultMatrix = new Enemy[EnemyMatrix.ROWS, EnemyMatrix.COLUMNS];
                
                for (int i = 0; i < EnemyMatrix.ROWS; i++)
                {
                    for (int j = 0; j < EnemyMatrix.COLUMNS; j++)
                    {
                        // TODO: Tweak actual Vector2 positions later
                        defaultMatrix[i, j] = EnemyFactory.CreateEnemy(
                            EnemyClassification.DRONE, 
                            new Vector2(i * SPACING, j * SPACING)
                        );
                    }
                }
                return new EnemyMatrix(defaultMatrix);
            }
        }
        /// <summary>
        /// Instance of the EnemyMatrix representing this wave formation.
        /// </summary>
        private EnemyMatrix eMatrix;
        /// <summary>
        /// Constructor initializes the wave with a default enemy matrix formation.
        /// </summary>
        public Wave(WavePattern pattern = WavePattern.DEFAULT)
        {
            switch (pattern)
            {
                case WavePattern.DEFAULT:
                default:
                    eMatrix = EnemyMatrixBuilder.BuildDefaultMatrix();
                    break;
            }
        }   
        /// <summary>
        /// Adds each enemy as a child node to the provided GameRoot.
        /// </summary>
        /// <param name="gameRoot"></param>
        public void InstantiateWaveEntitites(GameRoot gameRoot)
        {
            eMatrix.InstiantiateMatrixEntities(gameRoot);
        }
    }
}