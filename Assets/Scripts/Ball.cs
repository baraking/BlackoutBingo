using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ball
{
    public int ballValue;
    public float timePulled;

    public Ball(int value, float time)
    {
        ballValue = value;
        timePulled = time;
    }
}
