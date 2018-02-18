using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    public float pinsSettleWaitTime = 3;


    private Text _pinCountText;
    private bool _ballEnteredBox;
    private int _lastStandingCount = -1;
    private float _lastChangeTime;
    private Ball _ball;

    void Start()
    {
        _ball = FindObjectOfType<Ball>();
        _pinCountText = GameObject.Find("Pin Count").GetComponent<Text>();
        _pinCountText.text = CountStanding().ToString();
    }

    void Update()
    {
        if (_ballEnteredBox)
            CheckStanding();
    }

    public int CountStanding()
    {
        var pins = FindObjectsOfType<Pin>();

        int standingCount = 0;

        for (int i = 0; i < pins.Length; i++)
        {
            if (pins[i].IsStanding())
                standingCount++;
        }

        return standingCount;
    }

    private void CheckStanding()
    {
        int currentStandingCount = CountStanding();
        
        if (currentStandingCount != _lastStandingCount)
        {
            _lastChangeTime = Time.realtimeSinceStartup;
            _lastStandingCount = currentStandingCount;
            _pinCountText.text = _lastStandingCount.ToString();
        }
        else if ((Time.realtimeSinceStartup - _lastChangeTime) > pinsSettleWaitTime)
        {
            PinsHaveSettled();
        }
    }

    private void PinsHaveSettled()
    {
        _ballEnteredBox = false;
        _lastStandingCount = -1;
        _pinCountText.color = Color.green;
        _ball.Reset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
            _ballEnteredBox = true;
            _pinCountText.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pin = other.GetComponentInParent<Pin>();

        if (pin)
        {
            Destroy(pin.gameObject);
        }
    }
}
