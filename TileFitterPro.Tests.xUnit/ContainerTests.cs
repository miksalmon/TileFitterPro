using System.Collections.Generic;
using System.Drawing;
using TileFitter.Models;

using Xunit;

namespace TileFitterPro.Tests.XUnit
{
    // TODO WTS: Add appropriate tests
    public class ContainerTests
    {
        private Container Subject { get; }

        public ContainerTests()
        {
            Subject = new Container(10, 10, new List<Rectangle>());
        }

        [Fact]
        public void IsValidSolution_ShouldReturnTrueWhenValid()
        {
            Subject.PlacedTiles.AddRange(new List<Rectangle>()
            {
                new Rectangle(0, 0, 5, 5),
                new Rectangle(5, 5, 5, 5)
            });

            var result = Subject.IsValidSolution();
            Assert.True(result);
        }

        [Fact]
        public void IsValidSolution_ShouldReturnFalseWhenInvalid()
        {
        }
    }
}
