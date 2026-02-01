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
        protected int hp;
        /// <summary>
        /// Factory for creating Drops upon enemy destruction.
        /// </summary>
        private AbstractDropFactory dropFactory;
        /// <summary>
        /// Chance of a drop occurring upon enemy destruction.
        /// </summary>
        protected float dropChance;
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
        /// Sister property to 'inWaveFormation' - identifies whether this enemy has been activated.
        /// 
        /// False by default as this distinction is only required for enemies part of wave formations, so as 
        /// to avoid double-counting destroyed enemies.
        /// </summary>
        public bool Activated { get; set; } = false;
        /// <summary>
        /// Internal flag to identify whether this enemy is dead. Used to ensure 
        /// This instance of an Enemy can't decrement the global entity count more than once.
        /// </summary>
        private bool isDead = false;
        /// <summary>
        /// Identifies which cell of the respective matrix this enemy occupies IF this enemy
        /// is part of a wave formation.
        /// 
        /// These coords will be used to notify the parent wave that this particular enemy is 
        /// performing some action or entering some state (e.g. destroying due to being defeated
        /// by the player)
        /// </summary>
        protected int formationX, formationY;
        /// <summary>
        /// Allows access to and modifications to the Sprite2D node for this enemy.
        /// </summary>
        protected Sprite2D sprite;
        // Called when the node enters the scene tree for the first time.
        protected AnimatedSprite2D deathExplosion;
        protected Healthbar healthbar;
        protected Timer showHealthbarTimer;
        protected Timer deathDelayTimer;
        protected AudioStreamPlayer2D explosionSound;
        public override void _Ready()
        {
            gameData = GameData.Get();
            enemyid = gameData.GetNumberOfEnemies();
            gameData.AddEnemy(this);
            hp = 100;

            List<float> chances = new List<float>() { .75f, .25f };

            dropFactory = DropFactoryFactory.GetFactoryChance(
                dropChance,
                chances
            );

            SetWorth();
            EnemyDestroyed += gameData.OnUpdateScoreEventHandler;
            SignalWaveEnemyDestroyed += gameData.OnSignalWaveEnemyDestroyedEventHandler;
            sprite = GetNode<Sprite2D>("./Sprite2D");
            deathDelayTimer = GetNode<Timer>("./DelayDeathTimer");
            deathDelayTimer.WaitTime = .3;
            deathDelayTimer.OneShot = true;
            explosionSound = GetNode<AudioStreamPlayer2D>("./Explosion");
            deathExplosion = GetNode<AnimatedSprite2D>("./ExplodeAnimation");
            deathExplosion.Visible = false; 
            healthbar = GetNode<Healthbar>("./Healthbar");
            healthbar.SetHealth(100);
            showHealthbarTimer = GetNode<Timer>("./ShowHealthbarTimer");
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
        public virtual void TakeDamage(int amount)
        {
            healthbar.SetHealth(hp - amount, true);
            hp -= amount;
            if (hp <= 0)
            {
                //GD.Print($"Enemy {this} destroyed, worth {worth} points.");
                EmitSignal(nameof(EnemyDestroyed), worth);
                SetPhysicsOverhauler((enemy, delta) => enemy.Position = enemy.Position);
                SetPhysicsModifier(null);
                sprite.Visible = false;
                healthbar.Visible = false;
                deathExplosion.Visible = true;
                if (deathDelayTimer.IsStopped())
                {
                    explosionSound.Play();
                    deathExplosion.Play();
                    deathDelayTimer.Start();
                }
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
        /// Helper method for deferring the addition of a Drop to the scene tree.
        /// </summary>
        /// <param name="drop"></param>
        private void DeferredAddDrop(Drop drop)
        {
            GetParent().AddChild(drop);
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
        /// Handles QueueFree after a delay to allow death animation to play.
        /// 
        /// Called via signal from the DelayDeathTimer node.
        /// </summary>
        public override void _ExitTree()
        {
            base._ExitTree();
        }

        public virtual void OnDelayDeathTimerTimeout()
        {
            if (inWaveFormation && Activated == false)
            {
                //GD.Print($"Enemy {this} destroyed by player, emitting SignalWaveEnemyDestroyed");
                EmitSignal(SignalName.SignalWaveEnemyDestroyed, formationX, formationY, false);
            } else if (!inWaveFormation && !isDead)
            {
                //GD.Print("---------------------");
                //GD.Print($"Entities: {gameData.Entities} before decrement");
                gameData.Entities--;
                //GD.Print($"Entities: {gameData.Entities} after decrement");
                //GD.Print("---------------------");

                isDead = true;
            }

            Drop drop = MakeDrop();

            if (drop is not null)
                CallDeferred(nameof(DeferredAddDrop), drop);

            CallDeferred(nameof(FreeEnemyDeferred));
        }
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
        public delegate void SignalWaveEnemyDestroyedEventHandler(int X, int Y, bool activated = false);
    }
}