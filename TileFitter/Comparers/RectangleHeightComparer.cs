using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Comparers
{
    /// <summary>
    /// Comparer for sorting rectangles by descending order of height, followed by width for tie-breaking
    /// </summary>
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
