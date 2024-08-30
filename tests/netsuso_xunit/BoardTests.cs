

using System.Collections.Frozen;
using System.Collections.Immutable;

using FluentAssertions;

using NetSuSo;

namespace netsuso_xunit;

public class BoardFixture
{

    public int[,] MatrixRawValidValues { get; } = new int[9, 9] {
            {0, 5, 0, 7, 0, 3, 0, 6, 0},
            {0, 0, 7, 0, 0, 0, 8, 0, 0},
            {0, 0, 0, 8, 1, 6, 0, 0, 0},
            {0, 0, 0, 0, 3, 0, 0, 0, 0},
            {0, 0, 5, 0, 0, 0, 1, 0, 0},
            {7, 3, 0, 0, 4, 0, 0, 8, 6},
            {9, 0, 6, 0, 0, 0, 2, 0, 4},
            {8, 4, 0, 5, 7, 2, 0, 9, 3},
            {0, 0, 0, 4, 0, 9, 0, 0, 0},
        };

    public int[,] MatrixRawInvalidValues { get; } = new int[9, 9] {
            {0, 5, 0, 7, 0, 3, 0, 10, 0},
            {0, 0, 7, 0, 0, 0, 8, 0, 0},
            {0, 0, 0, 8, 1, 6, 0, 0, 0},
            {0, 0, 0, 0, 3, 0, 0, 0, 0},
            {0, 0, 20, 0, 0, 0, 1, 0, 0},
            {7, 3, 0, 0, 4, 0, 0, 8, 6},
            {9, 0, 6, 0, 0, 0, 2, 0, 4},
            {8, 4, 0, 5, 7, 2, 0, 9, 3},
            {0, 0, 0, 4, 0, -1, 0, 0, 0},
        };

    public int[,] MatrixRawInvalidNumberOfRows { get; } = new int[2, 9] {
            {0, 5, 0, 7, 0, 3, 0, 10, 0},
            {0, 0, 7, 0, 0, 0, 8, 0, 0},
        };

    public int[,] MatrixRawInvalidNumberOfColumns { get; } = new int[9, 3] {
            {0, 5, 0},
            {0, 0, 7},
            {0, 0, 0},
            {0, 0, 0},
            {0, 0, 5},
            {7, 3, 0},
            {9, 0, 6},
            {8, 4, 0},
            {0, 0, 0},
        };

    public int[] EnumerableRawValidValues { get; } = [
            0, 5, 0, 7, 0, 3, 0, 6, 0,
            0, 0, 7, 0, 0, 0, 8, 0, 0,
            0, 0, 0, 8, 1, 6, 0, 0, 0,
            0, 0, 0, 0, 3, 0, 0, 0, 0,
            0, 0, 5, 0, 0, 0, 1, 0, 0,
            7, 3, 0, 0, 4, 0, 0, 8, 6,
            9, 0, 6, 0, 0, 0, 2, 0, 4,
            8, 4, 0, 5, 7, 2, 0, 9, 3,
            0, 0, 0, 4, 0, 9, 0, 0, 0
    ];

    public int[] EnumerableRawInvalidLength { get; } = [
    0, 5, 0, 7, 0, 3, 0, 6, 0,
        0, 0, 7, 0, 0, 0, 8, 0, 0,
        0, 0, 0, 8, 1, 6, 0, 0, 0,
        0, 0, 0, 0, 3, 0, 0, 0, 0,
        0, 0, 5, 0, 0, 0, 1, 0, 0,
        7, 3, 0, 0, 4, 0, 0, 8, 6,
        9, 0, 6, 0, 0, 0, 2, 0, 4,
        8, 4, 0, 5, 7, 2, 0, 9, 3,
        0, 0, 0, 4, 0, 9,
    ];

