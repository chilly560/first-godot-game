using System;
using Game;
using Godot;

namespace Game.StatusModifier
{
    public interface IStatusModifier : ICollectable
    {
        /// <summary>
        /// Apply this modifier to the provided player (e.g. heal, buff, etc.).
        /// </summary>
        /// <param name="player">Target player to modify.</param>
        void ApplyModifier(Player player);
    }
}
