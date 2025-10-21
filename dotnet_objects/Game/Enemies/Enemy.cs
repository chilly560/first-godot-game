using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Drops;
using Game.Weapons;
using Godot;
using Game;

namespace Game.Enemies
{

    public abstract partial class Enemy : Area2D, IEnemy, ICollector
    {
        protected GameData gameData;

        private int enemyid;

        private int hp;

        private AbstractDropFactory dropFactory;

        private const float DROP_CHANCE = 0.5f;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            gameData = GetNode<GameData>("/root/GameRoot/GameData");
            enemyid = gameData.GetNumberOfEnemies();
            gameData.AddEnemy(this);
            hp = 100;
            GD.Print(gameData.GetNumberOfEnemies());
            GD.Print("[LOG] Spawned Enemy");
            List<float> chances = new List<float>() { 1f, 0f };

            dropFactory = DropFactoryFactory.GetFactoryChance(
                DROP_CHANCE,
                chances
            );

            GD.Print(dropFactory.ToString());
        }

        public void OnBodyEnteredEnemy(Node body)
        {
            GD.Print("Enemy collided with something");
            if (body is Player player)
            {
                GD.Print("Player entered body");
                player.TakeDamage(50);
            }
            else if (body is Bullet bullet)
            {
                GD.Print("Enemy Hit");
            }
        }

        public int GetID()
        {
            return enemyid;
        }

        public void TakeDamage(int amount)
        {
            GD.Print("Taking Damage...");
            this.hp -= amount;
            GD.Print("[LOG] Enemy took " + amount + " damage, remaining HP: " + this.hp);
            if (this.hp <= 0)
            {
                //this.gameData.RemoveEnemy(this.enemyid);
                Drop drop = MakeDrop();
                GetParent().CallDeferred("add_child", drop);
                
                this.QueueFree();
            }
        }

        public Drop MakeDrop()
        {
            Drop drop = dropFactory.MakeDrop((int)WeaponType.Shotgun);
            if (drop is null)
                return null;

            drop.Position = Position;
            return drop;
        }

        internal void SetID(int id)
        {
            this.enemyid = id;
        }
    }
}