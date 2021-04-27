using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    public interface IAttacker
    {
        Attack Attack { get; }

        void AttackPosition(Point position, Direction attackDirection=Direction.None);
    }
}