    public int[] EnumerableRawInvalidValues = [
            0, 5, -1, 7, 0, 3, 0, 6, 0,
            0, 0, 7, 0, 0, 0, 8, 0, 0,
            0, 0, 0, 8, 1, 6, 0, 0, 0,
            0, 0, 0, 0, 3, 0, 0, 0, 0,
            0, 0, 5, 10, 0, 0, 1, 0, 0,
            7, 3, 0, 0, 4, 0, 0, 8, 6,
            9, 0, 6, 0, 0, 0, 2, 0, 4,
            8, 4, 20, 5, 7, 2, 0, 9, 3,
            0, 0, 0, 4, 0, 9, 0, 0, 99
    ];

    public string StringRawValidValues { get; } = "050703060007000800000816000000030000005000100730040086906000204840572093000409000";

    public string StringRawInvalidLenght { get; } = "050703060007000800000816000000030000005000100730040086906000204840572093000409";

    public string StringRawInvalidCharacters { get; } = "050703060007000800000816000000030000?05000100730040086906ä002048405720930-04090亯0";

    public ISet<int> AllowedValues { get; } = Enumerable.Range(1, 9).ToFrozenSet();

}


public class BoardTests : IClassFixture<BoardFixture>
{

    // Some tests will change the board it cannot be shared amoung all tests. Scope it to the class
    private Board ValidBoard { get; init; }
    private readonly BoardFixture _sharedValues;

    public BoardTests(BoardFixture boardFixture)
    {
        _sharedValues = boardFixture;
        ValidBoard = Board.FromMatrix(_sharedValues.MatrixRawValidValues);

    }

    [Fact]
    void TestCreateBoardFromNestedListWithInvalidValues()
    {

        Action createBoardFromMatrix = () => Board.FromMatrix(_sharedValues.MatrixRawInvalidValues);
        createBoardFromMatrix.Should()
                             .Throw<ArgumentException>()
                             .WithMessage("""
                                        Found invalid values at the following positions (Position: Value):
                                        Coordinate { Row = 0, Column = 7 }: 10
                                        Coordinate { Row = 4, Column = 2 }: 20
                                        Coordinate { Row = 8, Column = 5 }: -1 (Parameter 'input')
                                        """);
    }

    [Fact]
    void TestCreateBoardFromNestedListWithInvalidNumberOfRows()
    {

        Action createBoardFromMatrix = () => Board.FromMatrix(_sharedValues.MatrixRawInvalidNumberOfRows);
        createBoardFromMatrix.Should()
                             .Throw<ArgumentException>()
                             .WithMessage("Expected exactly 9 rows, but found 2. (Parameter 'input')");
    }

    [Fact]
    void TestCreateBoardFromNestedListWithInvalidNumberOfColumns()
    {

        Action createBoardFromMatrix = () => Board.FromMatrix(_sharedValues.MatrixRawInvalidNumberOfColumns);
        createBoardFromMatrix.Should()
                             .Throw<ArgumentException>()
                             .WithMessage("Expected exactly 9 columns, but found 3. (Parameter 'input')");
    }


    [Fact]
    void TestCreateBoardFromEnumerableWithValidValue()
    {
        Board board = Board.FromEnumerable(_sharedValues.EnumerableRawValidValues);
        IEnumerable<int> indices = Enumerable.Range(0, 9);
        IEnumerable<Coordinate> coordinates = indices.SelectMany(x => indices, (x, y) => new Coordinate(x, y));
        foreach (Coordinate coordinate in coordinates)
        {

            board[coordinate].Should().Be(_sharedValues.EnumerableRawValidValues[coordinate.Row * Board.BOARD_DIM + coordinate.Column]);
        }
    }

    [Fact]
    void TestCreateBoardFromFlatArrayWithInvalidLength()
    {
        Action action = () => Board.FromEnumerable(_sharedValues.EnumerableRawInvalidLength);
        action.Should().Throw<ArgumentException>().WithMessage("Expected exactly 81 characters, but found 78. (Parameter 'input')");
    }

    [Fact]
    void TestCreateBoardFromFlatArrayWithInvalidValue()
    {

        Action action = () => Board.FromEnumerable(_sharedValues.EnumerableRawInvalidValues);
        action.Should().Throw<ArgumentException>().WithMessage("""
        Found invalid values at the following positions (Position: Value):
        2: -1
        39: 10
        65: 20
        80: 99 (Parameter 'input')
        """);
    }

