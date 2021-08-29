using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerupTypes { NoPowerup = 0, AddTime = 1, MultiplyPoints = 2};

    public static readonly int TIME_TO_ADD=10;
    public static readonly int POINTS_MULTIPLIER = 2;
    public static readonly int TIME_FOR_POINTS_MULTIPLIER = 5;

    public delegate void PowerupAction();
    PowerupAction myAction;

    void Start()
    {
        myAction = NoPowerup;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ChooseNewPowerup();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ActivatePowerup();
        }
    }

    public void ChooseNewPowerup()
    {
         
        var random = new System.Random();
        int powerupIndex = random.Next(1, System.Enum.GetNames(typeof(PowerupTypes)).Length);

        if (powerupIndex == (int)PowerupTypes.AddTime)
        {
            myAction = AddTime;
        }
        else if (powerupIndex == (int)PowerupTypes.MultiplyPoints)
        {
            myAction = MultiplyPoints;
        }
        else
        {
            ResetPowerup();
        }
    }

    public void ActivatePowerup()
    {
        myAction.Invoke();
        print(myAction.Method.Name);
        ResetPowerup();
    }

    public void ResetPowerup()
    {
        myAction = NoPowerup;
    }

    public void NoPowerup()
    {

    }

    public void AddTime()
    {
        LocalGameManager.Instance.timer.AddTime(TIME_TO_ADD);
    }

    public void MultiplyPoints()
    {
        LocalGameManager.Instance.scoreBoard.AddMultiplier(POINTS_MULTIPLIER, TIME_FOR_POINTS_MULTIPLIER);
    }
}
