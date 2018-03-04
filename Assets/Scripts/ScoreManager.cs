using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum FrameAction
{
    Tidy,
    EndFrame,
    ResetPins,
    EndGame
}

public class ScoreManager
{
    public event Action<FrameAction> Bowled;

    public const int TotalPins = 10;
    const int lastFrame = 10;

    private int _frame = 1;
    private int _standingPins = TotalPins;
    private FrameResult[] _frames = new FrameResult[lastFrame];
    private List<int> _currentFrameScores = new List<int>();
    
    #region Properties
    public int Frame
    {
        get { return _frame; }
        private set { OnFrameChanged(value); }
    }

    public int CurrentThrow => _currentFrameScores.Count;

    public int StandingPins
    {
        get { return _standingPins; }
        set
        {
            if (value < 0 || value > TotalPins)
                Debug.LogError("Invalid number of pins.");
            _standingPins = value;
        }
    }
    #endregion

    #region Public Methods
    public FrameAction Bowl(int pins)
    {
        StandingPins -= pins;
        _currentFrameScores.Add(pins);
        SaveFrameResult();

        FrameAction frameAction;
        //Handle last frame
        if (Frame == lastFrame)
        {
            frameAction = HandleLastFrame();
        }
        //Finish frame if second throw or strike.
        else if (CurrentThrow == 2 || StandingPins == 0)
        {
            frameAction = FinishFrame();
        }
        //Otherwise return Tidy
        else
        {
            frameAction = FrameAction.Tidy;
        }
        Bowled?.Invoke(frameAction);
        return frameAction;
    }

    public void Reset()
    {
        Frame = 1;
        _frames = new FrameResult[lastFrame];
    }

    public int?[] GetCumulativeFrameScores()
    {
        int[] scores = new int[_frames.Length];
        int?[] cumulativeScores = new int?[_frames.Length];

        for (int index = 0; index < _frames.Length; index++)
        {
            var bonus = GetBonus(index + 1, _frames[index].Bonus);

            //Check if frame is already finished, and if has bonus
            if (index < Frame - 1 &&
                _frames[index].Scores.Any() &&
                bonus.HasValue)
            {
                scores[index] = _frames[index].Scores.Sum() + bonus.Value;
                cumulativeScores[index] = scores.Sum();
            }
            //Check if last frame - then simply sum all values
            else if (
                index == lastFrame - 1 &&
                (_frames[index].Scores.Length == 3 || 
                (_frames[index].Scores.Length == 2 && _frames[index].Scores.Sum() < TotalPins)))
            {
                scores[index] = _frames[index].Scores.Sum();
                cumulativeScores[index] = scores.Sum();
            }
        }

        return cumulativeScores;
    }

    public int GetTotalScore()
    {
        int totalScore = _frames.Sum(f => f.Scores.Sum());
        //Calculate bonuses
        for (int index = 0; index < _frames.Length - 1; index++)
        {
            var bonus = GetBonus(index + 1, _frames[index].Bonus);
            if (bonus.HasValue)
            {
                totalScore += bonus.Value;
            }
        }
        return totalScore;
    }

    public string[] GetThrowResults()
    {
        var throwResults = new List<string>();

        for (int frameIndex = 0; frameIndex < _frames.Length; frameIndex++)
        {
            var frameResult = _frames[frameIndex];

            var throwScores = frameResult.Scores;

            //Set throw scores
            for (int scoreIndex = 0; scoreIndex < throwScores.Length; scoreIndex++)
            {
                var score = throwScores[scoreIndex];
                string scoreText = score.ToString();
                //Check for Spare first
                if (scoreIndex == 1 &&
                    score != 0 &&
                    throwScores.Take(2).Sum() == TotalPins)
                {
                    scoreText = "/";
                }
                else if (score == TotalPins)
                {
                    scoreText = "X";
                }
                throwResults.Add(scoreText);
            }
            //If no score add appropriate number of null
            for (int i = 0; i < 2 - throwScores.Length; i++)
            {
                throwResults.Add(null);
            }

        }

        return throwResults.ToArray();
    }
    #endregion
    
    private FrameAction HandleLastFrame()
    {
        if (CurrentThrow <= 2 && _currentFrameScores.Sum() >= TotalPins)
        {
            return StandingPins == 0
                ? ResetPins()
                : FrameAction.Tidy;
        }
        else if (CurrentThrow == 1)
        {
            return FrameAction.Tidy;
        }
        else
        {
            FinishFrame();
            return FrameAction.EndGame;
        }
    }

    private int? GetBonus(int frameNumber, Bonus bonus)
    {
        if (bonus == Bonus.None)
            return 0;

        var nextFrames = _frames
            .Skip(frameNumber)
            .ToArray();

        List<int> nextScores = new List<int>();

        foreach (FrameResult nextFrame in nextFrames)
        {
            foreach (int score in nextFrame.Scores)
            {
                nextScores.Add(score);
                //If bonus score are ready return bonus value
                if (nextScores.Count == (int)bonus)
                {
                    return nextScores.Sum();
                }
            }
        }

        //If bonus score is not yet known return null
        return nextScores.Count == (int)bonus
            ? nextScores.Sum()
            : new int?();
    }

    private FrameAction FinishFrame()
    {
        SaveFrameResult();
        Frame++;
        return FrameAction.EndFrame;
    }

    private void SaveFrameResult()
    {
        if (Frame <= lastFrame)
            _frames[Frame - 1] = new FrameResult(_currentFrameScores.ToArray());
    }

    private FrameAction ResetPins()
    {
        StandingPins = TotalPins;
        return FrameAction.ResetPins;
    }

    private void OnFrameChanged(int value)
    {
        _frame = value;
        ResetPins();
        _currentFrameScores.Clear();
    }
}