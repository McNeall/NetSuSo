

using FluentAssertions;

namespace NetSuSo.NUnit.Tests;

public class SolverTestsData
{
    public static IEnumerable<TestCaseData> EasyBoards
    {
        get
        {
            yield return new TestCaseData(Board.FromEnumerable([
                    0, 5, 0, 7, 0, 3, 0, 6, 0,
                    0, 0, 7, 0, 0, 0, 8, 0, 0,
                    0, 0, 0, 8, 1, 6, 0, 0, 0,
                    0, 0, 0, 0, 3, 0, 0, 0, 0,
                    0, 0, 5, 0, 0, 0, 1, 0, 0,
                    7, 3, 0, 0, 4, 0, 0, 8, 6,
                    9, 0, 6, 0, 0, 0, 2, 0, 4,
                    8, 4, 0, 5, 7, 2, 0, 9, 3,
                    0, 0, 0, 4, 0, 9, 0, 0, 0,
                ]), Board.FromEnumerable([
                    1, 5, 8, 7, 2, 3, 4, 6, 9,
                    3, 6, 7, 9, 5, 4, 8, 2, 1,
                    2, 9, 4, 8, 1, 6, 3, 7, 5,
                    6, 1, 9, 2, 3, 8, 5, 4, 7,
                    4, 8, 5, 6, 9, 7, 1, 3, 2,
                    7, 3, 2, 1, 4, 5, 9, 8, 6,
                    9, 7, 6, 3, 8, 1, 2, 5, 4,
                    8, 4, 1, 5, 7, 2, 6, 9, 3,
                    5, 2, 3, 4, 6, 9, 7, 1, 8,
                ])).SetArgDisplayNames("easy_board_01");
        }
    }
    public static IEnumerable<TestCaseData> MediumBoards
    {
        get
        {
            yield return new TestCaseData(
                Board.FromEnumerable([
                    0, 0, 2, 0, 0, 0, 8, 0, 0,
                    0, 0, 5, 0, 2, 0, 1, 0, 0,
                    4, 6, 0, 0, 0, 0, 0, 2, 9,
                    1, 3, 0, 0, 6, 0, 0, 5, 2,
                    0, 0, 9, 0, 8, 0, 4, 0, 0,
                    0, 0, 0, 3, 0, 2, 0, 0, 0,
                    0, 0, 6, 0, 7, 0, 2, 0, 0,
                    7, 0, 0, 0, 0, 0, 0, 0, 8,
                    0, 2, 0, 5, 1, 9, 0, 7, 0,
                ]),
                Board.FromEnumerable([
                    3, 1, 2, 9, 4, 7, 8, 6, 5,
                    9, 8, 5, 6, 2, 3, 1, 4, 7,
                    4, 6, 7, 8, 5, 1, 3, 2, 9,
                    1, 3, 8, 7, 6, 4, 9, 5, 2,
                    2, 7, 9, 1, 8, 5, 4, 3, 6,
                    6, 5, 4, 3, 9, 2, 7, 8, 1,
                    5, 9, 6, 4, 7, 8, 2, 1, 3,
                    7, 4, 1, 2, 3, 6, 5, 9, 8,
                    8, 2, 3, 5, 1, 9, 6, 7, 4,
                ])).SetArgDisplayNames("medium_board_01");
        }
    }
};

[TestFixture]
public class SolverTests
{


    [TestCaseSource(typeof(SolverTestsData), nameof(SolverTestsData.EasyBoards)),
    TestCaseSource(typeof(SolverTestsData), nameof(SolverTestsData.MediumBoards))]
    public void TestBasicSolver(Board unfinishedBoard, Board expected)
    {
        Console.WriteLine("Starting Test");
        BasicSolver basicSolver = new BasicSolver(unfinishedBoard);
        Board solution = basicSolver.Solve();
        solution.Should().BeEquivalentTo(expected);
    }

}
