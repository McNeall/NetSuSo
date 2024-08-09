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
        string messageTemplate = $"Argument '{{0}}' has to be between {INDEX_LOWER_BOUND} and {INDEX_UPPER_BOUND} both included.";
        if (isInvalid(row)) throw new ArgumentOutOfRangeException(string.Format(messageTemplate, nameof(row)));
        if (isInvalid(column)) throw new ArgumentOutOfRangeException(string.Format(messageTemplate, nameof(column)));
        this.Row = row;
        this.Column = column;
    }
}

public class Board
{
}
