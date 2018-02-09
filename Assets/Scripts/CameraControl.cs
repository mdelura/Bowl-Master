using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float zLimit = 1829;

    private Ball _ball;
    private Vector3 _offfset;


    // Use this for initialization
    void Start()
    {
        _ball = FindObjectOfType<Ball>();
        _offfset = transform.position - _ball.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < zLimit)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _ball.transform.position.z + _offfset.z);
            transform.position = _ball.transform.position + _offfset;
        }
    }
}
