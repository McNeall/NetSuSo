using System.Collections;
using System.Collections.Frozen;
using System.Dynamic;
using System.Text;

namespace NetSuSo;


public record struct Coordinate
{
    private static readonly int INDEX_LOWER_BOUND = 0;
    private static readonly int INDEX_UPPER_BOUND = 8;

    public int Row { get; init; }
    public int Column { get; init; }

    public Coordinate(int row, int column)
    {
        Predicate<int> isInvalid = value => value < INDEX_LOWER_BOUND || value > INDEX_UPPER_BOUND;
        string messageTemplate = $"Argument has to be between {INDEX_LOWER_BOUND} and {INDEX_UPPER_BOUND} both included.";
        if (isInvalid(row)) throw new ArgumentOutOfRangeException(nameof(row), row, messageTemplate);
        if (isInvalid(column)) throw new ArgumentOutOfRangeException(nameof(column), column, messageTemplate);
        this.Row = row;
        this.Column = column;
    }
}

public record struct Cell(Coordinate Coordinate, int Value)
{

}

public class Board : IEnumerable<Cell>
{
    public static readonly int SQUARE_SIZE = 3;
    public static readonly int BOARD_DIM = SQUARE_SIZE * SQUARE_SIZE;
    public static readonly int TOTAL_ELEMENTS = BOARD_DIM * BOARD_DIM;

    public static readonly FrozenSet<int> VALID_VALUES = Enumerable.Range(1, 9).ToFrozenSet();

    private readonly int[] values = new int[TOTAL_ELEMENTS];
    private readonly FrozenSet<Coordinate> initialEmptyCells;

    // public FrozenSet<int> ValidValues => VALID_VALUES;

    public int this[Coordinate coordinate]
    {
        get
        {
            return values[CalculateIndex(coordinate)];
        }

        set
        {
            bool result = TrySetValue(coordinate, value);
            if (!result) throw new InvalidCellValueException("Invalid value for coordinate.", coordinate, value);
        }
    }

    public bool TrySetValue(Coordinate coordinate, int value)
    {
        bool validForEmptyCell = value == 0 && initialEmptyCells.Contains(coordinate);
        bool valid_for_cell = IsValid(coordinate, value);
        if (validForEmptyCell || valid_for_cell)
        {
            values[CalculateIndex(coordinate)] = value;
            return true;
        }
        return false;
    }

    private Board(IEnumerable<int> field)
    {
        this.values = field.ToArray();
        initialEmptyCells = field.Select((v, i) => (Value: v, Index: i))
                                 .Where(e => e.Value == 0)
                                 .Select(e => new Coordinate(e.Index / BOARD_DIM, e.Index % BOARD_DIM))
                                 .ToFrozenSet();
    }

    public static Board FromMatrix(int[,] input)
    {
        int rows = input.GetLength(0);
        int columns = input.GetLength(1);

        if (rows != BOARD_DIM) throw new ArgumentException($"Expected exactly {BOARD_DIM} rows, but found {rows}.", nameof(input));
        if (columns != BOARD_DIM) throw new ArgumentException($"Expected exactly {BOARD_DIM} columns, but found {columns}.", nameof(input));
        Dictionary<Coordinate, int> invalidPositions = new();
        int[] flattendValues = new int[BOARD_DIM * BOARD_DIM];
        int currentValue;
        for (int row = 0; row < BOARD_DIM; row++)
        {
            for (int column = 0; column < BOARD_DIM; column++)
            {
                currentValue = input[row, column];
                if (0 <= currentValue && currentValue <= 9)
                {
                    flattendValues[row * BOARD_DIM + column] = currentValue;
                }
                else
                {
                    invalidPositions.Add(new Coordinate(row, column), currentValue);
                }

            }
        }
        if (invalidPositions.Count > 0)
        {
            string invalidPositionList = string.Join("\n", invalidPositions.Select(e => $"{e.Key}: {e.Value}"));
            throw new ArgumentException($"Found invalid values at the following positions (Position: Value):\n{invalidPositionList}", nameof(input));
        }
        return new Board(flattendValues);
    }

    public static Board FromEnumerable(IEnumerable<int> input)
    {
        int lenght = input.Count();
        if (lenght != BOARD_DIM * BOARD_DIM) throw new ArgumentException($"Expected exactly {BOARD_DIM * BOARD_DIM} characters, but found {lenght}.", nameof(input));
        Dictionary<int, int> invalidPositions = new();
        foreach (var indexValue in input.Select((e, i) => (Value: e, Index: i)))
        {
            if (indexValue.Value < 0 || 9 < indexValue.Value)
            {
                invalidPositions[indexValue.Index] = indexValue.Value;
            }
        }
        if (invalidPositions.Count > 0)
        {
            string invalidPositionList = string.Join("\n", invalidPositions.Select(e => $"{e.Key}: {e.Value}"));
            throw new ArgumentException($"Found invalid values at the following positions (Position: Value):\n{invalidPositionList}", nameof(input));
        }
        return new Board(input);
    }

