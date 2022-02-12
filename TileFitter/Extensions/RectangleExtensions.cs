using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Extensions
{
    public static class RectangleExtensions
    {
        public static int GetArea(this Rectangle  rectangle)
        {
            return rectangle.Width * rectangle.Height;
        }

        public static bool CanContain(this Rectangle rectangle, Rectangle otherRectangle)
        {
            return rectangle.Width >= otherRectangle.Width && rectangle.Height >= otherRectangle.Height;
        }
    }
}
