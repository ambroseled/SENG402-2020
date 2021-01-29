using SprayModule.Util;
using Xunit;

namespace SprayModuleTests.Util
{
    public class CoordinateConverterTests
    {
        [Fact]
        public void TestNZTMtoLatLngLatitude()
        {
        //Given
        var converter = new CoordinateConverter();
        //When
        var result = converter.NZTMtoLatLng(1570634.6, 5180148.2);
        //Then
        Assert.Equal(-43.53103121290705, result.Item1);
        }


        [Fact]
        public void TestNZTMtoLatLngLongitude()
        {
        //Given
        var converter = new CoordinateConverter();
        //When
        var result = converter.NZTMtoLatLng(1570634.6, 5180148.2);
        //Then
        Assert.Equal(172.63658005933488, result.Item2);
        }
    }
}