using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Weapons;
using Godot;

namespace Game.Enemies
{
    public partial class Bogey : Enemy
    {
        private GameData gameData;

        private Godot.Timer bogeyTimer;

        private bool reposition;

        private float targetPosition;

        private WeaponScene weaponScene;

        public override void _Ready()
        {
            base._Ready();
            // Create a timer that ticks down every x seconds
            // when timer ends, the bogey moves to the player's x position and shooots
            // This will be done with a godot timer node.
            reposition = true;
            gameData = GetNode<GameData>("%GameData");
            weaponScene = GetNode<WeaponScene>("./WeaponScene");
            weaponScene.SetWeapon(
                WeaponFactory.CreateWeapon(WeaponType.Pistol)
            );
            bogeyTimer = GetNode<Godot.Timer>("./BogeyTimer");
            bogeyTimer.WaitTime = 2f;
            bogeyTimer.Start();
        }

        public override void _Process(double delta)
        {
            if (reposition)
            {
                Position += Transform.X * 100 * (float)delta;
                if (Position.X == targetPosition)
                {
                    reposition = false;
                    GD.Print("Bogey reached target position, shooting");
                    weaponScene.Shoot();
                    OnBogeyTimerTimeout();
                }
            }
        }

        public void OnBogeyTimerTimeout()
        {
            if (!reposition)
            {
                GD.Print("Bogey timer timeout");
                targetPosition = gameData.GetPlayerX();
                reposition = true;
            }
            bogeyTimer.Start();
        }
    }
}