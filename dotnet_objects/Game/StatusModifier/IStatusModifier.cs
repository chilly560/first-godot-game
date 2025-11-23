using System;
using Game;
using Godot;

namespace Game.StatusModifier
{
    /// <summary>
    /// Marker interface for Status Modifiers, a type of Collectable that can be
    /// obtained by the player via the Drop system.
    /// </summary>
    public interface IStatusModifier : ICollectable
    {
    }
}
