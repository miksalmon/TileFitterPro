using System.Collections.Generic;
using System.Drawing;
using TileFitter.Extensions;

namespace TileFitter.Comparers
{
    internal class RectangleAreaComparer : Comparer<Rectangle>
    {
        public override int Compare(Rectangle x, Rectangle y)
        {
            if (x.GetArea() > y.GetArea())
            {
                return -1;
            }
            else if (x.GetArea() < y.GetArea())
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
