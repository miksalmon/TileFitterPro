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
            if (x.Width > y.Width)
            {
                return -1;
            }
            else if (x.Width < y.Width)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
