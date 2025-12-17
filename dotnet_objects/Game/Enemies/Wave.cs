using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Enemies.EnemySpawning;
using System.ComponentModel.DataAnnotations;

namespace Game.Enemies
{
    /// <summary>
    /// Class representing an enemy wave formation
    /// </summary>
    public class Wave
    {
        /// <summary>
        /// Physics modifiers for enemies in a wave
        /// </summary>
        public class WaveEnemyPhysicsOverhaulers
        {
            /// <summary>
            /// Drones don't move after instantiation.
            /// </summary>
            /// <param name="d"></param>
            /// <param name="delta"></param>
            public static void DefaultWavePhysicsOverhauler(Enemy d, double delta)
            {
                d.Position = d.Position;
            }

            public static void DiveWavePhysicsOverhauler(Enemy d, double delta)
            {
                d.Position -= d.Transform.Y * 1f;
            }
        }

        public class WaveEnemyPhysicsModifiers
        {
            private const float THIRTY_DEGREES_RADIANS = 0.523599f;

            private const float LEFT = THIRTY_DEGREES_RADIANS * 2;

            private const float RIGHT = THIRTY_DEGREES_RADIANS * -2;
            /// <summary>
            /// Currently WIP to figure out a way to make the enemy 'rotate' or 'look at' the player.
            /// 
            /// The complexity being that LookAt is not functioning as expected, and rotating based on 
            /// the angle between the enemy is proving to be trickier than expected.
            /// 
            /// Temporary solution: rotate 30 degrees in either direction based on direction of movement.
            /// This is implemented using a custom 'RotateDrone' method inside of the WaveDrone.
            /// 
            /// See ./WaveDrone.cs
            /// </summary>
            /// <param name="d"></param>
            /// <exception cref="ArgumentException"></exception>
            public static void FindPlayerPhysicsModifier(Enemy d)
            {

                if (d is WaveDrone wd)
                {
                    float playerx = GameData.Get().GetPlayerX();

                    if (Math.Abs(wd.Position.X - playerx) > 5)
                    {
                        if (wd.Position.X < playerx)
                        {
                            wd.RotateDrone(RIGHT);
                            //wd.LookAt(GameData.Get().GetPlayerPosition());
                        }
                        else if (wd.Position.X > playerx)
                        {
                            wd.RotateDrone(LEFT);
                        }
                    } else wd.RotateDrone(-1);

                    //wd.LookAt(GameData.Get().GetPlayerGlobalPosition());
                    /*
                    /*
                    if (diff < 0 && alreadyPassedPlayer > 0)
                    {
                        float absX = playerx - d.Position.X;
                        float absY = playery - d.Position.Y;
                        double hypotenuse = Math.Sqrt((absX * absX) + (absY * absY));
                        double rotationAngleRads = Mathf.DegToRad(90f) - Math.Abs(Math.Asin(
                            absY / hypotenuse
                        ));
                        d.Rotate((float)rotationAngleRads * -1);
                    }
                    */
                } else throw new ArgumentException("Invalid Enemy Type (Must be WaveDrone)");
            }
        }
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
            public const int ROWS = 8;
            /// <summary>
            /// Number of columns in the enemy matrix.
            /// </summary>
            public const int COLUMNS = 4;
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
            /// 
            public void ActivateRandom(bool lockToPlayer)
            {
                Random rand = new Random();
                int x = rand.Next(0, ROWS);
                int y = rand.Next(0, COLUMNS);

                if (matrix[x, y] != null)
                    ActivateEnemy(x, y, lockToPlayer);

                else ActivateRandom(lockToPlayer);
            }
            public void ActivateEnemy(int x, int y, bool lockToPlayer)
            {
                if (lockToPlayer && matrix[x, y] != null)
                {
                    matrix[x, y].SetPhysicsOverhauler(WaveEnemyPhysicsOverhaulers.DiveWavePhysicsOverhauler);
                    matrix[x, y].SetPhysicsModifier(WaveEnemyPhysicsModifiers.FindPlayerPhysicsModifier);
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
            private const int SPACING = 35;
            /// <summary>
            /// Centers the wave.
            /// </summary>
            private const int X_OFFSET = -115;
            /// <summary>
            /// Vertical offset for the wave spawn position (so that it actually instantiates
            /// on screen).
            /// </summary>
            private const int Y_OFFSET = 205;
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
                            EnemyClassification.WAVE_DRONE, 
                            new Vector2((i * SPACING) + X_OFFSET, (j * SPACING) + Y_OFFSET)
                        );

                        defaultMatrix[i, j].Rotate(Mathf.DegToRad(180));

                        defaultMatrix[i, j].SetPhysicsOverhauler(
                            WaveEnemyPhysicsOverhaulers.DefaultWavePhysicsOverhauler
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

        public void ActivateEnemy(bool lockToPlayer)
        {
            if (lockToPlayer)
            {
                // temporarily hardcoded to be true for testing purposes
                eMatrix.ActivateRandom(true);
            }
        }
    }
}