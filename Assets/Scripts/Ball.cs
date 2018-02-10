using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public float throwPower;

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        Launch();
    }

    public void Launch()
    {
        _rigidbody.velocity = Vector3.forward * throwPower + new Vector3(Random.Range(-0.1f, 0.1f), 0);
    }

    // Update is called once per frame
    void Update()
    {

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
