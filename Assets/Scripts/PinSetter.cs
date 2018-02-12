using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    private Text _pinCountText;

    // Use this for initialization
    void Start()
    {
        _pinCountText = GameObject.Find("Pin Count").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        _pinCountText.text = CountStanding().ToString();
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


    private void OnTriggerExit(Collider other)
    {
        Destroy(other);

    }
}
