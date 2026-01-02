using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Enemies.EnemySpawning;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Xml.Schema;

namespace Game.Enemies
{
    /// <summary>
    /// Class representing an enemy wave formation
    /// </summary>
    public class Wave
    {
        private const int BONUS = 250;
        /// <summary>
        /// GameData event bus singleton.
        /// </summary>
        private GameData gameData;

        public int WaveID { get; private set; } = GameData.Get().WaveNumber;
        /// <summary>
        /// Main behavioral (physics) instructions for Wave enemies. These are applied at runtime.
        /// </summary>
        public class WaveEnemyPhysicsOverhaulers
        {
            /// <summary>
            /// Enemies don't move after instantiation.
            /// </summary>
            /// <param name="d">The Enemy to apply these instructions to</param>
            /// <param name="delta">The time elapsed since the last frame</param>
            public static void DefaultWavePhysicsOverhauler(Enemy d, double delta)
            {
                d.Position = d.Position;
            }
            /// <summary>
            /// Makes the enemy dive downwards in a straight line.
            /// </summary>
            /// <param name="d">The Enemy to apply these instructions to</param>
            /// <param name="delta">The time elapsed since the last frame</param>
            public static void DiveWavePhysicsOverhauler(Enemy d, double delta)
            {
                d.Position -= d.Transform.Y * 1f;
            }
        }
        /// <summary>
        /// Additional physics instructions to be run after the PhysicsOverhauler. 
        /// </summary>
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
            /// <param name="d">The Enemy to apply these instructions to</param>
            /// <exception cref="ArgumentException">Thrown if the Enemy is not a WaveDrone</exception>
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
                        }
                        else if (wd.Position.X > playerx)
                        {
                            wd.RotateDrone(LEFT);
                        }
                    } else wd.RotateDrone(-1);
                } else throw new ArgumentException("Invalid Enemy Type (Must be WaveDrone)");
            }

            public static void BogeyShootPhysicsModifier(Enemy d)
            {
                if (d is Bogey b && (Math.Abs(b.Position.X - GameData.Get().GetPlayerX()) < 1))
                    b.Shoot();
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
            public const int COLUMNS = 8;
            /// <summary>
            /// Number of columns in the enemy matrix.
            /// </summary>
            public const int ROWS = 4;
            /// <summary>
            /// Number of cells actually populated by an enemy.
            /// </summary>
            public int Count;
            /// <summary>
            /// 2D array representing the enemy formation.
            /// </summary>
            private Enemy[,] matrix;
            private Random random;
            /// <summary>
            /// Constructor initializes the enemy matrix.
            /// </summary>
            public EnemyMatrix() : this(new Enemy[COLUMNS, ROWS])
            { }       
            /// <summary>
            /// Constructor with predefined matrix.
            /// </summary>
            /// <param name="matrix"></param>
            public EnemyMatrix(Enemy[,] matrix)
            {
                this.matrix = matrix;
                random = new Random();
            }
            /// <summary>
            /// Sets the enemy at the specified coordinates.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="enemy"></param>
            public void PopulateMatrix(int x, int y, Enemy enemy)
            {
                if (x >= 0 && x < COLUMNS && y >= 0 && y < ROWS)
                {
                    matrix[x, y] = enemy;
                }
            }  
            public void ActivateRandom(bool lockToPlayer)
            {
                int x = random.Next(0, COLUMNS);
                int y = random.Next(0, ROWS);

                if (matrix[x, y] != null)
                {
                    ActivateEnemy(x, y, lockToPlayer);
                }
                else if (Count > 0) 
                    ActivateRandom(lockToPlayer);
            }
            /// <summary>
            /// ACtivates the enemy at the specified coordinates.
            /// 
            /// When an enemy is activate, it is considered "destroyed" from the wave formation's perspective.
            /// </summary>
            /// <param name="lockToPlayer">Bool determining whether the enemy should chase the player's position</param>
            public void ActivateEnemy(int x, int y, bool lockToPlayer)
            {
                if (lockToPlayer && matrix[x, y] != null && matrix[x, y] is WaveDrone)
                {
                    //GD.Print($"Activating Enemy {matrix[x, y]} at [{x}, {y}] from wave formation.");
                    matrix[x, y].SetPhysicsOverhauler(WaveEnemyPhysicsOverhaulers.DiveWavePhysicsOverhauler);
                    matrix[x, y].SetPhysicsModifier(WaveEnemyPhysicsModifiers.FindPlayerPhysicsModifier);
                    //GD.Print("Enemy activated from wave formation, Manually Calling OnSignalWaveEnemyDestroyedEventHandler");
                    GameData.Get().OnSignalWaveEnemyDestroyedEventHandler(x, y, true);
                }
            }
            /// <summary>
            /// Adds each enemy in the matrix as a child node to the provided GameRoot.
            /// </summary>
            /// <param name="gameRoot"></param>
            public void InstiantiateMatrixEntities(GameRoot gameRoot)
            {
                for (int i = 0; i < COLUMNS; i++)
                    for (int j = 0; j < ROWS; j++)
                        if (matrix[i, j] != null)
                        {
                            gameRoot.CallDeferred("add_child", matrix[i, j]);
                            Count++;
                            //GD.Print($"Count: {Count}");
                        }
            }
            public Enemy GetEnemyAt(int x, int y)
            {
                if (x >= 0 && x < COLUMNS && y >= 0 && y < ROWS)
                {
                    if (matrix[x, y] == null)
                        throw new NullReferenceException("No enemy at specified matrix coordinates");

                    return matrix[x, y];
                }
                throw new IndexOutOfRangeException("Invalid matrix coordinates");
            }

            public void RemoveEnemyAt(int x, int y)
            {
                if (x >= 0 && x < COLUMNS && y >= 0 && y < ROWS)
                {
                    matrix[x, y] = null;
                }
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
            private const int SPACING = 36;
            /// <summary>
            /// Centers the wave.
            /// </summary>
            private const int X_OFFSET = -125;
            /// <summary>
            /// Vertical offset for the wave spawn position (so that it actually instantiates
            /// on screen).
            /// </summary>
            private const int Y_OFFSET = 60;

            private static Vector2 GetEnemyMatrixVector2(int i, int j)
            {
                return new Vector2((i * SPACING) + X_OFFSET, (j * SPACING) + Y_OFFSET);
            }
            /// <summary>
            /// Builds a default enemy matrix formation with all positions filled with Drones.
            /// </summary>
            /// <returns></returns>
            public static EnemyMatrix BuildDefaultMatrix()
            {
                Enemy[,] defaultMatrix = new Enemy[EnemyMatrix.COLUMNS, EnemyMatrix.ROWS];
                
                for (int i = 0; i < EnemyMatrix.COLUMNS; i++)
                    for (int j = 0; j < EnemyMatrix.ROWS; j++)
                    {
                        // TODO: Tweak actual Vector2 positions later
                        defaultMatrix[i, j] = EnemyFactory.CreateEnemy(
                            EnemyClassification.WAVE_DRONE, 
                            GetEnemyMatrixVector2(i, j)
                        );

                        defaultMatrix[i, j].SetFormation(i, j);

                        defaultMatrix[i, j].Rotate(Mathf.DegToRad(180));

                        defaultMatrix[i, j].SetPhysicsOverhauler(
                            WaveEnemyPhysicsOverhaulers.DefaultWavePhysicsOverhauler
                        );
                    }

                EnemyMatrix e = new EnemyMatrix(defaultMatrix);
                //e.Count = EnemyMatrix.ROWS * EnemyMatrix.COLUMNS;
                return e;
            }

            public static EnemyMatrix BuildAggressiveMatrix()
            {
                Enemy[,] aggressiveMatrix = new Enemy[EnemyMatrix.COLUMNS, EnemyMatrix.ROWS];

                Random rand = new Random();

                for(int i = 0; i < EnemyMatrix.COLUMNS; i++)
                    for (int j = 0; j < EnemyMatrix.ROWS; j++)
                    {
                        if (j == 0 && i % 2 == 0)
                        {
                            aggressiveMatrix[i, j] = EnemyFactory.CreateEnemy(
                                EnemyClassification.WAVE_BOGEY,
                                GetEnemyMatrixVector2(i, j)
                            );
                        }

                        else if (j > 0 && rand.NextDouble() * 5 > 2.5)
                        {
                            aggressiveMatrix[i, j] = EnemyFactory.CreateEnemy(
                                EnemyClassification.WAVE_DRONE,
                                GetEnemyMatrixVector2(i, j)
                            );

                            aggressiveMatrix[i, j].Rotate(Mathf.DegToRad(180));
                        }
                        
                        if (aggressiveMatrix[i, j] != null)
                        {
                            aggressiveMatrix[i, j].SetFormation(i, j);

                            aggressiveMatrix[i, j].SetPhysicsOverhauler(
                                WaveEnemyPhysicsOverhaulers.DefaultWavePhysicsOverhauler
                            );
                        }
                    }

                return new EnemyMatrix(aggressiveMatrix);
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
            gameData = GameData.Get();  
            gameData.RemoveEnemyXYFromFormation += EnemyDestroyedEventHandler;
            
            switch (pattern)
            {
                case WavePattern.DEFAULT:
                    eMatrix =  EnemyMatrixBuilder.BuildDefaultMatrix();
                    break;
                case WavePattern.AGGRESSIVE:
                    eMatrix = EnemyMatrixBuilder.BuildAggressiveMatrix();
                    break;
                default:
                    throw new ArgumentException("Invalid Wave Pattern");
            }
        }   
        /// <summary>
        /// Constructor called for subsequent waves after the first wave. 
        /// </summary>
        /// <param name="WaveID">Chronological by wave iterations.</param>
        public Wave(int WaveID, WavePattern pattern = WavePattern.DEFAULT) : this(pattern)
        {
            this.WaveID = WaveID;
        }
        /// <summary>
        /// Adds each enemy as a child node to the provided GameRoot.
        /// </summary>
        /// <param name="gameRoot"></param>
        public void InstantiateWaveEntitites(GameRoot gameRoot)
        {
            eMatrix.InstiantiateMatrixEntities(gameRoot);
        }
        
        private void EnemyDestroyedEventHandler(int X, int Y, bool activated)
        {
            if (activated)
                eMatrix.GetEnemyAt(X, Y).Activated = true;

            eMatrix.RemoveEnemyAt(X, Y);

            eMatrix.Count--;
                
            GD.Print($"Wave ({WaveID}) Enemy Count: {eMatrix.Count}.");

            if (eMatrix.Count <= 0)
                Destroy();
        }

        public void ActivateEnemy(bool lockToPlayer)
        {
            if (lockToPlayer)
            {
                // temporarily hardcoded to be true for testing purposes
                eMatrix.ActivateRandom(true);
            } 
        }

        private void Destroy()
        {
            gameData.RemoveEnemyXYFromFormation -= EnemyDestroyedEventHandler;
            // GD.Print($"Wave Complete.");
            gameData.EmitWaveDestroyedEventHandlerSignal();
            gameData.EmitWaveBonusEventHandlerSignal(BONUS);
        }
        /// <summary>
        /// Number of Enemies remaining in the current wave.
        /// </summary>
        public int GetCount()
        {
            return eMatrix.Count;
        }
    }
}