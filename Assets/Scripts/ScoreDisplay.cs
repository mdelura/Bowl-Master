using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    private Text[] _throwDisplays;
    private Text[] _cumulativeFrameDisplays;

    // Use this for initialization
    void Start()
    {
        var framesScores = new GameObject[transform.childCount];

        int index = 0;
        foreach (RectTransform item in transform)
        {
            framesScores[index] = item.gameObject;
            index++;
        }

        const string throwTextName = "Throw";

        _throwDisplays = framesScores
            .SelectMany(i => i.GetComponentsInChildren<Text>())
            .Where(t => t.name.Substring(0, throwTextName.Length) == throwTextName)
            .ToArray();

        _cumulativeFrameDisplays = framesScores
            .SelectMany(i => i.GetComponentsInChildren<Text>())
            .Where(t => t.name == "Frame Score")
            .ToArray();
    }

    public void UpdateScoreDisplay(string[] throwResults, int?[] cumulativeFrameScores)
    {
        for (int i = 0; i < throwResults.Length; i++)
        {
            _throwDisplays[i].text = throwResults[i];
        }

        for (int i = 0; i < cumulativeFrameScores.Length; i++)
        {
            _cumulativeFrameDisplays[i].text = cumulativeFrameScores[i].HasValue ? cumulativeFrameScores[i].Value.ToString() : null;
        }
    }
}
