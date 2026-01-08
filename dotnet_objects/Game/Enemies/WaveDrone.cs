using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Game.Enemies
{
	/// <summary>
	/// Special extensino of the Drone class which has an internal tracker for rotations,
	/// preventing over-rotation (past 90 degrees in either direction)
	/// </summary>
	public partial class WaveDrone : Drone
	{
        /// <summary>
        /// The max rotation in radians that a WaveDrone can have in either direction (left or right)
        /// 
        /// Negative = Right
        /// Positive = Left
        /// </summary>
		private const float THIRTY_DEGREES_RADIANS = .523599f;

        /// <summary>
        /// 
        /// </summary>
		private float rotation;
	    public override void _Ready()
		{
			rotation = 0;
            base._Ready();
		}
        /// <summary>
        /// Toggles the "left" or "right" or "center" sprites based on the passed in value
        /// </summary>
        /// <param name="positiveNegativeZero">Negative = right turn, Positive = left turn, 0 = center</param>
        private void ToggleSprite(float positiveNegativeZero)
        {
            if (positiveNegativeZero == 0)
            {
                sprite.Texture = gameData.TextureCache.WaveDrone.Center;
                
            }
            else if (positiveNegativeZero > 0)
            {
                sprite.Texture = gameData.TextureCache.WaveDrone.Left;
            }
            else
            {
                sprite.Texture = gameData.TextureCache.WaveDrone.Right;
            }
        }
        /// <summary>
        /// Rotates in the desired direction IF the WaveDrone has not already been rotated in that direction
        /// </summary>
        /// <param name="radians">A radian of thirty degrees (negative for right, positive for left) to roatate the drone by. -1 to rotate back to center</param>
        public void RotateDrone(float radians)
        {
            if (radians == -1 && rotation != 0)
            {
                Rotate(rotation < 0 ? THIRTY_DEGREES_RADIANS : -THIRTY_DEGREES_RADIANS);
                rotation = 0;
                ToggleSprite(0);
            }
            else if (radians != -1)
            {
                if ( (rotation == THIRTY_DEGREES_RADIANS && radians != (THIRTY_DEGREES_RADIANS * 2)) || 
                     (rotation == -THIRTY_DEGREES_RADIANS && radians != (-THIRTY_DEGREES_RADIANS * 2)))
                {
                    rotation += radians;
                    Rotate(radians);   
                } else if (rotation == 0) {
                    rotation = radians / 2;
                    Rotate(radians / 2);
                }

                ToggleSprite(radians);
            }
        }       
	}
}
