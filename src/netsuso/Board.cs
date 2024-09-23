using System.Collections;
using System.Collections.Frozen;
using System.Text;
using System.Globalization;

namespace NetSuSo;

/**
<summary>Represents a coordinate on a board.</summary>
*/
public record struct Coordinate
{
    private const int IndexLowerBound = 0;
    private const int IndexUpperBound = 8;

    /**
    <summary>The row index of the coordiante</summary>
    <value>A value between zero and eight, both included.</value>
    */
    public int Row { get; init; }

    /**
    <summary>The column index of the coordiante</summary>
    <value>A value between zero and eight, both included.</value>
    */
    public int Column { get; init; }


    /**
    <summary>Create a coordinate from the given row and column indices if they are valid.</summary>

    <param name="row">Row index of the coordinate. Needs to be between 0 and 8, both included.</param>
    <param name="column">Column index of the coordinate. Needs to be between 0 and 8, both included.</param>

    <exception cref="ArgumentOutOfRangeException">If row or column index are invalid. Indices are invalid in case they are not between zero and 8, both included.</exception>

    */
    public Coordinate(int row, int column)
    {
        Predicate<int> isInvalid = value => value < IndexLowerBound || value > IndexUpperBound;

        string messageTemplate = $"Expecting a value betwenn {IndexLowerBound} and {IndexUpperBound}.";
        if (isInvalid(row)) throw new ArgumentOutOfRangeException(nameof(row), row, messageTemplate);
        if (isInvalid(column)) throw new ArgumentOutOfRangeException(nameof(column), column, messageTemplate);
        this.Row = row;
        this.Column = column;
    }
}

/**
<summary>Link a <see cref="Coordinate"/> to a value on the board.</summary>

<param name="Coordinate">The <see cref="Coordinate"/> to which <paramref name="Value"/> belongs.</param>
<param name="Value">The value set on the board at <paramref name="Coordinate"/>.</param>

*/
public record struct Cell(Coordinate Coordinate, int Value)
{
    /**
    <value>Coordiante on which <see cref="Value"/> is set.</value>
    */
    public Coordinate Coordinate { get; init; } = Coordinate;

    /**
    <value>Value at <see cref="Coordinate"/>.</value>
    */
    public int Value { get; init; } = Value;

}

/**
<summary>Models a possibly unfinished Sudoku board.</summary>
*/
public class Board : IEnumerable<Cell>
{
    public const int SquareSize = 3;
    public const int BoardDim = SquareSize * SquareSize;
    public const int TotalElements = BoardDim * BoardDim;

    public static readonly FrozenSet<int> ValidValues = Enumerable.Range(1, 9).ToFrozenSet();

    private readonly int[] values = new int[TotalElements];
    private readonly FrozenSet<Coordinate> initialEmptyCells;

