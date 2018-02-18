using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Ball))]
public class DragLaunch : MonoBehaviour
{
    const float realSpeedFactor = 0.01878f;

    private Ball _ball;

    private float _xBound;

    private bool _nudging;

    private float _nudgeXStartPos;

    private float _panelWidth;
    
    // Use this for initialization
    void Start()
    {
        _ball = GetComponent<Ball>();
        _xBound = GameObject.Find("Floor").transform.lossyScale.x / 2 - _ball.transform.lossyScale.x / 2;
        _panelWidth = Camera.main.ViewportToScreenPoint(transform.lossyScale).x;
    }

    // Update is called once per frame
    void Update()
    {
        if (_nudging && !_ball.IsLaunched)
        {
            float moveValue = Input.mousePosition.x - _nudgeXStartPos;
            float movePercent = Mathf.Clamp(moveValue / _panelWidth, -1, 1);
            float nudgeValue = movePercent * _xBound;

            var ballPosition = _ball.transform.position;
            ballPosition.x = nudgeValue;
            _ball.transform.position = ballPosition;
        }
    }

    public void NudgeStart()
    {
        _nudgeXStartPos = Input.mousePosition.x;
        _nudging = true;
    }

    public void NudgeEnd() => _nudging = false;

    public void Nudge(float xNudge)
    {
        if (_ball.IsLaunched) return;

        var ballPosition = _ball.transform.position;
        ballPosition.x = Mathf.Clamp(ballPosition.x + xNudge, - _xBound, _xBound);
        _ball.transform.position = ballPosition;
    }

    private float _dragStartTime;
    private Vector3 _dragStartPosition;

    public void DragStart()
    {
        //Capture time & position of drag start
        _dragStartTime = Time.time;
        _dragStartPosition = Input.mousePosition;
    }


    private List<float> _ySpeed = new List<float>();

    public void DragEnd()
    {
        //Calculate velocity and launch the ball
        float dragDuration = Time.time - _dragStartTime;
        Vector3 dragVelocity = (Input.mousePosition - _dragStartPosition) / dragDuration * realSpeedFactor;

        Vector3 launchVelocity = new Vector3(dragVelocity.x, 0, dragVelocity.y );

        //DEBUG
        _ySpeed.Add(dragVelocity.y / dragDuration);
        _ball.Launch(Vector3.forward * 15.5f);
        //print($"Average ySpeed is: {_ySpeed.Average()} m/s, {_ySpeed.Average() /3.6f} km/h, current throw {launchVelocity.z/3.6f} km/h");

        _ball.Launch(launchVelocity);
    }


}
