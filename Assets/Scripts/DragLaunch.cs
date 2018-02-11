using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Ball))]
public class DragLaunch : MonoBehaviour
{
    const float realSpeedFactor = 0.01878f;

    private float _usedScreen = 0.8f;

    private Ball _ball;
    // Use this for initialization
    void Start()
    {
        _ball = GetComponent<Ball>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private float _dragStartTime;
    private Vector3 _dragStartPosition;

    public void DragStart()
    {
        //Capture time & position of drag start
        _dragStartTime = Time.realtimeSinceStartup;
        _dragStartPosition = Input.mousePosition;
    }


    private List<float> _ySpeed = new List<float>();

    public void DragEnd()
    {
        //Calculate velocity and launch the ball
        float dragDuration = Time.realtimeSinceStartup - _dragStartTime;
        Vector3 dragVelocity = (Input.mousePosition - _dragStartPosition) / dragDuration;// * realSpeedFactor;

        Vector3 launchVelocity = new Vector3(dragVelocity.x, 0, dragVelocity.y );

        //DEBUG
        _ySpeed.Add(dragVelocity.y / dragDuration);
        print($"Average ySpeed is: {_ySpeed.Average()} m/s, {_ySpeed.Average() /3.6f} km/h, current throw {launchVelocity.z/3.6f} km/h");

        //print($"Start position X: {_dragStartPosition.x}, Y: {_dragStartPosition.y}  Drag time: {dragTime}, Velocity: X: {throwVelocity.x}, Z: {throwVelocity.z}");


        _ball.Launch(launchVelocity);


    }
}
