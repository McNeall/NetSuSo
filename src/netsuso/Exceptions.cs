namespace NetSuSo;

public class InvalidCellValueException : Exception
{
    private readonly Coordinate? _coordinate;
    private readonly int _value;

    public InvalidCellValueException() : base() { }
    public InvalidCellValueException(string message) : base(message) { }

    public InvalidCellValueException(string message, Exception inner) : base(message, inner) { }

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

public class BoardNotSolvableException : Exception
{
    public BoardNotSolvableException() : base() { }
    public BoardNotSolvableException(string message) : base(message) { }

    public BoardNotSolvableException(string message, Exception inner) : base(message, inner) { }

}
