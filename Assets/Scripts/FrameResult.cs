using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Bonus
{
    None = 0,
    Spare = 1,
    Strike = 2
}

public struct FrameResult
{
    private int[] _scores;

    public FrameResult(params int[] scores)
    {
        _scores = new int[scores.Length];
        for (int i = 0; i < scores.Length; i++)
        {
            _scores[i] = scores[i];
        }
    }

    public Bonus Bonus
    {
        get
        {
            if (Scores.FirstOrDefault() == ScoreManager.TotalPins)
            {
                return Bonus.Strike;
            }
            else if (Scores.Sum() >= ScoreManager.TotalPins)
            {
                return Bonus.Spare;
            }
            else
            {
                return Bonus.None;
            }

        }
    }
    public int[] Scores => _scores ?? new int[] { };
}