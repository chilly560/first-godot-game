using System.Collections.Generic;
using Game.Drops;
using Game.Weapons;
using Godot;

namespace Game.Enemies
{

    public abstract partial class Enemy : Area2D, IEnemy, ICollector
    {
        protected GameData gameData;

        private int enemyid;

        private int hp;

        private AbstractDropFactory dropFactory;

        private const float DROP_CHANCE = 0.5f;

        protected int worth;

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
        }

        public void OnBodyEnteredEnemy(Node body)
        {
            if (body is Player player)
            {
                player.TakeDamage(50);
            }
        }

        public int GetID()
        {
            return enemyid;
        }

        public void TakeDamage(int amount)
        {
            this.hp -= amount;
            if (this.hp <= 0)
            {
                EmitSignal(nameof(EnemyDestroyed), worth);

                Drop drop = MakeDrop();

                if (drop is not null)
                    GetParent().AddChild(drop);

                CallDeferred(nameof(FreeEnemyDeferred));
            }
        }

        private void FreeEnemyDeferred()
        {
            QueueFree();
        }

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

        internal void SetID(int id)
        {
            this.enemyid = id;
        }

        public int GetWorth()
        {
            return worth;
        }

        protected abstract void SetWorth();

        [Signal]
        public delegate void EnemyDestroyedEventHandler(int score);
    }
}