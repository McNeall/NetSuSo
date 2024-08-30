

using System.Collections.Frozen;
using System.Collections.Immutable;

using FluentAssertions;

using NetSuSo;


public class SharedBoardTestData
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

public class BoardTests
{

    // Some tests will change the board it cannot be shared amoung all tests. Scope it to the class
    private Board ValidBoard { get; set; }
    private SharedBoardTestData _sharedValues;

    [OneTimeSetUp]
    public void Init()
    {
        _sharedValues = new SharedBoardTestData();
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
}
