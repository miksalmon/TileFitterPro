using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Comparers
{
    internal class RandomComparer : Comparer<Rectangle>
    {
        public override int Compare(Rectangle x, Rectangle y)
        {
            var random = new Random();
            var value = random.Next();

            return value % 2 == 0 ? 1 : -1;
        }
    }
}
