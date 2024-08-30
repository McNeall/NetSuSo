using FluentAssertions;
using FluentAssertions.Execution;
namespace NetSuSo.NUnit.Tests;



internal class CoordinateTestsData
{
    public static IEnumerable<TestCaseData> ValidCoordiantes
    {
        get
        {
            yield return new TestCaseData(0, 0).SetName("both_zero");
            yield return new TestCaseData(0, 0).SetName("one_and_zero");
        }
    }
}



public class CoordinateTests
{
    [SetUp]
    public void Setup()
    {
    }

    private static readonly int[][] rowColumnList = { [0, 0], [1, 6] };

    // Run a certain test of this set with
    // dotnet test -- NUnit.Where="name=='ValidCoordianteTests(0,0)'"
    // [Test, TestCaseSource(nameof(rowColumnList))]
    // public void ValidCoordianteTests(int row, int column)
    // {
    //     Coordinate coord = new Coordinate(row, column);
    //     using (new AssertionScope())
    //     {
    //         coord.Row.Should().Be(row);
    //         coord.Row.Should().Be(5);
    //     }
    // }

    // Run a certain test of this set with
    // dotnet test -- NUnit.Where="name=='ValidCoordianteTests(0,0)'"
    [Test, TestCaseSource(typeof(CoordinateTestsData), nameof(CoordinateTestsData.ValidCoordiantes))]
    public void ValidCoordianteTests(int row, int column)
    {
        Coordinate coord = new Coordinate(row, column);
        using (new AssertionScope())
        {
            coord.Row.Should().Be(row);
            coord.Row.Should().Be(5);
        }

    }
}
