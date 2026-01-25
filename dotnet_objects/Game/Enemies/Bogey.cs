using System;
using Game.Weapons;
using Godot;

namespace Game.Enemies
{
    public partial class Bogey : Enemy, Bobber
    {
        private Godot.Timer bogeyTimer;
        private Godot.Timer shootTimer;
        private bool reposition;
        private bool paused;
        private float targetPosition;
        private WeaponScene weaponScene;
        private Healthbar healthbar;
        public float BobDelta { get ; set ; }
        public bool Down { get ; set ; }
        public Vector2 PreviousPosition { get ; set ; }
        private Timer showHealthbarTimer;
        private AudioStreamPlayer2D shootSound;
        public override void _Ready()
        {
            dropChance = .5f;
            base._Ready();
            healthbar = GetNode<Healthbar>("./Healthbar");
            healthbar.SetHealth(100);
            healthbar.Visible = false; // Hide healthbar by default
            showHealthbarTimer = GetNode<Timer>("./ShowHealthbarTimer");
            showHealthbarTimer.WaitTime = 2.0;
            showHealthbarTimer.OneShot = true;
            reposition = true;
            paused = false;
            weaponScene = GetNode<WeaponScene>("./Sprite2D/WeaponScene");
            weaponScene.SetWeapon(
                WeaponFactory.CreateWeapon(WeaponType.Pistol, this)
            );
            bogeyTimer = GetNode<Godot.Timer>("./BogeyTimer");
            shootTimer = GetNode<Godot.Timer>("./BogeyAltTimer");
            bogeyTimer.WaitTime = 2f;
            shootTimer.WaitTime = 2f;
            bogeyTimer.Start();
            targetPosition = gameData.GetPlayerX();
            shootSound = GetNode<AudioStreamPlayer2D>("./Shoot");
        }
        /// <summary>
        /// Overrides the standard enemy physics. This is to accomodate the prexisting default behavior of the bogey, whilst allowing
        /// custom physics to be added or to overhaul this behavior.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            if (physicsOverhauler is not null)
                physicsOverhauler.Invoke(this, delta);   

            if (reposition  && !paused )
            {
                if (Position.X == targetPosition || Math.Abs(Position.X - targetPosition) < 1)
                {
                    reposition = false;
                    weaponScene.Shoot();
                    OnBogeyTimerTimeout();
                    sprite.Texture = gameData.TextureCache.Bogey.Center;
                }

                else if (Position.X < targetPosition)
                {
                    Position += Transform.X * 100 * (float)delta;
                    sprite.Texture = gameData.TextureCache.Bogey.Right;
                }

                else
                {
                    Position += Transform.X * -100 * (float)delta;
                    sprite.Texture = gameData.TextureCache.Bogey.Left;
                }

                targetPosition = gameData.GetPlayerX();
            }

            physicsModifier?.Invoke(this);
        }

        public void OnBogeyTimerTimeout()
        {
            if (!reposition)
            {
                targetPosition = gameData.GetPlayerX();
                reposition = true;
                paused = true;
                shootSound.Play();
                shootTimer.Start();
            }
            bogeyTimer.Start();
        }

        public void OnBogeyAltTimerTimeout()
        {
            paused = false;
        }

        protected override void SetWorth()
        {
            worth = 25;
        }

        public void Shoot()
        {
            shootSound.Play();
            weaponScene.Shoot();
        }

        public override void TakeDamage(int amount)
        {
            healthbar.SetHealth(hp - amount, true);
            healthbar.Visible = true; 
            
            if (!showHealthbarTimer.IsStopped())
            {
                showHealthbarTimer.Stop();
            }
            showHealthbarTimer.Start();
            
            base.TakeDamage(amount);
        }

        private void OnBogeyShowHealthbarTimerTimeout()
        {
            healthbar.Visible = false;
        }
    }
}