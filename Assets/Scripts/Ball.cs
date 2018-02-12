using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    public bool IsLaunched { get; private set; }

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _audioSource = GetComponent<AudioSource>();
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
        _rigidbody.useGravity = true;
        _rigidbody.velocity = velocity;
        IsLaunched = true;
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

}
