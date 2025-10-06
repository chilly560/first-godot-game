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

        private Godot.Timer shootTimer;

        private bool reposition;

        private bool paused;

        private float targetPosition;

        private WeaponScene weaponScene;

        public override void _Ready()
        {
            base._Ready();
            // Create a timer that ticks down every x seconds
            // when timer ends, the bogey moves to the player's x position and shooots
            // This will be done with a godot timer node.
            reposition = true;
            paused = false;
            gameData = GetNode<GameData>("/root/Game/GameData");
            weaponScene = GetNode<WeaponScene>("./AnimatedSprite2D/WeaponScene");
            weaponScene.SetWeapon(
                WeaponFactory.CreateWeapon(WeaponType.Pistol, this)
            );
            bogeyTimer = GetNode<Godot.Timer>("./BogeyTimer");
            shootTimer = GetNode<Godot.Timer>("./BogeyAltTimer");
            bogeyTimer.WaitTime = 2f;
            shootTimer.WaitTime = 2f;
            bogeyTimer.Start();
            targetPosition = gameData.GetPlayerX();
        }

        public override void _Process(double delta)
        {
            if (reposition && !paused)
            {
                if (Position.X == targetPosition || Math.Abs(Position.X - targetPosition) < 1)
                {
                    reposition = false;
                    GD.Print("Bogey reached target position, shooting");
                    weaponScene.Shoot();
                    OnBogeyTimerTimeout();
                }

                else if (Position.X < targetPosition)
                    Position += Transform.X * 100 * (float)delta;

                else
                    Position += Transform.X * -100 * (float)delta;

                targetPosition = gameData.GetPlayerX();
            }
        }

        public void OnBogeyTimerTimeout()
        {
            if (!reposition)
            {
                GD.Print("Bogey timer timeout");
                targetPosition = gameData.GetPlayerX();
                reposition = true;
                paused = true;
                shootTimer.Start();
            }
            bogeyTimer.Start();
        }

        public void OnBogeyAltTimerTimeout()
        {
            paused = false;
        }
    }
}