using FluentAssertions;
using FluentAssertions.Execution;
namespace NetSuSo.NUnit.Tests;



internal class CoordinateTestsData
{
    public static IEnumerable<TestCaseData> ValidCoordiantes
    {
        get
        {

            IEnumerable<int> values = Enumerable.Range(0, 9);
            IEnumerable<TestCaseData> validRowColumnCombinations = values.SelectMany(e => values, (row, column) => new TestCaseData(row, column).SetName($"({row}, {column})"));
            foreach (var data in validRowColumnCombinations) yield return data;
        }
    }

    public static IEnumerable<TestCaseData> InvalidRowCoordiantes
    {
        get
        {
            yield return new TestCaseData(-1, 5);
            yield return new TestCaseData(9, 3);
            yield return new TestCaseData(100, 3);
            yield return new TestCaseData(-20, 3);
        }
    }

    public static IEnumerable<TestCaseData> InvalidColumnCoordiantes
    {
        get
        {
            yield return new TestCaseData(3, -1);
            yield return new TestCaseData(6, 9);
            yield return new TestCaseData(3, 102);
            yield return new TestCaseData(3, -20);
        }
    }

    public static IEnumerable<TestCaseData> InvalidRowAndColumnCoordiantes
    {
        get
        {
            yield return new TestCaseData(9, -1);
            yield return new TestCaseData(-2, 10);
            yield return new TestCaseData(-5, -6);
            yield return new TestCaseData(100, 20);
        }
    }
}



public class CoordinateTests
{

    /**
    <summary>Test that coordinates are correctly created from valid row and column indices.</summary>
    <param name="row">A valid row index.</param>
    <param name="column">A valid column index.</param
    */
    [Test, TestCaseSource(typeof(CoordinateTestsData), nameof(CoordinateTestsData.ValidCoordiantes))]
    public void ValidCoordianteTests(int row, int column)
    {
        Coordinate coord = new Coordinate(row, column);
        using (new AssertionScope())
        {
            coord.Row.Should().Be(row);
            coord.Column.Should().Be(column);
        }
    }


    /**
    <summary>Test error handling for invalid row indices.</summary>
    <param name="row">An invalid row index.</param>
    <param name="column">A valid column index.</param
    */
    [Test, TestCaseSource(typeof(CoordinateTestsData), nameof(CoordinateTestsData.InvalidRowCoordiantes))]
    public void InvalidRowCoordianteTests(int row, int column)
    {
        Action createCoordiante = () => new Coordinate(row, column);

        createCoordiante
            .Should().Throw<ArgumentException>()
            .WithMessage($"""
                Expecting a value betwenn 0 and 8. (Parameter 'row')
                Actual value was {row}.
                """);
    }


    /**
    <summary>Test error handling for invalid column indices.</summary>
    <param name="row">A valid row index.</param>
    <param name="column">An invalid column index.</param
    */
    [Test, TestCaseSource(typeof(CoordinateTestsData), nameof(CoordinateTestsData.InvalidColumnCoordiantes))]
    public void InvalidColumnCoordianteTests(int row, int column)
    {
        Action createCoordiante = () => new Coordinate(row, column);

        createCoordiante
            .Should().Throw<ArgumentException>()
            .WithMessage($"""
                Expecting a value betwenn 0 and 8. (Parameter 'column')
                Actual value was {column}.
                """);
    }

    /**
    <summary>Test error handling for invalid row and column indices.</summary>
    <param name="row">An invalid row index.</param>
    <param name="column">An invalid column index.</param
    */
    [Test, TestCaseSource(typeof(CoordinateTestsData), nameof(CoordinateTestsData.InvalidRowAndColumnCoordiantes))]
    public void InvalidRowAndColumnCoordianteTests(int row, int column)
    {
        Action createCoordiante = () => new Coordinate(row, column);

        createCoordiante
            .Should().Throw<ArgumentException>()
            .WithMessage($"""
                Expecting a value betwenn 0 and 8. (Parameter 'row')
                Actual value was {row}.
                """);
    }
}
