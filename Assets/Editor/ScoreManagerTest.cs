using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ScoreManagerTest
{
    ScoreManager _scoreManager;

    [SetUp]
    public void TestSetup()
    {
        _scoreManager = new ScoreManager();
    }

    [Test]
    public void Bowl_Strike_ReturnsEndFrame()
    {
        ScoreManager scoreManager = new ScoreManager();
        Assert.AreEqual(FrameAction.EndFrame, scoreManager.Bowl(10));
    }

    [Test]
    public void Bowl_OneThrowLessOrEqual9_ReturnsTidy()
    {
        for (int i = 0; i <= 9; i++)
        {
            ScoreManager scoreManager = new ScoreManager();
            Assert.AreEqual(FrameAction.Tidy, scoreManager.Bowl(i));
        }
    }

    [Test]
    public void Bowl_TwoThrows_ReturnsEndTurn()
    {
        for (int i = 0; i <= 10; i++)
        {
            ScoreManager scoreManager = new ScoreManager();
            scoreManager.Bowl(0);
            Assert.AreEqual(FrameAction.EndFrame, scoreManager.Bowl(i));
        }
    }

    [Test]
    public void Bowl_12Strikes_ReturnsEndGame()
    {
        //Arrange
        for (int i = 0; i < 11; i++)
        {
            _scoreManager.Bowl(10);
        }

        //Assert
        Assert.AreEqual(FrameAction.EndGame, _scoreManager.Bowl(10));
    }

    [Test]
    public void Bowl_SpareIn10Frame_ResetPins()
    {
        //Arrange
        for (int i = 0; i < 9; i++)
        {
            _scoreManager.Bowl(10);
        }

        _scoreManager.Bowl(5);


        //Assert
        Assert.AreEqual(FrameAction.ResetPins, _scoreManager.Bowl(5));
    }

    [Test]
    public void Bowl_20NonStrikeThrows_ReturnsEndGame()
    {
        //Arrange
        for (int i = 0; i < 19; i++)
        {
            _scoreManager.Bowl(1);
        }

        //Assert
        Assert.AreEqual(FrameAction.EndGame, _scoreManager.Bowl(1));
    }

    [Test]
    public void Bowl_StrikeAndAnyInLastFrame_ReturnsTidy()
    {
        //Arrange
        for (int i = 0; i < 18; i++)
        {
            _scoreManager.Bowl(1);
        }

        _scoreManager.Bowl(ScoreManager.TotalPins);
        //Assert
        Assert.AreEqual(FrameAction.Tidy, _scoreManager.Bowl(1));
    }

    [Test]
    public void Frame_AfterBowls_ReturnsExpectedValue()
    {
        //Arrange
        var items = new Tuple<int[], int>[]
        {
            new Tuple<int[], int>(new int[] { 3 },  1),
            new Tuple<int[], int>(new int[] { 3, 3 },  2),
            new Tuple<int[], int>(new int[] { 3, 3, 3 },  2),
            new Tuple<int[], int>(new int[] { 2, 8 },  2),
            new Tuple<int[], int>(new int[] { 2, 8, 10 }, 3),
            new Tuple<int[], int>(new int[] { 10, 10, 10 }, 4),
            new Tuple<int[], int>(new int[] { 0, 10, 10 }, 3)
        }
        .Select(t => new
        {
            Bowls = t.Item1,
            ExpectedFrame = t.Item2
        })
        .ToArray();

        var expectedResults = items
            .Select(i => i.ExpectedFrame);

        //Act
        var actualResults = new List<int>();
        foreach (var item in items)
        {
            ScoreManager scoreManager = new ScoreManager();
            foreach (var bowl in item.Bowls)
            {
                scoreManager.Bowl(bowl);
            }
            actualResults.Add(scoreManager.Frame);
        }

        //Assert
        CollectionAssert.AreEqual(expectedResults, actualResults);
    }

    [Test]
    public void CurrentThrow_AfterBowls_ReturnsExpectedValue()
    {
        //Arrange
        var items = new Tuple<int[], int>[]
        {
            new Tuple<int[], int>(new int[] { 3 },  1),
            new Tuple<int[], int>(new int[] { 3, 3 },  0),
            new Tuple<int[], int>(new int[] { 3, 3, 3 },  1),
            new Tuple<int[], int>(new int[] { 2, 8 },  0),
            new Tuple<int[], int>(new int[] { 2, 8, 10 }, 0),
            new Tuple<int[], int>(new int[] { 10, 10, 10 }, 0),
            new Tuple<int[], int>(new int[] { 10, 10, 10, 0 }, 1),
        }
        .Select(t => new
        {
            Bowls = t.Item1,
            ExpectedThrowFrame = t.Item2
        })
        .ToArray();

        var expectedResults = items
            .Select(i => i.ExpectedThrowFrame);

        //Act
        var actualResults = new List<int>();
        foreach (var item in items)
        {
            ScoreManager scoreManager = new ScoreManager();
            foreach (var bowl in item.Bowls)
            {
                scoreManager.Bowl(bowl);
            }
            actualResults.Add(scoreManager.CurrentThrow);
        }

        //Assert
        CollectionAssert.AreEqual(expectedResults, actualResults);
    }

    [Test]
    public void StandingPins_AfterBowl_ReturnsExpectedValue()
    {
        for (int i = 0; i <= 9; i++)
        {
            ScoreManager scoreManager = new ScoreManager();
            scoreManager.Bowl(i);

            Assert.AreEqual(10 - i, scoreManager.StandingPins);
        }
    }

    [Test]
    public void GetTotalScore_AfterBowls_ReturnsExpectedValue()
    {
        //Arrange
        var items = new Tuple<int[], int>[]
        {
            new Tuple<int[], int>(new int[] { 2 },  2),
            new Tuple<int[], int>(new int[] { 2, 3 },  5),
            new Tuple<int[], int>(new int[] { 1, 2, 3 },  6),
            new Tuple<int[], int>(new int[] { 2, 8, 1, 2 },  14),
            new Tuple<int[], int>(new int[] { 2, 8, 10, 7, 1 }, 46),
            new Tuple<int[], int>(new int[] { 10, 10, 10, 0, 1  }, 62),
            new Tuple<int[], int>(new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10}, 300),
            new Tuple<int[], int>(new int[] { 1, 2, 5, 5, 5, 4, 5, 5, 10, 9, 1, 2, 3, 4, 6, 10, 10, 9, 1 }, 153),
            new Tuple<int[], int>(new int[] { 8,1, 9,1, 10, 9,1, 10, 10, 10, 10, 9,1, 9,1,10 },  217),
        }
        .Select(t => new
        {
            Bowls = t.Item1,
            ExpectedTotalScore = t.Item2
        })
        .ToArray();

        var expectedResults = items
            .Select(i => i.ExpectedTotalScore);

        //Act
        var actualResults = new List<int>();
        foreach (var item in items)
        {
            ScoreManager scoreManager = new ScoreManager();
            foreach (var bowl in item.Bowls)
            {
                scoreManager.Bowl(bowl);
            }
            actualResults.Add(scoreManager.GetTotalScore());
        }

        //Assert
        CollectionAssert.AreEqual(expectedResults, actualResults);
    }

    [Test]
    public void GetCumulativeFrameScores_AfterBowls_ReturnsExpectedValues()
    {
        //Arrange
        var items = new Tuple<int[], int?[]>[]
        {
            new Tuple<int[], int?[]>(
                new int[] { 2 },  
                new int?[10] { null, null, null, null, null, null, null, null, null, null }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 2, 3 },
                new int?[10] { 5, null, null, null, null, null, null, null, null, null }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 1, 2, 4 },
                new int?[10] { 3, null, null, null, null, null, null, null, null, null }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 2, 8, 1, 2 },
                new int?[10] { 11, 14, null, null, null, null, null, null, null, null }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 2, 8, 10, 7, 1 },
                new int?[10] { 20, 38, 46, null, null, null, null, null, null, null }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 10, 10, 10, 0, 1 },
                new int?[10] { 30, 50, 61, 62, null, null, null, null, null, null }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10},
                new int?[10] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300 }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 1, 2, 5, 5, 5, 4, 5, 5, 10, 9, 1, 2, 3, 4, 6, 10, 10, 9, 1 },
                new int?[10] { 3, 18, 27, 47, 67, 79, 84, 104, 133, 153 }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 8,1, 9,1, 10, 9,1, 10, 10, 10, 10, 9,1, 9,1,10 },
                new int?[10] { 9, 29, 49, 69, 99, 129, 158, 178, 197, 217 }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1 },
                new int?[10] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 }
                ),
            new Tuple<int[], int?[]>(
                new int[] { 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 9,1 },
                new int?[10] { 2, 4, 6, 8, 10, 12, 14, 16, 18, null }
                ),
        }
        .Select(t => new
        {
            Bowls = t.Item1,
            ExpectedCumulativeScores = t.Item2
        })
        .ToArray();

        var expectedResults = items
            .Select(i => i.ExpectedCumulativeScores)
            .ToArray();

        //Act
        var actualResults = new List<int?[]>();
        foreach (var item in items)
        {
            ScoreManager scoreManager = new ScoreManager();
            foreach (var bowl in item.Bowls)
            {
                scoreManager.Bowl(bowl);
            }
            actualResults.Add(scoreManager.GetCumulativeFrameScores());
        }

        //Assert
        for (int i = 0; i < expectedResults.Length; i++)
        {
            CollectionAssert.AreEqual(expectedResults[i], actualResults[i], $"Collection index: {i}");
        }
    }

    [Test]
    public void GetThrowsScores_AfterBowls_ReturnsExpectedValues()
    {
        //Arrange
        var items = new Tuple<int[], string[]>[]
        {
            new Tuple<int[], string[]>(
                new int[] { 2 },
                new string[] 
                {
                    "2", null, null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null
                }),
            new Tuple<int[], string[]>(
                new int[] { 2, 3 },
                new string[]
                {
                    "2", "3", null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null
                }),
            new Tuple<int[], string[]>(
                new int[] { 1, 2, 4 },
                new string[]
                {
                    "1", "2", "4", null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                }),
            new Tuple<int[], string[]>(
                new int[] { 2, 8, 1, 2 },
                new string[]
                {
                    "2", "/", "1", "2", null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                }),
            new Tuple<int[], string[]>(
                new int[] { 2, 8, 10, 7, 1 },
                new string[]
                {
                    "2", "/", "X", null, "7",
                    "1", null, null, null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                }),
            new Tuple<int[], string[]>(
                new int[] { 10, 10, 10, 0, 1 },
                new string[]
                {
                    "X", null, "X", null, "X",
                    null, "0", "1", null, null,
                    null, null, null, null, null,
                    null, null, null, null, null,
                }),
            new Tuple<int[], string[]>(
                new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10},
                new string[]
                {
                    "X", null, "X", null, "X", null, "X", null, "X", null,
                    "X", null, "X", null, "X", null, "X", null, "X", "X", "X"
                }),
            new Tuple<int[], string[]>(
                new int[] { 1, 2, 5, 5, 5, 4, 5, 5, 10, 9, 1, 2, 3, 4, 6, 10, 10, 9, 1 },
                new string[]
                {
                    "1", "2", "5", "/", "5",
                    "4", "5", "/", "X", null,
                    "9", "/", "2", "3", "4", "/", "X", null, "X", "9", "1",
                }),
            new Tuple<int[], string[]>(
                new int[] { 8,1, 9,1, 10, 9,1, 10, 10, 10, 10, 9,1, 9,1,10 },
                new string[]
                {
                    "8","1", "9","/", "X", null, "9","/", "X", null, "X", null, "X", null, "X", null, "9","/", "9","/", "X"
                }),
            new Tuple<int[], string[]>(
                new int[] { 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1, 1,1 },
                new string[]
                {
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                    "1", "1", "1", "1", "1",
                }),
        }
        .Select(t => new
        {
            Bowls = t.Item1,
            ExpectedThrowResults = t.Item2
        })
        .ToArray();

        var expectedResults = items
            .Select(i => i.ExpectedThrowResults)
            .ToArray();

        //Act
        var actualResults = new List<string[]>();
        foreach (var item in items)
        {
            ScoreManager scoreManager = new ScoreManager();
            foreach (var bowl in item.Bowls)
            {
                scoreManager.Bowl(bowl);
            }
            actualResults.Add(scoreManager.GetThrowResults());
        }

        //Assert
        for (int i = 0; i < expectedResults.Length; i++)
        {
            CollectionAssert.AreEqual(expectedResults[i], actualResults[i], $"Collection index: {i}");
        }
    }

    [Test]
    public void Reset_ExpectedPropertiesAreReset()
    {
        //Arrange
        for (int i = 0; i < 7; i++)
        {
            _scoreManager.Bowl(3);
        }
        //Act
        _scoreManager.Reset();
        //Assert
        Assert.AreEqual(1, _scoreManager.Frame);
        Assert.AreEqual(0, _scoreManager.CurrentThrow);
        Assert.AreEqual(10, _scoreManager.StandingPins);
        Assert.AreEqual(0, _scoreManager.GetTotalScore());
    }
}