    [Fact]
    void TestCreateBoardFromValidString()
    {

        Board board = Board.FromString(_sharedValues.StringRawValidValues);
        IEnumerable<int> indices = Enumerable.Range(0, 9);
        IEnumerable<Coordinate> coordinates = indices.SelectMany(x => indices, (x, y) => new Coordinate(x, y));
        foreach (Coordinate coordinate in coordinates)
        {

            board[coordinate].Should().Be(_sharedValues.EnumerableRawValidValues[coordinate.Row * Board.BOARD_DIM + coordinate.Column]);
        }
    }

    [Fact]
    void TestCreateBoardFromInvalidLengthString()
    {
        Action action = () => Board.FromString(_sharedValues.StringRawInvalidLenght);
        action.Should().Throw<ArgumentException>().WithMessage("Expected exactly 81 characters, but found 78. (Parameter 'input')");
    }

    [Fact]
    void TestCreateBoardFromInvalidValueString()
    {
        Action action = () => Board.FromString(_sharedValues.StringRawInvalidCharacters);
        action.Should().Throw<ArgumentException>().WithMessage("""
        Found invalid values at the following positions (Position: Value):
        36: ?
        57: ä
        73: -
        79: 亯 (Parameter 'input')
        """);
    }


    [Fact]
    void TestAvailableColumnValues()
    {
        for (int colIdx = 0; colIdx < 9; colIdx++)
        {
            ISet<int> usedColumnValues = new HashSet<int>();
            for (int rowIdx = 0; rowIdx < 9; rowIdx++)
            {
                usedColumnValues.Add(_sharedValues.MatrixRawValidValues[rowIdx, colIdx]);
            }
            ValidBoard.AvailableColumnValues(colIdx).Should().BeEquivalentTo(_sharedValues.AllowedValues.Except(usedColumnValues)).And.BeAssignableTo<FrozenSet<int>>();
        }
    }

    [Fact]
    void TestAvailableColumnValuesInvalidIndex()
    {
        ValidBoard.Invoking(board => board.AvailableColumnValues(-1)).Should().Throw<ArgumentException>().WithMessage("""
        Expecting a value betwenn 0 and 8. (Parameter 'columnIndex')
        Actual value was -1.
        """);
    }


    [Fact]
    void TestAvailableRowValues()
    {
        for (int rowIdx = 0; rowIdx < 9; rowIdx++)
        {
            ISet<int> usedRowValues = new HashSet<int>();
            for (int colIdx = 0; colIdx < 9; colIdx++)
            {
                usedRowValues.Add(_sharedValues.MatrixRawValidValues[rowIdx, colIdx]);
            }
            ValidBoard.AvailableRowValues(rowIdx).Should().BeEquivalentTo(_sharedValues.AllowedValues.Except(usedRowValues)).And.BeAssignableTo<FrozenSet<int>>();
        }
    }

    [Fact]
    void TestAvailableSquareValues()
    {
        int[] oneDimeCoords = [0, 3, 6];
        // Cartesian Product
        Coordinate[] topLeftCells = oneDimeCoords.SelectMany(ele => oneDimeCoords, (row, column) => new Coordinate(row, column)).ToArray();

        foreach (var topLeftCell in topLeftCells)
        {

            HashSet<int> usedSquareValues = [];
            foreach (var col in Enumerable.Range(topLeftCell.Column, 3))
            {
                foreach (var row in Enumerable.Range(topLeftCell.Row, 3))
                {
                    usedSquareValues.Add(_sharedValues.MatrixRawValidValues[row, col]);
                }
            }
            ValidBoard.AvailableSquareValues(topLeftCell).Should().BeEquivalentTo(_sharedValues.AllowedValues.Except(usedSquareValues), "because these values are not used in square with top left cell at coordinate {0}", topLeftCell.ToString()).And.BeAssignableTo<FrozenSet<int>>();
        }
    }

