using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{

    public float standingThreshold;

    // Use this for initialization
    void Start()
    {

    }

    bool _hasFallenMessage;

    // Update is called once per frame
    void Update()
    {
    }

    public bool IsStanding() =>Mathf.Abs(transform.rotation.eulerAngles.x) < standingThreshold && Mathf.Abs(transform.rotation.eulerAngles.z) < standingThreshold;
}
