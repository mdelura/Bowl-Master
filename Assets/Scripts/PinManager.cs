using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinManager : MonoBehaviour
{
    const float pinZStartPosition = 18.29f;

    public float pinsSettleWaitTime = 3;
    public float raisePinsYDistance = 0.4f;

    public GameObject pinSetPrefab;


    private Text _pinCountText;
    private int _currentPins = GameManager.TotalPins;
    private int _lastStandingCount = -1;
    private float _lastChangeTime;
    private Ball _ball;
    private Animator _animator;
    private GameObject _standingPins;
    private GameManager _gameManager;

    public bool PinsReady { get; private set; } = true;

    private void RaisePinsNotReady() => PinsReady = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _ball = FindObjectOfType<Ball>();
        //_pinCountText = GameObject.Find("Pin Count").GetComponent<Text>();
        _pinCountText = GameObject.Find("Info Text").GetComponent<Text>();
        _pinCountText.text = CountStanding().ToString();
        _standingPins = GameObject.Find("Standing Pins");
        _gameManager = new GameManager();
    }

    void Update()
    {
        if (_ball.IsLaunched)
            UpdateStandingCountAndSettle();
    }

    private void UpdateStandingCountAndSettle()
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

    private int CountStanding()
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

    private void PinsHaveSettled()
    {
        int fallenPins = _currentPins - _lastStandingCount;

        PreparePins(_gameManager.Bowl(fallenPins));
        _lastStandingCount = -1;
        _pinCountText.color = Color.green;
        _pinCountText.text = 
            $"Frame: {_gameManager.Frame}\r\n" +
            $"Fallen pins: {fallenPins}\r\n" +
            $"Current score: {_gameManager.TotalScore}\r\n" +
            $"Standing: {_currentPins}";
        _ball.Reset();
    }

    private void PreparePins(FrameAction frameAction)
    {
        if (frameAction == FrameAction.Tidy)
        {
            ReorganizeStandingPins();
            _animator.SetTrigger(AnimatorParam.TidyTrigger);
            _currentPins = _lastStandingCount;
        }
        else
        {
            CleanPins();
            _animator.SetTrigger(AnimatorParam.ResetTrigger);
            _currentPins = GameManager.TotalPins;
            //_pinCountText.text = _currentPins.ToString();
        }

        if (frameAction == FrameAction.EndGame)
        {
            //_gameManager.Reset();
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

        PinsReady = true;
    }

    public void RenewPins()
    {
        Instantiate(pinSetPrefab, new Vector3(0, 0.4f, pinZStartPosition), Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
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
