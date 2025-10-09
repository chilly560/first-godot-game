using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Drops
{
    public abstract class AbstractDropFactory
    {
        public abstract IDrop MakeDrop(int type);
    }
}