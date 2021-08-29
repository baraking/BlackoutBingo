using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    public enum PowerupTypes { NoPowerup = 0, AddTime = 1, MultiplyPoints = 2};

    public static readonly int TIME_TO_ADD=10;
    public static readonly int POINTS_MULTIPLIER = 2;
    public static readonly int TIME_FOR_POINTS_MULTIPLIER = 10;

    public static readonly int FILLED_METER = 100;

    public delegate void PowerupAction();
    PowerupAction myAction;

    public float powerupMeterValue;

    public Slider meter;
    public Image meterImage;

    public Color originalMeterColor;
    public Color filledMeterColor;

    public TMP_Text powerupText;

    void Start()
    {
        myAction = NoPowerup;
        meter.maxValue = FILLED_METER;
        meter.minValue = 0;
        meter.value = 0;
    }

    public void ResetMeter()
    {
        powerupMeterValue = 0;
        meter.value = 0;
        meterImage.color = originalMeterColor;
    }

    public void FillMeter(float valueToFill)
    {
        powerupMeterValue += valueToFill;
        if (powerupMeterValue >= FILLED_METER)
        {
            powerupMeterValue = FILLED_METER;
            LocalGameManager.Instance.audio.audioPlayer.PlayOneShot(LocalGameManager.Instance.audio.filledMeter, 0.5f);
            ChooseNewPowerup();
        }
        else if (powerupMeterValue < 0) 
        {
            powerupMeterValue = 0;
        }
        else if (powerupMeterValue < FILLED_METER)
        {
            ResetPowerup();
        }
        meter.value = powerupMeterValue;

        if (meter.value == FILLED_METER)
        {
            meterImage.color = filledMeterColor;
        }
        else
        {
            meterImage.color = originalMeterColor;
        }
    }

    public void ChooseNewPowerup()
    {
         
        var random = new System.Random();
        int powerupIndex = random.Next(1, System.Enum.GetNames(typeof(PowerupTypes)).Length);

        if (powerupIndex == (int)PowerupTypes.AddTime)
        {
            myAction = AddTime;
            powerupText.text = "Time";
        }
        else if (powerupIndex == (int)PowerupTypes.MultiplyPoints)
        {
            myAction = MultiplyPoints;
            powerupText.text = "X2";
        }
        else
        {
            ResetPowerup();
        }
    }

    public void ActivatePowerup()
    {
        myAction.Invoke();
        LocalGameManager.Instance.audio.audioPlayer.PlayOneShot(LocalGameManager.Instance.audio.powerup, 0.5f);
        print(myAction.Method.Name);
        ResetMeter();
        ResetPowerup();
    }

    public void ResetPowerup()
    {
        myAction = NoPowerup;
        powerupText.text = "";
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
