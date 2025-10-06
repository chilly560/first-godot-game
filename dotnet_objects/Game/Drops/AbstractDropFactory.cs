using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Drops
{
    public abstract class AbstractDropFactory
    {
        protected abstract IDrop MakeDrop(Enum type);
    }
}