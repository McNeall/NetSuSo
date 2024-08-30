

using System.Collections.Immutable;
using System.Dynamic;

using NetSuSo;

class BasicSolver
{

    private readonly Board _board;
    public BasicSolver(Board board)
    {
        _board = board;
    }

    public Board Solve()
    {
        Coordinate[] unsetCells = _board.Where(cell => cell.Value == 0).Select(cell => cell.Coordinate).Reverse().ToArray();
        // TODO: Check order of stack elements
        Stack<Coordinate> remainingCells = new(unsetCells);
        Stack<Coordinate> history = new();

        // TODO: Remove or just peek?
        while (remainingCells.TryPeek(out Coordinate currentCoordiante))
        {
            int currentValue = _board[currentCoordiante];
            ISet<int> remainingValues = Board.VALID_VALUES.Except(Enumerable.Range(1, currentValue)).OrderBy(ele => ele).ToImmutableSortedSet();
            if (SetNextAllowedValueForCoordinate(currentCoordiante, remainingValues))
            {
                history.Push(remainingCells.Pop());
            }
            else
            {
                // Use the indexer, because this operation is excepted to never fail.
                _board[currentCoordiante] = 0;
                if (history.TryPop(out Coordinate lastSetCoordinate)) remainingCells.Push(lastSetCoordinate);
                else throw new BoardNotSolvableException("No valid solution found.");
            }

        }
        return _board;
    }

    private bool SetNextAllowedValueForCoordinate(Coordinate coordinate, ISet<int> remainingValues)
    {
        foreach (var element in remainingValues)
        {
            if (_board.TrySetValue(coordinate, element)) return true;
        }
        return false;
    }
}

