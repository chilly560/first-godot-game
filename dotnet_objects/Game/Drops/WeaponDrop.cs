using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Game.Weapons;
using Godot;

namespace Game.Drops
{
    public class WeaponDrop : Drop
    {

        private PackedScene enemyScene { get; } = (PackedScene)ResourceLoader.Load("res://scenes/ShotgunDrop.tscn");

        private int dir = 1;

        public WeaponDrop(IWeapon weapon) : base(weapon) { }

    }
}