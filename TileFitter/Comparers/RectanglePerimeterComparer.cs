using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileFitter.Extensions;
using TileFitter.Models;

namespace TileFitter.Comparers
{
    internal class RectanglePerimeterComparer : Comparer<Rectangle>
    {
        public override int Compare(Rectangle x, Rectangle y)
        {
            if (x.GetPerimeter() > y.GetPerimeter())
            {
                return -1;
            }
            else if (x.GetPerimeter() < y.GetPerimeter())
            {
                return 1;
            }
            else
            {
                return x.GetPerimeter() >= y.GetPerimeter() ? -1 : 1;
            }
        }
    }
}
