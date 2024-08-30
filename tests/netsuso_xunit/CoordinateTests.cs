using NetSuSo;
using FluentAssertions;
using FluentAssertions.Execution;
using static FluentAssertions.FluentActions;

namespace netsuso_xunit;



public class CoordinateTestsData
{

    public static TheoryData<int, int> ValidCoordiantes =>
        new TheoryData<int, int> {
            { 0, 0 },
            { 0, 6 }
        };
};


public class CoordinateTests
{

    [Theory]
    [MemberData(nameof(CoordinateTestsData.ValidCoordiantes), MemberType = typeof(CoordinateTestsData))]
    public void ValidCoordianteTests(int row, int column)
    {
        Coordinate coord = new Coordinate(row, column);

        using (new AssertionScope())
        {
            coord.Row.Should().Be(row);
            coord.Column.Should().Be(column);
        }
    }


    [Fact]
    public void InvalidCoordianteTests()
    {

        using (new AssertionScope())
        {
            var exceptionAssertion = Invoking(() => new Coordinate(-1, 0)).Should().Throw<ArgumentOutOfRangeException>();
            exceptionAssertion.WithMessage("Argument has to be between 0 and 8 both included. (Parameter 'row')\nActual value was -1.");
            var exception = exceptionAssertion.Which;
            exception.ActualValue.Should().Be(-1);
            exception.ParamName.Should().Be("row");
        }
    }
}
