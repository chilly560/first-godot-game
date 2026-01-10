using Godot;

namespace Game.Enemies
{
    /// <summary>
    /// Interface for properties required to 'Bob' the Wave formation up and down during
    /// Idle.
    /// </summary>
    public interface IWaveEnemy
    {   
        /// <summary>
        /// Tracks the amount of bobbing (up/down) the Enemy has done as part of its idle animation.
        /// </summary>
        float BobDelta
        {
            get;
            set;
        }
        /// <summary>
        /// Determines whether an IWaveEnemy should Bob up or down when idle.
        /// </summary>
        bool Down
        {
            get;
            set;
        }
        /// <summary>
        /// Tracks the previous GlobalPosition of this WaveEnemy
        /// </summary>
        Vector2 PreviousPosition
        {
            get;
            set;
        }
    }
}