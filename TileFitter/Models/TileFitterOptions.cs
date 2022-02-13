using TileFitter.Interfaces;

namespace TileFitter
{
    public class TileFitterOptions
    {
        public Algorithm Algorithm { get; }
    }

    public enum Algorithm
    {
        MaximalRectangles
    }
}