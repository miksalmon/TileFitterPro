﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace TileFitter.Interfaces
{
    public interface ITileReader
    {
        IEnumerable<Rectangle> ReadTiles(string filePath);
    }
}