    public static Board FromString(string input)
    {
        if (input.Length != BOARD_DIM * BOARD_DIM) throw new ArgumentException($"Expected exactly 81 characters, but found {input.Length}.", nameof(input));
        Dictionary<int, string> invalidPositions = new();
        int[] values = new int[BOARD_DIM * BOARD_DIM];
        // Allow indexing into the string without allocating new memory
        ReadOnlySpan<char> inputSlice = input.AsSpan();
        for (int i = 0; i < input.Length; i++)
        {
            if (!int.TryParse(inputSlice[i..(i + 1)], out values[i]))
            {
                invalidPositions.Add(i, input[i].ToString());
            }
        }

        if (invalidPositions.Count > 0)
        {
            string invalidPositionList = string.Join("\n", invalidPositions.Select(e => $"{e.Key}: {e.Value}"));
            throw new ArgumentException($"Found invalid values at the following positions (Position: Value):\n{invalidPositionList}", nameof(input));
        }

        return new Board(values);
    }

    public FrozenSet<int> AvailableColumnValues(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= BOARD_DIM)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"Expecting a value betwenn 0 and {BOARD_DIM - 1}.");
        }
        ISet<int> valuesInColumn = new HashSet<int>();
        for (int i = columnIndex; i < TOTAL_ELEMENTS; i += BOARD_DIM)
        {
            valuesInColumn.Add(values[i]);
        }
        return VALID_VALUES.Except(valuesInColumn).ToFrozenSet();
    }


    public FrozenSet<int> AvailableRowValues(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= BOARD_DIM)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, $"Expecting a value betwenn 0 and {BOARD_DIM - 1}.");
        }
        int fromIndex = rowIndex * BOARD_DIM;
        int toIndex = fromIndex + BOARD_DIM;
        ISet<int> valuesInRow = new HashSet<int>(values[fromIndex..toIndex]);
        return VALID_VALUES.Except(valuesInRow).ToFrozenSet();

    }

    public FrozenSet<int> AvailableSquareValues(Coordinate coordinate)
    {
        Coordinate topLeftCell = new Coordinate(coordinate.Row - coordinate.Row % SQUARE_SIZE, coordinate.Column - coordinate.Column % SQUARE_SIZE);
        ISet<int> valuesInSquare = new HashSet<int>();
        foreach (var row in Enumerable.Range(topLeftCell.Row, 3))
        {
            int firstIndexInSquareForRow = row * BOARD_DIM + topLeftCell.Column;
            valuesInSquare.UnionWith(values[firstIndexInSquareForRow..(firstIndexInSquareForRow + SQUARE_SIZE)]);

        }
        return VALID_VALUES.Except(valuesInSquare).ToFrozenSet();
    }

    public FrozenSet<int> AvailableSquareValues(int row, int column)
    {
        return AvailableSquareValues(new Coordinate(row, column));
    }

    public bool IsValid(Coordinate coordinate, int value)
    {
        if (!VALID_VALUES.Contains(value)) return false;
        if (AvailableRowValues(coordinate.Row).Contains(value) && AvailableColumnValues(coordinate.Column).Contains(value) && AvailableSquareValues(coordinate).Contains(value)) return true;
        return false;
    }

    private int CalculateIndex(Coordinate coordinate)
    {
        return coordinate.Row * BOARD_DIM + coordinate.Column;
    }

    public IEnumerator<Cell> GetEnumerator()
    {
        return new BoardEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Nested to allow access to private methods of Board
    private class BoardEnumerator : IEnumerator<Cell>
    {
        private readonly Board _board;
        private int _currentIndex;


        public Cell Current
        {
            get
            {
                _currentIndex++;
                return new Cell(new Coordinate(_currentIndex / BOARD_DIM, _currentIndex % BOARD_DIM), _board.values[_currentIndex]);
            }
        }

        object IEnumerator.Current => Current;

        public BoardEnumerator(Board board)
        {
            _board = board;
            _currentIndex = -1;
        }

        public void Dispose()
        {
            // Nothing to do here
        }

        public bool MoveNext()
        {
            return _currentIndex < TOTAL_ELEMENTS - 1;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }
    }
}



static class BoardFormatter
{

    private static readonly string UNICODE_TEMAPLATE = """
    ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐
    │ {0} │ {1} │ {2} │ {3} │ {4} │ {5} │ {6} │ {7} │ {8} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {9} │ {10} │ {11} │ {12} │ {13} │ {14} │ {15} │ {16} │ {17} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {18} │ {19} │ {20} │ {21} │ {22} │ {23} │ {24} │ {25} │ {26} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {27} │ {28} │ {29} │ {30} │ {31} │ {32} │ {33} │ {34} │ {35} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {36} │ {37} │ {38} │ {39} │ {40} │ {41} │ {42} │ {43} │ {44} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {45} │ {46} │ {47} │ {48} │ {49} │ {50} │ {51} │ {52} │ {53} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {54} │ {55} │ {56} │ {57} │ {58} │ {59} │ {60} │ {61} │ {62} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {63} │ {64} │ {65} │ {66} │ {67} │ {68} │ {69} │ {70} │ {71} │
    ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
    │ {72} │ {73} │ {74} │ {75} │ {76} │ {77} │ {78} │ {79} │ {80} │
    └───┴───┴───┴───┴───┴───┴───┴───┴───┘
    """;

    public static string UnicodeRepresentation(Board board)
    {
        return string.Format(UNICODE_TEMAPLATE, board.Select(x => x.ToString()).ToArray());
    }

}
