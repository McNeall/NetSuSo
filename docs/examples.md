The solver provided by NetSuSo expects an instance of `NetSuSo.Board`. A `Board` instance should be created
by one of the factory methods.

## The `FromMatrix` factory method

To create a board with the the  `FromMatrix` factory method requires a two dimensional array. Each dimension needs to have exactly nine elements. Allowed values are integers between zero and nine both included.

```c#
using NetSuSo;

int[,] matrixValues = new int[9, 9] {
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

Board matrixValuesBoard = Board.FromMatrix(matrixValues);
BasicSolver matrixValuesSolver = new BasicSolver(matrixValuesBoard);
matrixValuesSolver.Solve();
Console.WriteLine(matrixValuesBoard.UnicodeRepresentation());
```

## The `FromEnumerable` factory method

To create a board with the the  `FromEnumerable` factory method requires an IEnumberable with 81 elements,
each an integer between zero to nine. The following code shows an example:

```c#
using NetSuSo;

int[] enumerableValues = [
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
Board enumberableValuesBoard = Board.FromEnumerable(enumerableValues);
BasicSolver enumberableValuesSolver = new BasicSolver(enumberableValuesBoard);
enumberableValuesSolver.Solve();
Console.WriteLine(enumberableValuesBoard.UnicodeRepresentation());
```

## The `FromString` factory method

To create a board with the the  `FromString` factory method requires a string with 81 elements,
each an integer between zero to nine. The examples above can be modified as follows:

```c#
using NetSuSo;

string stringValues = "050703060" +
                      "007000800" +
                      "000816000" +
                      "000030000" +
                      "005000100" +
                      "730040086" +
                      "906000204" +
                      "840572093" +
                      "000409000";

Board stringValuesBoard = Board.FromString(stringValues);
BasicSolver stringValuesSolver = new BasicSolver(stringValuesBoard);
stringValuesSolver.Solve();
Console.WriteLine(stringValuesBoard.UnicodeRepresentation());
```
