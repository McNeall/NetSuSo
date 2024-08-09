namespace NetSuSo.NUnit.Tests;

using NetSuSo;
// using System.Collections.Generic.List;

public class CoordinateTests
{
    [SetUp]
    public void Setup()
    {
    }


    private static readonly int[][] rowColumnList = { [0, 0], [1, 6] };



    [Test, TestCaseSource(nameof(rowColumnList))]
    public void ValidCoordianteTests(int row, int column)
    {
        Coordinate coord = new Coordinate(row, column);
        Assert.That(coord, Has.Property("Row").EqualTo(row));
        Assert.That(coord, Has.Property("Column").EqualTo(column));

    }
}
