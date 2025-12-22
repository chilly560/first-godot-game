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
		private const float THIRTY_DEGREES_RADIANS = .523599f;

		private float rotation;


		public override void _Ready()
		{
			base._Ready();
			rotation = 0;
		}
        /// <summary>
        /// Rotates in the desired direction IF the WaveDrone has not already been rotated in that direction
        /// </summary>
        /// <param name="radians">A radian of thirty degrees (negative for right, positive for left)</param>
        public void RotateDrone(float radians)
        {
            if (radians == -1 && rotation != 0)
            {
                Rotate(rotation < 0 ? THIRTY_DEGREES_RADIANS : -THIRTY_DEGREES_RADIANS);
                rotation = 0;
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
            }
        }       
	}
}
