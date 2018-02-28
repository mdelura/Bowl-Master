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

    bool _fallenMessageShown;

    // Update is called once per frame
    void Update()
    {
        if (!IsStanding() && !_fallenMessageShown)
        {
            _fallenMessageShown = true;
        }
    }

    public bool IsStanding()
    {
        float xEulerAbs = Mathf.Abs(transform.rotation.eulerAngles.x);
        float zEulerAbs = Mathf.Abs(transform.rotation.eulerAngles.z);
        xEulerAbs = xEulerAbs > 180 ? 360 - xEulerAbs : xEulerAbs;
        zEulerAbs = zEulerAbs > 180 ? 360 - zEulerAbs : zEulerAbs;

        return xEulerAbs < standingThreshold && zEulerAbs < standingThreshold;
    }
}
