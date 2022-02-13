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
        public static int GetArea(this Rectangle rectangle)
        {
            return rectangle.Width * rectangle.Height;
        }

        public static int GetPerimeter(this Rectangle rectangle)
        {
            return (2 * rectangle.Width) + (2 * rectangle.Height);
        }

        public static bool CanContain(this Rectangle rectangle, Rectangle otherRectangle)
        {
            return rectangle.Width >= otherRectangle.Width && rectangle.Height >= otherRectangle.Height;
        }

        public static (Rectangle rectangle1, Rectangle rectangle2) SplitAlongWidth(this Rectangle rectangle, int splitWidth)
        {
            if (rectangle.Width < 2)
            {
                throw new ArgumentException("Impossible to split along width");
            }

            if (splitWidth < 1 || splitWidth >= rectangle.Width)
            {
                throw new ArgumentException("Impossible to split along width");
            }

            var rectangle1 = new Rectangle(rectangle.Left, rectangle.Top, splitWidth, rectangle.Height);
            var rectangle2 = new Rectangle(rectangle.Left + splitWidth, rectangle.Top, rectangle.Width - splitWidth, rectangle.Height);

            return (rectangle1, rectangle2);
        }

        public static (Rectangle rectangle1, Rectangle rectangle2) SplitAlongHeight(this Rectangle rectangle, int splitHeight)
        {
            if (rectangle.Height < 2)
            {
                throw new ArgumentException("Impossible to split along height");
            }

            if (splitHeight < 1 || splitHeight >= rectangle.Height)
            {
                throw new ArgumentException("Impossible to split along height");
            }

            var rectangle1 = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, splitHeight);
            var rectangle2 = new Rectangle(rectangle.Left, rectangle.Top + splitHeight, rectangle.Width, rectangle.Height - splitHeight);

            return (rectangle1, rectangle2);
        }

        public static string ToOutputFormat(this Rectangle rectangle) => $"{rectangle.Width},{rectangle.Height},{rectangle.Top},{rectangle.Left}";
    }
}
