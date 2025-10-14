using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.StatusModifier
{
    public interface IStatusModifier : ICollectable
    {
        public void ApplyModifier(Player player);
    }
}