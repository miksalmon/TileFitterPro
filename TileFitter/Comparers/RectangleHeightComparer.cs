using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Comparers
{
    internal class RectangleHeightComparer : Comparer<Rectangle>
    {
        public override int Compare(Rectangle x, Rectangle y)
        {
            if (x.Height > y.Height)
            {
                return -1;
            }
            else if (x.Height < y.Height)
            {
                return 1;
            }
            else
            {
                return x.Width >= y.Width ? -1 : 1;
            }
        }
    }
}
