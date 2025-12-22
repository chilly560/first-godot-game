using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace NewGameProject.scripts
{
    public partial class EnemyBullet : Bullet 
    {
        public override void OnAreaEnteredBullet(Node body)
        {
            if (body is Player p)
            {
                p.TakeDamage(Damage);
                QueueFree();
            }
        }
    }
}