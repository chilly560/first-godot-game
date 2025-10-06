using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Enemies;
using Game.Weapons;
using Godot;
using Game.StatusModifier;

namespace Game.Drops
{
    public class Drop : IDrop
    {
        public void AddAttribute(Player player, Node2D attribute)
        {
            if (attribute is null)
                throw new ArgumentException("ERROR: ATTRIBUTE CANNOT BE NULL");
            else if (
                attribute is not IWeapon ||
                attribute is not IEnemy ||
                attribute is not IStatusModifier
            )
                throw new ArgumentException("ERROR: ATTRIBUTE NOT OF TYPE IWEAPON, IENEMY, OR ISTATUSMODIFIER");
        }
    }
}