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

public class GameManager : MonoBehaviour
{
    public const int TotalPins = 10;
    const int lastFrame = 10;

    private int _frame = 1;
    private int _standingPins = TotalPins;
    private FrameResult[] _frames = new FrameResult[lastFrame];
    private List<int> _currentFrameScores = new List<int>();
    private GameObject[] _framesScores;
    private GameObject _restartButton;

    private void Start()
    {
        var scoresPanel = GameObject.Find("Scores Panel");

        _framesScores = new GameObject[scoresPanel.transform.childCount];

        int index = 0;
        foreach (RectTransform item in scoresPanel.transform)
        {
            _framesScores[index] = item.gameObject;
            index++;
        }

        _restartButton = GameObject.Find("Restart Button");
        _restartButton.SetActive(false);
    }

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
    }

    public int?[] CumulativeFrameScores
    {
        get
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
                else if (index == lastFrame - 1 &&
                    _frames[index].Scores.Any())
                {
                    scores[index] = _frames[index].Scores.Sum();
                    cumulativeScores[index] = scores.Sum();
                }
            }

            return cumulativeScores;
        }
    }

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

    private bool _gameFinished;
    public bool GameFinished
    {
        get { return _gameFinished; }
        private set { OnGameFinishedChanged(value); }
    }
    #endregion

    #region Public Methods
    public FrameAction Bowl(int pins)
    {
        StandingPins -= pins;
        _currentFrameScores.Add(pins);
        SaveFrameResult();
        UpdateScoreView();

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

    private void UpdateScoreView()
    {
        if (_framesScores == null || Frame > lastFrame)
        {
            return;
        }

        var cumulativeFrameScores = CumulativeFrameScores;

        for (int frameIndex = 0; frameIndex < Frame; frameIndex++)
        {
            var frameScoreView = _framesScores[frameIndex];
            var frameResult = _frames[frameIndex];

            var throwScores = frameIndex == Frame - 1999
                ? _currentFrameScores.ToArray()
                : frameResult.Scores;

            //Set throw scores
            if (throwScores.Any())
            {
                const string throwTextName = "Throw";

                var throwViews = frameScoreView.GetComponentsInChildren<Text>()
                    .Where(t => t.name.Substring(0, throwTextName.Length) == throwTextName)
                    .ToArray();

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
                        scoreText = "X";
                    throwViews[scoreIndex].text = scoreText;
                }
                //Set Spare mark if spare
                if (throwScores.Length == 2 && throwScores.Sum() == 10)
                {
                    throwViews[1].text = "/";
                }
            }

            //Set cumulative frame score
            if (cumulativeFrameScores[frameIndex].HasValue &&
                (Frame != lastFrame || GameFinished))
            {
                frameScoreView.GetComponentsInChildren<Text>()
                    .Single(t => t.name == "Frame Score").text = cumulativeFrameScores[frameIndex].Value.ToString();
            }
        }
    }

    public void Reset()
    {
        Frame = 1;
        _frames = new FrameResult[lastFrame];
        foreach (Text scoreDisplayItem in _framesScores.SelectMany(i => i.GetComponentsInChildren<Text>()))
        {
            scoreDisplayItem.text = null;
        }
        GameFinished = false;
    }
    #endregion

    private void OnGameFinishedChanged(bool value)
    {
        if (_gameFinished != value)
        {
            _gameFinished = value;
            _restartButton.SetActive(_gameFinished);
        }

    }
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
            GameFinished = true;

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
        UpdateScoreView();
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