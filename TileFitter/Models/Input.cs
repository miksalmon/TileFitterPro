using System;
using System.Collections.Generic;
using System.Drawing;

namespace TileFitter.Models
{
    public class Input
    {
        public IEnumerable<Rectangle> Rectangles { get; init; }

        public Rectangle Container { get; init; }
    }
}
