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
    public void TotalScore_AfterBowls_ReturnsExpectedValue()
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
            actualResults.Add(scoreManager.TotalScore);
        }

        //Assert
        CollectionAssert.AreEqual(expectedResults, actualResults);
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
        Assert.AreEqual(0, _scoreManager.TotalScore);
    }

    [Test]
    public void GetFrameResult_ReturnsExpectedFrameResult()
    {
        //Arrange
        var bowls = new int[]
        {
            1,
            3,
            10,
            7,
            3
        };

        var expectedResults = new FrameResult[]
        {
            new FrameResult( 1, 3),
            new FrameResult(10),
            new FrameResult(7, 3),
        };

        foreach (var bowl in bowls)
        {
            _scoreManager.Bowl(bowl);
        }

        //Assert
        for (int i = 0; i < expectedResults.Length; i++)
        {
            Assert.AreEqual(expectedResults[i].Bonus, _scoreManager.GetFrameResult(i + 1).Bonus);
            CollectionAssert.AreEqual(expectedResults[i].Scores, _scoreManager.GetFrameResult(i + 1).Scores);
        }
    }
}