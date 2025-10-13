using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Drops;
using Godot;

namespace Game.Enemies
{

    public abstract partial class Enemy : Area2D, IEnemy
    {
        private GameData gameData;

        private int enemyid;

        private int hp;

        private ICollectable drop;

        private AbstractDropFactory dropFactory;

        private const float DROP_CHANCE = 0.5f;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            this.gameData = GetNode<GameData>("/root/Game/GameData");
            this.enemyid = this.gameData.GetNumberOfEnemies();
            this.gameData.AddEnemy(this);
            this.hp = 100;
            GD.Print(this.gameData.GetNumberOfEnemies());
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
                this.QueueFree();
            }
        }  

        internal void SetID(int id)
        {
            this.enemyid = id;
        }
    }
}