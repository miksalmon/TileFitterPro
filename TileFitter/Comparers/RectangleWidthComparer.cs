using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Comparers
{
    internal class RectangleWidthComparer : Comparer<Rectangle>
    {
        public override int Compare(Rectangle x, Rectangle y)
        {
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
                return x.Height >= y.Height ? -1 : 1;
            }
        }
    }
}
