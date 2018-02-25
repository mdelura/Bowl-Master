using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum FrameAction
{
    Tidy,
    EndFrame,
    ResetPins,
    EndGame
}

public class ScoreManager
{
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

    public int TotalScore
    {
        get
        {
            int totalScore = _frames.Sum(f => f.Scores.Sum()) + _currentFrameScores.Sum();
            //Calculate bonuses
            for (int index = 0; index < _frames.Length - 1; index++)
            {
                totalScore += GetBonus(index + 1, _frames[index].Bonus);
            }
            return totalScore;
        }
    }

    private int GetBonus(int frameNumber, Bonus bonus)
    {
        if (bonus == Bonus.None)
            return 0;

        var nextFrames = _frames
            .Skip(frameNumber)
            .ToArray();

        List<int> nextScores = new List<int>();

        for (int f = 0; f < nextFrames.Length; f++)
        {
            for (int s = 0; s < nextFrames[f].Scores.Length; s++)
            {
                nextScores.Add(nextFrames[f].Scores[s]);
                if (nextScores.Count == (int)bonus)
                {
                    return nextScores.Sum();
                }
            }
        }

        return nextScores.Sum();
    }

    public int StandingPins
    {
        get { return _standingPins; }
        set
        {
            if (value < 0 || value > TotalPins)
                throw new UnityException("Invalid number of pins.");
            _standingPins = value;
        }
    }
    #endregion

    #region Public Methods
    public FrameAction Bowl(int pins)
    {
        StandingPins -= pins;
        _currentFrameScores.Add(pins);

        //Handle last frame
        if (Frame == lastFrame)
        {
            return HandleLastFrame();
        }

        //Finish frame if second throw or strike.
        if (CurrentThrow == 2 || StandingPins == 0)
        {
            return FinishFrame();
        }
        //Otherwise return Tidy
        else
        {
            return FrameAction.Tidy;
        }
    }

    public FrameResult GetFrameResult(int frameNumber) => _frames[frameNumber - 1];

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
            SaveFrameResult();
            _currentFrameScores.Clear();
            return FrameAction.EndGame;
        }
    }

    public void Reset()
    {
        Frame = 1;
        _frames = new FrameResult[lastFrame];

    } 
    #endregion

    private FrameAction FinishFrame()
    {
        SaveFrameResult();
        Frame++;
        return FrameAction.EndFrame;
    }

    private void SaveFrameResult() => _frames[Frame - 1] = new FrameResult(_currentFrameScores.ToArray());

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