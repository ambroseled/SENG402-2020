using NumSharp;
using SprayModule.Exception;
using SprayModule.Model;
using SprayModule.Util;
using Xunit;

namespace SprayModuleTests.Util
{
    public class HelicopterLocatorTests
    {
        [Fact]
        public void GetHelicopterIndexHeliXLessThanOriginXThrowsHelicopterOutOfSprayAreaException()
        {
            var matrix = TestHelper.MatrixCreator(0, 0, 30, 30, 1);
            var heliPosition = new NZTMPoint(-5, 8, 0);
            var locator = new HelicopterLocator(matrix, np.ndarray(new int[] {0, 1}));
            Assert.Throws<HelicopterOutOfSprayAreaException>(() => locator.GetHelicopterIndex(heliPosition));
        }
        
        [Fact]
        public void GetHelicopterIndexHeliXGreaterThanRightEdgeXThrowsHelicopterOutOfSprayAreaException()
        {
            var matrix = TestHelper.MatrixCreator(0, 0, 30, 30, 1);
            var heliPosition = new NZTMPoint(35, 8, 0);
            var locator = new HelicopterLocator(matrix, np.ndarray(new int[] {0, 1}));
            Assert.Throws<HelicopterOutOfSprayAreaException>(() => locator.GetHelicopterIndex(heliPosition));
        }
        
        [Fact]
        public void GetHelicopterIndexHeliYGreaterThanOriginYThrowsHelicopterOutOfSprayAreaException()
        {
            var matrix = TestHelper.MatrixCreator(0, 0, 30, 30, 1);
            var heliPosition = new NZTMPoint(5, 8, 0);
            var locator = new HelicopterLocator(matrix, np.ndarray(new int[] {0, 1}));
            Assert.Throws<HelicopterOutOfSprayAreaException>(() => locator.GetHelicopterIndex(heliPosition));
        }
        
        [Fact]
        public void GetHelicopterIndexHeliYLessThanRightEdgeYThrowsHelicopterOutOfSprayAreaException()
        {
            var matrix = TestHelper.MatrixCreator(0, 0, 30, 30, 1);
            var heliPosition = new NZTMPoint(5, -32, 0);
            var locator = new HelicopterLocator(matrix, np.ndarray(new int[] {0, 1}));
            Assert.Throws<HelicopterOutOfSprayAreaException>(() => locator.GetHelicopterIndex(heliPosition));
        }

        [Fact]
        public void GetHelicopterIndexHeliInMatrix0Origin()
        {
            var matrix = TestHelper.MatrixCreator(0, 0, 30, 30, 1);
            var heliPosition = new NZTMPoint(25, -8, 0);
            var locator = new HelicopterLocator(matrix, np.ndarray(new int[] {0, 1}));
            var (xIndex, yIndex) = locator.GetHelicopterIndex(heliPosition);
            Assert.Equal(25, xIndex);
            Assert.Equal(8, yIndex);
        }
        
        [Fact]
        public void GetHelicopterIndexHeliInMatrixNon0Origin()
        {
            var matrix = TestHelper.MatrixCreator(14567, 6893, 30, 30, 1);
            var heliPosition = new NZTMPoint(14579, 6866, 0);
            var locator = new HelicopterLocator(matrix, np.ndarray(new int[] {0, 1}));
            var (xIndex, yIndex) = locator.GetHelicopterIndex(heliPosition);
            Assert.Equal(12, xIndex);
            Assert.Equal(27, yIndex);
        }

    }
}