    /**
    <summary>Indexing based on a <see cref="Coordinate"/></summary>
    <returns>Returns the value currently set on <paramref name="coordinate"/> for the <see cref="Board"/></returns>
    */
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
                                 .Select(e => new Coordinate(e.Index / BoardDim, e.Index % BoardDim))
                                 .ToFrozenSet();
    }

    /**
    <summary>Return a new Board based on the passed values.</summary>

    <remarks>Each value in the passed array represents one value on the board. Allowed values are zero to 9. The value zero marks an empty field.</remarks>
    <param name="input">A two dimensional array representing values. The first coordinate represents the row, the second the column. Needs to have exactly nine row entries, with nice column entries each.</param>
    <returns>Instance of <see cref="Board"/> initialized with the passed values.</returns>
    <exception cref="ArgumentException">Raised in the following cases:
        <list type="bullet">
            <item><description>If the outter list has less than nine inner lists.</description></item>
            <item><description>If an inner list has less than nine elements.</description></item>
            <item><description>If the values are not between zero and nine.</description></item>
        </list>
    </exception>
    */

    public static Board FromMatrix(
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1814", Justification = "The board is only valid in case each row has the same number of columns. A multidimensinal array enforces that, while a jagged array does not. Since we expect a same number of elements for each row a jagged array does not save memory.")]
        int[,] input)
    {
        ArgumentNullException.ThrowIfNull(input);
        int rows = input.GetLength(0);
        int columns = input.GetLength(1);

        if (rows != BoardDim) throw new ArgumentException($"Expected exactly {BoardDim} rows, but found {rows}.", nameof(input));
        if (columns != BoardDim) throw new ArgumentException($"Expected exactly {BoardDim} columns, but found {columns}.", nameof(input));
        Dictionary<Coordinate, int> invalidPositions = new();
        int[] flattendValues = new int[BoardDim * BoardDim];
        int currentValue;
        for (int row = 0; row < BoardDim; row++)
        {
            for (int column = 0; column < BoardDim; column++)
            {
                currentValue = input[row, column];
                if (0 <= currentValue && currentValue <= 9)
                {
                    flattendValues[row * BoardDim + column] = currentValue;
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

    /**
    <summary>Return a new Board based on the passed values.</summary>

    <remarks>The enumerable needs to have exactly 81 values. Allowed values are zero to 9. The value zero marks an empty field.</remarks>
    <param name="input">An enumerable with exactly 81 values. The values are set from left to right, top to bottom.</param>
    <returns>Instance of <see cref="Board"/> initialized with the passed values.</returns>
    <exception cref="ArgumentException">Raised in the following cases:
        <list type="bullet">
            <item><description>If <paramref name="input"/> does not have exactly 81 elements</description></item>
            <item><description>If elements of <paramref name="input"/> are not between 0 and 9, both included.</description></item>
        </list>
    </exception>
    */
    public static Board FromEnumerable(IEnumerable<int> input)
    {
        Dictionary<int, int> invalidPositions = new();
        int length = 0;
        foreach (var indexValue in input.Select((e, i) => (Value: e, Index: i)))
        {
            if (indexValue.Value < 0 || 9 < indexValue.Value)
            {
                invalidPositions[indexValue.Index] = indexValue.Value;
            }
            length = indexValue.Index;
        }
        if (length != BoardDim * BoardDim) throw new ArgumentException($"Expected exactly {BoardDim * BoardDim} characters, but found {length}.", nameof(input));
        if (invalidPositions.Count > 0)
        {
            string invalidPositionList = string.Join("\n", invalidPositions.Select(e => $"{e.Key}: {e.Value}"));
            throw new ArgumentException($"Found invalid values at the following positions (Position: Value):\n{invalidPositionList}", nameof(input));
        }
        return new Board(input);
    }



    /**
    <summary>Return a new Board based on the passed values.</summary>

    <remarks>The passed string needs to have exactly 81 characters. Each character being an integer.</remarks>
    <param name="input">The values used to create the board.</param>
    <returns>Instance of <see cref="Board"/> initialized with the passed values.</returns>
    <exception cref="ArgumentException">Raised in the following cases:
        <list type="bullet">
            <item><description>If <paramref name="input"/> does not have exactly 81 characters</description></item>
            <item><description>If there is a character that is not convertible to an integer</description></item>
            <item><description>If the converted characters are not integers between zero and nine</description></item>
        </list>
    </exception>
    */
    public static Board FromString(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.Length != BoardDim * BoardDim) throw new ArgumentException($"Expected exactly 81 characters, but found {input.Length}.", nameof(input));
        Dictionary<int, string> invalidPositions = new();
        int[] values = new int[BoardDim * BoardDim];
        // Allow indexing into the string without allocating new memory
        ReadOnlySpan<char> inputSlice = input.AsSpan();
        for (int i = 0; i < values.Length; i++)
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


    /**
    <summary>Return the allowed but unused values for the column given by <paramref name="columnIndex"/></summary>

    <param name="columnIndex">The column index specifying the column for which the values should be retrieved. The index is zero based, hence the allowed values are between zero and eight, both included.</param>

    <returns>Values available for the column identified by the passed <paramref name="columnIndex"/>.</returns>

    <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is less than zero or greater than eight.</exception>
    */
    public FrozenSet<int> AvailableColumnValues(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= BoardDim)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, $"Expecting a value betwenn 0 and {BoardDim - 1}.");
        }
        HashSet<int> valuesInColumn = new HashSet<int>();
        for (int i = columnIndex; i < TotalElements; i += BoardDim)
        {
            valuesInColumn.Add(values[i]);
        }
        return ValidValues.Except(valuesInColumn).ToFrozenSet();
    }

    /**
    <summary>Return the allowed but unused values for the row given by <paramref name="rowIndex"/></summary>

    <param name="rowIndex">The row index specifying the row for which the values should be retrieved. The index is zero, hence the allowed values are between zero and eight, both included.</param>
    <returns>Values available for the row indentified by the passed <paramref name="rowIndex"/>.</returns>
    <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is less than zero or greater than eight.</exception>
    */
    public FrozenSet<int> AvailableRowValues(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= BoardDim)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, $"Expecting a value betwenn 0 and {BoardDim - 1}.");
        }
        int fromIndex = rowIndex * BoardDim;
        int toIndex = fromIndex + BoardDim;
        ISet<int> valuesInRow = new HashSet<int>(values[fromIndex..toIndex]);
        return ValidValues.Except(valuesInRow).ToFrozenSet();

    }

    /**
    <summary>Return the allowed but unused values of the 3x3 square that contains <paramref name="coordinate"/></summary>

    <remarks>The board is separated in in 3x3 squares starting from the top left cell at (0, 0). Every three rows and columns a new square starts.</remarks>

    <param name="coordinate">Coordinate used to determine the square for which the values should be returned.</param>
    <returns>Values available for the 3x3 square the given <paramref name="coordinate"/> is located in.</returns>
    */
    public FrozenSet<int> AvailableSquareValues(Coordinate coordinate)
    {
        Coordinate topLeftCell = new Coordinate(coordinate.Row - coordinate.Row % SquareSize, coordinate.Column - coordinate.Column % SquareSize);
        HashSet<int> valuesInSquare = new HashSet<int>();
        foreach (var row in Enumerable.Range(topLeftCell.Row, 3))
        {
            int firstIndexInSquareForRow = row * BoardDim + topLeftCell.Column;
            valuesInSquare.UnionWith(values[firstIndexInSquareForRow..(firstIndexInSquareForRow + SquareSize)]);

        }
        return ValidValues.Except(valuesInSquare).ToFrozenSet();
    }


    /**
    <summary>Check that <paramref name="value"/> is valid for <paramref name="coordinate"/> based on the current state of the board.</summary>

    <remarks>A value is seen as valid if all of the following cases hold:
        <list type="bullet">
            <item><description>Value is between 1 and 9 both included.</description></item>
            <item><description>Value is not yet present in the row the cell belongs to. This does not include zeros. The value zero is allowed multiple times.</description></item>
            <item><description>Value is not yet present in the column the cell belongs to. This does not include zeros. The value zero is allowed multiple times.</description></item>
            <item><description>Value is not yet present in the 3x3 square the cell is in. This does not include zeros.The value zero is allowed multiple times.</description></item>
        </list>
    </remarks>
    <param name="coordinate">Position on the board.</param>
    <param name="value">Value for position given by <paramref name="coordinate"/>.</param>
    <returns><c>True</c> if value is valid for <paramref name="coordinate"/> otherwise <c>False</c>.</returns>
    */
    public bool IsValid(Coordinate coordinate, int value)
    {
        if (!ValidValues.Contains(value)) return false;
        if (AvailableRowValues(coordinate.Row).Contains(value) && AvailableColumnValues(coordinate.Column).Contains(value) && AvailableSquareValues(coordinate).Contains(value)) return true;
        return false;
    }

    private static int CalculateIndex(Coordinate coordinate)
    {
        return coordinate.Row * BoardDim + coordinate.Column;
    }

    /**
    <summary>Enumerator to iterate through the board left to right, top to bottom.</summary>

    <remarks>The returned enumerator provides a <see cref="Cell"/>, which holds a <see cref="Coordinate"/> and value.</remarks>

    <returns>The <see cref="Cell"/> gives access to the <see cref="Coordinate"/> and value of the current element</returns>
    */
    public IEnumerator<Cell> GetEnumerator()
    {
        return new BoardEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /**
    <summary>Implementation of IEnumerator dealing allowing to iterate over <see cref="Board"/></summary>

    <remarks>The board is passed to the Enumerator when it is instantiated. Changing the board during iteration is reflected in the iterator. The number of elements a Board has is fixed</remark
    */
    private class BoardEnumerator : IEnumerator<Cell>
    {
        private readonly Board _board;
        private int _currentIndex;

        /**
        <summary>Current value of the enumerator</summary>
        <value>A <see cref="Cell"/> holding the current <see cref="Coordinate"/> and the related value.</value>
        */
        public Cell Current
        {
            get
            {
                _currentIndex++;
                return new Cell(new Coordinate(_currentIndex / BoardDim, _currentIndex % BoardDim), _board.values[_currentIndex]);
            }
        }

        object IEnumerator.Current => Current;

        /**
        <summary>Creates a enumerator based on the passed board</summary>
        <param name="board">The board that should be enumerated</param>
        */
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
            return _currentIndex < TotalElements - 1;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }
    }
}



class BoardFormatter
{

    private static readonly CompositeFormat UnicodeTemplate = CompositeFormat.Parse("""
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
    """);

    public static string UnicodeRepresentation(Board board)
    {
        return string.Format(CultureInfo.InvariantCulture, UnicodeTemplate, board.Select(x => x.Value.ToString("D1", CultureInfo.InvariantCulture)).ToArray());
    }

}