    [Fact]
    void TestAvailableRowValuesInvalidIndex()
    {
        ValidBoard.Invoking(board => board.AvailableRowValues(-1))
            .Should().Throw<ArgumentException>()
            .WithMessage("""
                Expecting a value betwenn 0 and 8. (Parameter 'rowIndex')
                Actual value was -1.
                """);
    }

    [Fact]
    void TestIsValidWithValidValue()
    {
        Coordinate coordinate = new Coordinate(0, 0);
        ValidBoard.IsValid(coordinate, 1).Should().BeTrue();
    }

    [Fact]
    void TestIsValidWithInvalidValue()
    {
        Coordinate coordinate = new Coordinate(0, 0);
        ValidBoard.IsValid(coordinate, 1).Should().BeTrue();
    }

    [Fact]
    void TestBoardIterator()
    {
        foreach (var cell in ValidBoard)
        {
            cell.Value.Should().Be(_sharedValues.MatrixRawValidValues[cell.Coordinate.Row, cell.Coordinate.Column], "because {0} was initialized with it", cell.Coordinate);
        }
    }

    [Fact]
    void TestBoardGettingByIndex()
    {
        IEnumerable<int> dimensions = Enumerable.Range(0, Board.BOARD_DIM);
        IEnumerable<Coordinate> coordinates = dimensions.SelectMany(x => dimensions, (x, y) => new Coordinate(x, y));
        foreach (Coordinate coordinate in coordinates)
        {
            ValidBoard[coordinate].Should().Be(_sharedValues.MatrixRawValidValues[coordinate.Row, coordinate.Column], "because  {0} was initialized with this value", coordinate);

        }
    }

    [Fact]
    void TestBoardSettingValidValueByIndex()
    {
        Coordinate coordinate = new Coordinate(8, 8);
        ValidBoard[coordinate] = 1;
        ValidBoard[coordinate].Should().Be(1, "because the empty value has been overwritten");
    }

    [Fact]
    void TestBoardSettingInvalidValueByIndex()
    {
        Coordinate coordinate = new Coordinate(0, 0);
        ValidBoard.Invoking(board => board[coordinate] = 5)
            .Should().Throw<InvalidCellValueException>()
            .WithMessage("Invalid value for coordinate. Coordinate { Row = 0, Column = 0 }: 5");
    }

    [Fact]
    void TestBoardSettingValueAndUnsetting()
    {
        Coordinate coordinate = new Coordinate(8, 8);
        ValidBoard[coordinate] = 1;
        ValidBoard[coordinate] = 0;
        ValidBoard[coordinate].Should().Be(0);
    }


    [Fact]
    void TestBoardRepresentation()
    {

        Board board = Board.FromMatrix(_sharedValues.MatrixRawValidValues);

        string boardString = BoardFormatter.UnicodeRepresentation(board);
        boardString.Should().Be("""
        ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐
        │ 0 │ 5 │ 0 │ 7 │ 0 │ 3 │ 0 │ 6 │ 0 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 0 │ 0 │ 7 │ 0 │ 0 │ 0 │ 8 │ 0 │ 0 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 0 │ 0 │ 0 │ 8 │ 1 │ 6 │ 0 │ 0 │ 0 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 0 │ 0 │ 0 │ 0 │ 3 │ 0 │ 0 │ 0 │ 0 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 0 │ 0 │ 5 │ 0 │ 0 │ 0 │ 1 │ 0 │ 0 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 7 │ 3 │ 0 │ 0 │ 4 │ 0 │ 0 │ 8 │ 6 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 9 │ 0 │ 6 │ 0 │ 0 │ 0 │ 2 │ 0 │ 4 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 8 │ 4 │ 0 │ 5 │ 7 │ 2 │ 0 │ 9 │ 3 │
        ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
        │ 0 │ 0 │ 0 │ 4 │ 0 │ 9 │ 0 │ 0 │ 0 │
        └───┴───┴───┴───┴───┴───┴───┴───┴───┘
        """
        );
        Console.WriteLine(boardString);
    }
}
