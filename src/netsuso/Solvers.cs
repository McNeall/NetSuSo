using System.Collections.Immutable;

namespace NetSuSo;

/**
<summary>A basic Sudoku solver using a brute force backtracking algorithm.</summary>

<remarks>
This solver uses backtracking for the empty cells. Cells are processed left to right, top to
bottom. It always picks the next higher valid value. The algorithm stops in case one valid
solution is found. There is no check if there is another valid solution.
</remarks>
*/
public class BasicSolver
{

    private readonly Board _board;

    /**
    <summary>Creates a solver for the given <paramref name="board"/>.</summary>

    <param name="board">Board that should be solved.</param>
    */
    public BasicSolver(Board board)
    {
        _board = board;
    }

    /**
    <summary>Search a valid solution of the board and returns the filled board.</summary>

    <remarks>The solver modifies the passed board while searching a solution.</remarks>

    <returns>The initially passed board instance modified to hold a solution if any exists</returns>
    <exception cref="BoardNotSolvableException">description</exception>
    */
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
            ISet<int> remainingValues = Board.ValidValues.Except(Enumerable.Range(1, currentValue)).OrderBy(ele => ele).ToImmutableSortedSet();
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

