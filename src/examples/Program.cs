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
