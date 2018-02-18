using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    public float pinsSettleWaitTime = 3;
    public float raisePinsYDistance = 0.4f;


    private Text _pinCountText;
    private bool _ballEnteredBox;
    private int _lastStandingCount = -1;
    private float _lastChangeTime;
    private Ball _ball;
    private Animator _animator;
    private int _throwInFrame;
    private GameObject _standingPins;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _ball = FindObjectOfType<Ball>();
        _pinCountText = GameObject.Find("Pin Count").GetComponent<Text>();
        _pinCountText.text = CountStanding().ToString();
        _standingPins = GameObject.Find("Standing Pins");
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
            _lastChangeTime = Time.time;
            _lastStandingCount = currentStandingCount;
            _pinCountText.text = _lastStandingCount.ToString();
        }
        else if ((Time.time - _lastChangeTime) > pinsSettleWaitTime)
        {
            PinsHaveSettled();
        }
    }

    private void PinsHaveSettled()
    {
        PrepareNextThrow();
        _ballEnteredBox = false;
        _lastStandingCount = -1;
        _pinCountText.color = Color.green;
        _ball.Reset();
    }

    private void PrepareNextThrow()
    {
        if (_throwInFrame > 1 || _lastStandingCount == 0)
        {
            _throwInFrame = 0;
            CleanPins();
            _animator.SetTrigger(AnimatorParam.ResetTrigger);
        }
        else
        {
            ReorganizeStandingPins();
            _animator.SetTrigger(AnimatorParam.TidyTrigger);
        }
    }

    private void CleanPins()
    {
        foreach (var pin in _standingPins.GetComponentsInChildren<Pin>())
        {
            pin.transform.parent = transform;
        }
    }

    private void ReorganizeStandingPins()
    {
        foreach (var pin in FindObjectsOfType<Pin>())
        {
            if (pin.IsStanding())
            {
                //Move standing pins to a Standing Pins grouping game object
                pin.transform.parent = _standingPins.transform;
                //Freeze them
                var pinRigidBody = pin.GetComponent<Rigidbody>();
                pinRigidBody.useGravity = false;
                pinRigidBody.freezeRotation = true;
                pinRigidBody.velocity = Vector3.zero;
                pinRigidBody.angularVelocity = Vector3.zero;
                pinRigidBody.isKinematic = true;
            }
        }
    }

    public void PrepareLoweredPins()
    {
        var pinsRigidBodies = _standingPins.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < pinsRigidBodies.Length; i++)
        {
            pinsRigidBodies[i].useGravity = true;
            pinsRigidBodies[i].freezeRotation = false;
            pinsRigidBodies[i].isKinematic = false;

            pinsRigidBodies[i].gameObject.transform.rotation = Quaternion.identity;
        }
    }

    public void RenewPins()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
            _ballEnteredBox = true;
            _throwInFrame++;
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

    class AnimatorParam
    {
        public const string TidyTrigger = "Tidy Trigger";
        public const string ResetTrigger = "Reset Trigger";

    }
}
