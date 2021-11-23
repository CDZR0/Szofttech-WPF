﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szofttech_WPF.Logic
{
    class Coordinate
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
