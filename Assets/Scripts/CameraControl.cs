using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float zLimit = 18.29F;

    private Ball _ball;
    private Vector3 _offfset;

    private Vector3 _startPosition;

    // Use this for initialization
    void Start()
    {
        _startPosition = transform.position;
        _ball = FindObjectOfType<Ball>();
        _offfset = transform.position - _ball.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < zLimit + _offfset.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _ball.transform.position.z + _offfset.z);
            transform.position = _ball.transform.position + _offfset;
        }
        else if (!_ball.IsLaunched)
        {
            transform.position = _startPosition;
        }
    }
}
