using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private Vector3 _startPosition;


    bool _isLaunched;
    public bool IsLaunched
    {
        get { return _isLaunched; }
        set
        {
            _isLaunched = value;
            OnIsLaunchedChanged(value);
        }
    }

    private void OnIsLaunchedChanged(bool isLaunched)
    {
        _rigidbody.useGravity = isLaunched;
        if (!isLaunched)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }


    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _audioSource = GetComponent<AudioSource>();
        _startPosition = transform.position;
    }

    const float speedDistance = 18.29f;
    float _launchStart;
    float _zStart;
    bool _speedShown;

    public void Launch(Vector3 velocity)
    {
        if (IsLaunched) return;

        _launchStart = Time.realtimeSinceStartup;
        _zStart = transform.position.z;
        IsLaunched = true;
        _rigidbody.velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z >= 18.29f && !_speedShown)
        {
            float distance = transform.position.z - _zStart;
            float duration = Time.realtimeSinceStartup - _launchStart;
            float speed = distance / duration;
            //print($"Distance: {distance}, Duration: {duration}, Speed: {speed} m/s, {speed / 3.6F} km/h.");
            _speedShown = true;
        }
    }

    bool _audioPlayed;

    private void OnCollisionEnter(Collision collision)
    {
        if (!_audioPlayed)
        {
            _audioSource.Play();
            _audioPlayed = true;
        }
    }

    public void Reset()
    {
        transform.SetPositionAndRotation(_startPosition, Quaternion.identity);
        IsLaunched = false;
    }
}
