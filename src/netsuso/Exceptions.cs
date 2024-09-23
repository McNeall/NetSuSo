namespace NetSuSo;


/**
<summary>Raised in case a value is not allowed in a cell.</summary>
*/
public class InvalidCellValueException : Exception
{
    private readonly Coordinate? _coordinate;
    private readonly int _value;

    public InvalidCellValueException() : base() { }
    public InvalidCellValueException(string message) : base(message) { }

    public InvalidCellValueException(string message, Exception inner) : base(message, inner) { }

    /**
    <summary>Store the invalid value for the given coordinate and append them to the error message.</summary>

    <param name="message">Message explaining the error.</param>
    <param name="coordinate">Coordinate for which <paramref name="value"/> is invalid at the time the exception was raised.</param>
    <param name="value">The value invalid for <paramref name="coordinate"/> at the time the exception was raised.</param>
    */
    public InvalidCellValueException(string message, Coordinate coordinate, int value) : base(message)
    {
        _coordinate = coordinate;
        _value = value;
    }

    public override string Message
    {
        get
        {
            string s = base.Message;
            if (_coordinate != null)
            {
                s += $" {_coordinate}: {_value}";
            }
            return s;
        }

    }
}


/**
<summary>Raised in case the board does not have a solution.</summary>
*/
public class BoardNotSolvableException : Exception
{
    public BoardNotSolvableException() : base() { }
    public BoardNotSolvableException(string message) : base(message) { }

    public BoardNotSolvableException(string message, Exception inner) : base(message, inner) { }

}
