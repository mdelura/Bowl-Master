using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Ball _ball;
    private GameObject _restartButton;
    private ScoreDisplay _scoreDisplay;


    public ScoreManager ScoreManager { get; private set; } = new ScoreManager();

    private bool _gameInProgress = true;
    public bool GameInProgress
    {
        get { return _gameInProgress; }
        private set { OnGameInProgressChanged(value); }
    }

    private void OnGameInProgressChanged(bool value)
    {
        if (_gameInProgress != value)
        {
            _gameInProgress = value;
            _restartButton.SetActive(!_gameInProgress);
        }
    }

    // Use this for initialization
    void Start()
    {
        _ball = FindObjectOfType<Ball>();

        _restartButton = GameObject.Find("Restart Button");
        _restartButton.SetActive(false);

        _scoreDisplay = FindObjectOfType<ScoreDisplay>();

        ScoreManager.Bowled += ScoreManager_Bowled;
    }

    private void ScoreManager_Bowled(FrameAction frameAction)
    {
        _ball.Reset();
        UpdateScoreDisplay();
        if (frameAction == FrameAction.EndGame)
        {
            GameInProgress = false;
        }
    }

    private void UpdateScoreDisplay() => _scoreDisplay.UpdateScoreDisplay(ScoreManager.GetThrowResults(), ScoreManager.GetCumulativeFrameScores());

    public void Restart()
    {
        ScoreManager.Reset();
        UpdateScoreDisplay();
        GameInProgress = true;
    }
}
