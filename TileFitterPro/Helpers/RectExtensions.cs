using Windows.Foundation;

namespace TileFitterPro.Helpers
{
    internal static class RectExtensions
    {
        public static Rect Scale(this Rect rectangle, double scale)
        {
            return new Rect(rectangle.X * scale, rectangle.Y * scale, rectangle.Width * scale, rectangle.Height * scale);
        }
    }
}
