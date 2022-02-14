
using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TileFitter.Models;

namespace TileFitterTests
{
    [TestClass]
    public class ContainerTests
    {
        [TestMethod]
        public void IsValidSolution_ShouldReturnTrueWhenValid()
        {
            var subject = new Container(10, 10, new List<Rectangle>());
            subject.PlacedTiles.AddRange(new List<Rectangle>()
            {
                new Rectangle(0, 0, 5, 5),
                new Rectangle(5, 5, 5, 5)
            });

            var result = subject.IsValidSolution();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidSolution_ShouldReturnFalseWhenOverlap()
        {
            var subject = new Container(10, 10, new List<Rectangle>());
            subject.PlacedTiles.AddRange(new List<Rectangle>()
            {
                new Rectangle(0, 0, 5, 5),
                new Rectangle(1, 1, 5, 5)
            });

            var result = subject.IsValidSolution();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidSolution_ShouldReturnFalseWhenOutside()
        {
            var subject = new Container(10, 10, new List<Rectangle>());
            subject.PlacedTiles.AddRange(new List<Rectangle>()
            {
                new Rectangle(10, 10, 10, 10)
            });

            var result = subject.IsValidSolution();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidSolution_ShouldReturnFalseWhenRemainingTiles()
        {
            var subject = new Container(10, 10, new List<Rectangle>() { new Rectangle(0, 0, 5, 5) });
            subject.PlacedTiles.AddRange(new List<Rectangle>()
            {
                new Rectangle(5, 5, 5, 5)
            });

            var result = subject.IsValidSolution();
            Assert.IsFalse(result);
        }
    }
}
