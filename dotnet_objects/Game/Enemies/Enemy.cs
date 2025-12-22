using System;
using System.Collections.Generic;
using Game.Drops;
using Game.Weapons;
using Godot;

namespace Game.Enemies
{
    /// <summary>
    /// Common base class for all Enemies to inherit from.
    /// 
    /// Implements the ICollector interface, such that Enemies can hold or "collect" Drops.
    /// </summary>
    public abstract partial class Enemy : Area2D, IEnemy, ICollector, IDynamic2DPhysicsObject<Enemy>
    {
        /// <summary>
        /// Reference to GameData Signal BUS and data store.
        /// </summary>
        protected GameData gameData;
        /// <summary>
        /// Unique enemy ID assigned at instantiation.
        /// </summary>
        private int enemyid;
        /// <summary>
        /// Current HP of the enemy.
        /// </summary>
        private int hp;
        /// <summary>
        /// Factory for creating Drops upon enemy destruction.
        /// </summary>
        private AbstractDropFactory dropFactory;
        /// <summary>
        /// Chance of a drop occurring upon enemy destruction.
        /// </summary>
        private const float DROP_CHANCE = 0.5f;
        /// <summary>
        /// Number of points this enemy is worth.
        /// </summary>
        protected int worth;
        /// <summary>
        /// Delegate for modifying the physics state of this enemy.
        /// </summary>
        protected Action<Enemy, double> physicsOverhauler;
        /// <summary>
        /// Delegate for modifying the physics state of this enemy.
        /// </summary>
        protected Action<Enemy> physicsModifier;
        /// <summary>
        /// This bool is used to identify whether this exists as a part of larger wave formation.
        /// When this enemy is part of a wave formation, it will send signals back to the wave.
        /// </summary>
        protected bool inWaveFormation;
        /// <summary>
        /// Identifies which cell of the respective matrix this enemy occupies IF this enemy
        /// is part of a wave formation.
        /// 
        /// These coords will be used to notify the parent wave that this particular enemy is 
        /// performing some action or entering some state (e.g. destroying due to being defeated
        /// by the player)
        /// </summary>
        protected int formationX, formationY;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            gameData = GetNode<GameData>("/root/GameRoot/GameData");
            enemyid = gameData.GetNumberOfEnemies();
            gameData.AddEnemy(this);
            hp = 100;
            List<float> chances = new List<float>() { .5f, .5f };

            dropFactory = DropFactoryFactory.GetFactoryChance(
                DROP_CHANCE,
                chances
            );

            SetWorth();
            EnemyDestroyed += gameData.OnUpdateScoreEventHandler;
            SignalWaveEnemyDestroyed += gameData.OnSignalWaveEnemyDestroyedEventHandler;
        }

        public override void _Process(double delta)
        {
            if (physicsOverhauler is null)
                throw new NullReferenceException("Instance of Enemy does not have defined physics");

            physicsOverhauler.Invoke(this, delta);
            
            if (physicsModifier is not null)
                physicsModifier.Invoke(this);
        }
        /// <summary>
        /// To be used when creating a new Enemy as part of a wave formation
        /// </summary>
        /// <param name="X">X matrix coord</param>
        /// <param name="Y">Y matrix coord</param>
        public void SetFormation(int X, int Y)
        {
            inWaveFormation = true;
            formationX = X;
            formationY = Y;
        }

        /// <summary>
        /// Handles collision with the player, dealing damage upon contact.
        /// </summary>
        /// <param name="body">Node colliding with this Enemy</param>
        public void OnBodyEnteredEnemy(Node body)
        {
            if (body is Player player)
            {
                player.TakeDamage(50);
            }
        }
        /// <summary>
        /// Gets the unique enemy ID.
        /// </summary>
        /// <returns>Enemy ID</returns>
        public int GetID()
        {
            return enemyid;
        }
        /// <summary>
        /// Reduces enemy HP by the specified amount, and handles the 'death case' if HP falls to zero or below.
        /// </summary>
        /// <param name="amount">Damage to be taken as an int</param>
        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (hp <= 0)
            {
                EmitSignal(nameof(EnemyDestroyed), worth);

                if (inWaveFormation)
                    EmitSignal(nameof(SignalWaveEnemyDestroyed), formationX, formationY);

                Drop drop = MakeDrop();

                if (drop is not null)
                    GetParent().AddChild(drop);

                CallDeferred(nameof(FreeEnemyDeferred));
            }
        }
        /// <summary>
        /// Frees the enemy node from the scene tree at the end of the current frame.
        /// </summary>
        private void FreeEnemyDeferred()
        {
            QueueFree();
        }
        /// <summary>
        /// Creates a Drop upon enemy destruction.
        /// 
        /// Delegates Drop creation to a DropFactory.
        /// </summary>
        /// <returns>A Drop which can be collected by another ICollector</returns>
        public Drop MakeDrop()
        {
            Drop drop;
            int type = dropFactory.GetFactoryType();

            // quick n dirty implementation because it's late
            if (type == 0)
            {
                drop = dropFactory.MakeDrop((int)WeaponType.Shotgun);
                if (drop is null)
                    return null;
            } else {
                drop = dropFactory.MakeDrop((int)DropType.StatusModifier.HealthBoost);
            }
            drop.Position = Position;
            return drop;
        }
        public void SetPhysicsModifier(Action<Enemy> del)
        {
            physicsModifier = del;
        }
        public void SetPhysicsOverhauler(Action<Enemy, double> del)
        {
            physicsOverhauler = del;
        }
        /// <summary>
        /// Sets the unique enemy ID.
        /// </summary>
        /// <param name="id"></param>
        internal void SetID(int id)
        {
            enemyid = id;
        }
        /// <summary>
        /// Gets the number of points this enemy is worth.
        /// </summary>
        /// <returns></returns>
        public int GetWorth()
        {
            return worth;
        }
        /// <summary>
        /// Sets the number of points this enemy is worth.
        /// </summary>
        protected abstract void SetWorth();
        /// <summary>
        /// Sends a signal to the GameData signal bus when this enemy is destroyed.
        /// </summary>
        /// <param name="score">Amount to increase player score by.</param>
        [Signal]
        public delegate void EnemyDestroyedEventHandler(int score);
        /// <summary>
        /// A secondary signal used to notify the 'Wave' this enemy is a part of that it has been destroyed.
        /// </summary>
        [Signal]
        public delegate void SignalWaveEnemyDestroyedEventHandler(int X, int Y);
        
    }
}